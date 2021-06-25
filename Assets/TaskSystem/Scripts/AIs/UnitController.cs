
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;
using CodeMonkey;
using TaskSystem;
using TaskSystem.GatherResource;
using UnityEngine;


public class compare : IComparer<TaskType>
{
    public Dictionary<TaskType, int> jobTypeOrderDictionary;
    public int Compare(TaskType x, TaskType y)
    {
        if (jobTypeOrderDictionary[x] < jobTypeOrderDictionary[y])
        {
            return -1;
        }
        else if(jobTypeOrderDictionary[x] > jobTypeOrderDictionary[y])
        {
            return 1;
        }
        else if(x < y)
        {
            return -1;
        }
        else if(y < x)
        {
            return 1;
        }
        return 0;
    }
    public compare(Dictionary<TaskType, int> dictionary)
    {
        this.jobTypeOrderDictionary = dictionary;
    }
}


public class UnitController : AIBase
{
 
    private State state;
    private float waitingTimer;
    private GameObject ResourceGameObject;


    private IMovePosition moveWay;
    private IMoveVelocity moveVelocity;
    private CharacterAnimation characterAnimation;
    
    private Dictionary<ResourceType, GameObject> resourceTypeIconDictionary;
    
    //工作类型与优先级对应的字典
    public  Dictionary<TaskType, int> jobtypeOrderDictionary;
    //按照工作优先级排列的工作列表
    private List<TaskType> jobTypeList;
    
    private IComparer<TaskType> compareType;

    private UnitData unitData;
    public Action OnJobOrderChanged;
    
    private void Awake()
    {
        jobTypeList = new List<TaskType>(); 
        jobtypeOrderDictionary = new Dictionary<TaskType, int>();
        
        EventCenter.Instance.AddEventListener<IArgs>(EventType.ChangeMode, ChangeControlWay);
        
        //单位的相关组件与数据
        unitData = GetComponent<UnitData>();
        moveWay = gameObject.GetComponent<IMovePosition>();
        characterAnimation = gameObject.GetComponent<CharacterAnimation>();
        moveVelocity = gameObject.GetComponent<IMoveVelocity>();
        
        Idle();
    }

    private void Start()
    {
        //依照scripableObject的数据对工作类型顺序列表 与 工作类型与优先级对应表 以及 工作类型与执行方法对应表进行初始化
        for (int i = 0; i < (int)TaskType.enumcount; i++)
        {
            jobTypeList.Add((TaskType)i);
            jobtypeOrderDictionary.Add((TaskType)i,4);
        }
        //实例一个比较方法
        compareType = new compare(jobtypeOrderDictionary);
        resourceTypeIconDictionary = new Dictionary<ResourceType, GameObject>();
        //依照比较方法对工人的工作类型进行排序,工人按照该顺序去领取工作
        jobTypeList.Sort(compareType);
        state = State.WaitingForNextTask;
    }

    void Update()
    {
        switch (state)
        {
            case State.WaitingForNextTask:
                //等待请求新任务
                waitingTimer -= Time.deltaTime;
                if (waitingTimer <= 0)
                {
                    float waitingTimerMax = .2f;
                    waitingTimer = waitingTimerMax;
                    RequestNextTask();
                }
                break;
            case State.ExecutingTask:
                break;
            //离开工作模式，进入玩家控制模式
            case State.Wondering:
                InputManager.Instance.StartOrEnd(true);
                EventCenter.Instance.AddEventListener<IArgs>(EventType.GetAxis, handleAxis);
                break;
        }
    }

    public void handleAxis(IArgs eventArgs)
    {
        EventParameter<Vector3> eventParameter = eventArgs as EventParameter<Vector3>;
        if(!eventParameter.t.Equals(Vector3.zero))
        {
            moveTowards(eventParameter.t);
        }
        else
        {
            Idle();
        }
    }

    public void ModifyOrder(TaskType _taskType)
    {
        int order;
        if (jobtypeOrderDictionary.TryGetValue(_taskType, out order))
        {
            jobtypeOrderDictionary[_taskType] = (jobtypeOrderDictionary[_taskType] ) % 4 + 1;
        }
        else
        {
            jobtypeOrderDictionary[_taskType] = 1;
        }
        jobTypeList.Sort(compareType);
        OnJobOrderChanged?.Invoke();
    }
    
    public void RequestNextTask()
    {
        TaskBase task = null;
        for (int i = 0; i < jobTypeList.Count; i++)
        {
            //任务中心处理工人的任务请求
            task = TaskCenter.Instance.handleTaskRequest(jobTypeList[i]);
            if (task != null)
            {
                break;
            } 
        }
        if (task == null)
        {
            state = State.WaitingForNextTask;
        }
        else
        {
            state = State.ExecutingTask;
            switch (task.taskType)
            {
                case TaskType.GatherGold:
                case TaskType.GatherWood:
                    ExecuteTask_Gather(task as GatherResourceTask);
                    break;
                case TaskType.GoToPlace:
                    ExcuteTask_Move(task as WorkerMoveTask);
                    break;
                default:
                    Debug.Log(task.taskType + "尚未编写执行方法");
                    state = State.WaitingForNextTask;
                    break;
            }

        }
    }

    private void interrupt()
    {
        
    }

    private void ChangeControlWay(IArgs _args)
    {
        if(state == State.WaitingForNextTask)
        {
            interrupt();
        }
        if(state == State.Wondering)
        {
            state = State.WaitingForNextTask;
            return;
        }
        state = State.Wondering;
    }
    #region GatherResourceTask
    private void ExecuteTask_Gather(GatherResourceTask task)
    {
        GameHandler.ResourceManager resourceManager = task.resourceManager;
        Vector3 storePosition = task.storePosition;
        int canCarryAmount = unitData.GetMaxCarryAmount() - unitData.GetCarryAmount();
        //工人前往资源点
        moveTo(resourceManager.GetResourcePointTransform().position, () =>
        {
            int mineTimes = resourceManager.GetResourceAmount() < canCarryAmount
                ? resourceManager.GetResourceAmount()
                : canCarryAmount;
            //工人采集资源
            Gather(mineTimes, resourceManager.ResourceType, () =>
            {
                //资源被工人捡起
                resourceManager.GiveResource(mineTimes);
                //工人捡起资源
                Grab(mineTimes, () =>
                {
                    //工人背满了
                    if (unitData.GetMaxCarryAmount() - unitData.GetCarryAmount() < 1)
                    {
                        //如果资源点仍有剩余资源则回收资源点
                        if (resourceManager.IsHasResource())
                        {
                            restoreResource(resourceManager);
                        }

                        //生成资源物品，工人回储存点
                        CreateResourceIcon(resourceManager.ResourceType);
                        ExcuteGatherBack(storePosition);
                    }
                    //工人没背满,继续请求当前收集资源的相关任务
                    else
                    {
                        switch (resourceManager.ResourceType)
                        {
                            case ResourceType.Gold:
                                task = TaskCenter.Instance.handleTaskRequest(TaskType.GatherGold) as GatherResourceTask;
                                break;
                            case ResourceType.Wood:
                                task = TaskCenter.Instance.handleTaskRequest(TaskType.GatherWood) as GatherResourceTask;
                                break;
                        }

                        //仍有相关任务则执行，无任务返回存储点
                        if (task != null)
                        {
                            ExecuteTask_Gather(task);
                        }
                        else
                        {
                            CreateResourceIcon(resourceManager.ResourceType);
                            ExcuteGatherBack(storePosition);
                        }
                    }
                });

            });
        });
    }

    /// <summary>
    /// 回收资源点生成新任务,并由事件中心发布
    /// </summary>
    /// <param name="resourceManager"></param>
    private void restoreResource(GameHandler.ResourceManager resourceManager)
    {
        //资源点仍剩余资源,重新触发事件
        
        switch (resourceManager.ResourceType)
        {
            case ResourceType.Gold:
                EventCenter.Instance.EventTrigger<IArgs>(EventType.ClickGoldResource,new EventParameter<GameHandler.ResourceManager>(resourceManager));
                break;
            case ResourceType.Wood:
                EventCenter.Instance.EventTrigger<IArgs>(EventType.ClickWoodResource, new EventParameter<GameHandler.ResourceManager>(resourceManager));
                break;
        }

    }

    /// <summary>
    /// 生成与资源对应的物品图标
    /// </summary>
    /// <param name="resourceType"></param>
    private void CreateResourceIcon(ResourceType resourceType)
    {
        if (!resourceTypeIconDictionary.TryGetValue(resourceType,
            out ResourceGameObject))
        {
            switch (resourceType)
            {
                case ResourceType.Gold:
                    ResourceGameObject = GameAssets.Instance.createItemSprite(gameObject.transform,
                        new Vector3(0, 0.5f, 0), ItemType.Gold);
                    break;
                case ResourceType.Wood:
                    ResourceGameObject = GameAssets.Instance.createItemSprite(gameObject.transform,
                        new Vector3(0, 0.5f, 0), ItemType.Wood);
                    break;
            }

            resourceTypeIconDictionary[resourceType] = ResourceGameObject;
        }
        else
        {
            ResourceGameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 结束资源收集回到存储点并放下资源
    /// </summary>
    /// <param name="storePosition"></param>
    private void ExcuteGatherBack(Vector3 storePosition)
    {
        moveTo(storePosition, (() =>
        {
            GameResource.AddAmount(GameResource.ResourceType.Gold, unitData.GetCarryAmount());
            Drop(ResourceGameObject, (() =>
            {
                Idle();
                state = State.WaitingForNextTask;
            }));
        }));
    }
    #endregion

    #region MoveTask

    private void ExcuteTask_Move(WorkerMoveTask task)
    {
        moveTo(task.Destination,(() =>
        {
            Idle();
            state = State.WaitingForNextTask;
        }));
    }
    

    #endregion
        //worker的移动行为之所以是行为 是因为做这件事可能不止表现层，还可能有逻辑层
    public void moveTo(Vector3 position, Action onArriveAtPosition = null)
    {
        //启动移动方式,到达目标 事件赋值给移动方式的移动结束后处理,赋值而不是加,避免上次移动结束的事件仍会被触发
       
        moveWay.Enable();
        moveVelocity.Enable();
        Action idle = () => { Idle(); };
        idle += onArriveAtPosition;
        moveWay.BindOnPostMoveEnd(idle);
        moveWay.SetMovePosition(position);
    }

    public void moveTowards(Vector3 direction)
    {
        moveVelocity.Enable();
        moveVelocity.SetVelocity(direction);
        characterAnimation.PlayDirectMoveAnimation(characterId,direction,false);
        
    }
    //worker的闲置行为
    public void Idle()
    {
        moveWay.Disable();
        moveVelocity.Disable();
        characterAnimation.PlayobjectAnimaiton(characterId,0,ObjectAnimationType.Idle);
    }
    //worker的胜利行为
    public void Victory(int loopTimes,Action onVictoryEnd)
    {
        characterAnimation.OnAnimationEnd = onVictoryEnd;
        characterAnimation.PlayobjectAnimaiton(characterId,0,ObjectAnimationType.Throw);
    }
    //worker的清扫行为
    public void CleanUp(int loopTimes,Action onCleanEnd)
    {
        characterAnimation.OnAnimationEnd = onCleanEnd;
        characterAnimation.PlayobjectAnimaiton(characterId,loopTimes, ObjectAnimationType.Clean);
    }
    /// <summary>
    /// 工人的采集行为,根据资源类型不同,采集动画也不一样
    /// </summary>
    /// <param name="resourceType"></param>
    /// <param name="OnGatherEnd"></param>
    public void Gather(int actTimes,ResourceType resourceType, Action OnGatherEnd = null)
    {
        characterAnimation.OnAnimationEnd = OnGatherEnd;
        switch (resourceType)
        {
            case ResourceType.Gold:
                characterAnimation.PlayobjectAnimaiton(characterId,actTimes,ObjectAnimationType.Mine);
                break;
            case ResourceType.Wood:
                characterAnimation.PlayobjectAnimaiton(characterId,actTimes,ObjectAnimationType.Cut);
                break;
        }
    }

    public void Grab(int amount, Action OnGrabEnd = null)
    {
        unitData.AddCarryAmount(amount);
        OnGrabEnd?.Invoke();
    }
    
    

    public void Drop(Action OnDropEnd = null)
    {
        unitData.ClearCarry();
        OnDropEnd?.Invoke();
    }
    

    public void Drop(GameObject gameObject,Action OnDropEnd = null)
    {
        gameObject.SetActive(false);
        Drop(OnDropEnd);
    }
    
}
