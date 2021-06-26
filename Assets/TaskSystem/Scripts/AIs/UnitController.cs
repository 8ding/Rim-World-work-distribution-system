
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
    private TaskBase task = null; //单位正在执行的任务
    
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
                case TaskType.CarryItem:
                    ExcuteTask_CarryItem(task as CarryItemTask);
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
        if(task != null)
        {
           switch (task.taskType)
               {
                   case TaskType.GatherGold:
                   case TaskType.GatherWood:
                       if((task as GatherResourceTask).resourceManager.IsHasContent())
                       {
                           restoreResource((task as GatherResourceTask).resourceManager);
                       }
                       break;
               } 
        }
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
            EventCenter.Instance.RemoveEventListener<IArgs>(EventType.GetAxis, handleAxis);
            return;
        }
        state = State.Wondering;
        InputManager.Instance.StartOrEnd(true);
        EventCenter.Instance.AddEventListener<IArgs>(EventType.GetAxis, handleAxis);
    }
    #region GatherResourceTask
    private void ExecuteTask_Gather(GatherResourceTask task)
    {
        GameHandler.ResourceManager resourceManager = task.resourceManager;
        //工人前往资源点
        moveTo(resourceManager.GetContentTransform().position, () =>
        {
            StartCoroutine(Gather(resourceManager, (() =>
            {
                Idle();
                state = State.WaitingForNextTask;
            })));
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

    #region CarryItemtask

    private void ExcuteTask_CarryItem(CarryItemTask _task)
    {
        GameHandler.ItemManager itemManager = _task.itemManager;
        CMDebug.TextPopup("执行搬运任务",Vector3.zero,2f);
    }
    

    #endregion

    #region BeHaviar
     //移动行为之所以是行为 是因为做这件事可能不止表现层，还可能有逻辑层
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
        characterAnimation.PlayDirectMoveAnimation(unitData.CharacterId,direction,false);
        
    }
    //worker的闲置行为
    public void Idle()
    {
        moveWay.Disable();
        moveVelocity.Disable();
        characterAnimation.PlayobjectAnimaiton(unitData.CharacterId,ObjectAnimationType.Idle);
    }
    //worker的胜利行为
    public  void Victory(Action onVictoryEnd)
    {
        characterAnimation.PlayobjectAnimaiton(unitData.CharacterId,ObjectAnimationType.Throw);
        onVictoryEnd?.Invoke();
    }
    //worker的清扫行为
    public void CleanUp(Action onCleanEnd)
    {
        characterAnimation.PlayobjectAnimaiton(unitData.CharacterId, ObjectAnimationType.Clean);
        onCleanEnd?.Invoke();
    }
    /// <summary>
    /// 工人的采集行为,根据资源类型不同,采集动画也不一样
    /// </summary>
    /// <param name="resourceType"></param>
    /// <param name="OnGatherEnd"></param>
    public IEnumerator Gather(GameHandler.ResourceManager _resourceManager, Action OnGatherEnd = null)
    {
        int temp = _resourceManager.GetContentAmount();
 
        switch (_resourceManager.ResourceType)
        {
            case ResourceType.Gold:
                characterAnimation.PlayobjectAnimaiton(unitData.CharacterId,ObjectAnimationType.Mine,() =>{_resourceManager.GiveContent(1);});
                break;
            case ResourceType.Wood:
                characterAnimation.PlayobjectAnimaiton(unitData.CharacterId,ObjectAnimationType.Cut,() =>{_resourceManager.GiveContent(1);});
                break;
        }
        while (_resourceManager.IsHasContent())
        {
            yield return new WaitWhile(() =>
            {
                if(temp != _resourceManager.GetContentAmount())
                {
                    temp = _resourceManager.GetContentAmount();
                    return false;
                }
                return true;
            });
            int amount = 1;
            for (int i = 0; i < (int)MoveDirection.enumCount; i++)
            {
                
                Vector3 position = PathManager.Instance.GetOneOffsetPositon(gameObject.transform.position, (MoveDirection) i);
                if(PathManager.Instance.getAmountLeft(position, PlacedObjectType.Gold) > amount)
                {
                    PathManager.Instance.AddAmount(position, PlacedObjectType.Gold, amount);
                    break;
                }
            }
        }
        OnGatherEnd?.Invoke();
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
    
    #endregion

    private void OnDisable()
    {
        EventCenter.Instance.RemoveEventListener<IArgs>(EventType.ChangeMode,ChangeControlWay);
    }
}
