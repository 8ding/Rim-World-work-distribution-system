
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
    
    private Dictionary<ResourcePointType, GameObject> resourceTypeIconDictionary;
    
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
        resourceTypeIconDictionary = new Dictionary<ResourcePointType, GameObject>();
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
                    StartCoroutine(ExcuteTask_CarryItem(task as CarryItemTask));
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
        // if(task != null)
        // {
        //    switch (task.taskType)
        //        {
        //            case TaskType.GatherGold:
        //            case TaskType.GatherWood:
        //                GameHandler.ResourceManager resourceManager = PathManager.Instance.GetResourceManager((task as GatherResourceTask).ResourcePosition);
        //                if(resourceManager.IsHasContent())
        //                { 
        //                    //资源点仍剩余资源,重新构建任务
        //                    TaskCenter.Instance.BuildTask((task as GatherResourceTask).ResourcePosition,task.taskType);
        //                }
        //                break;
        //            case TaskType.GoToPlace:
        //                Idle();
        //                break;
        //        } 
        // }
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
        Vector3 position = task.ResourcePosition;
        //工人前往资源点
        moveTo(position, () =>
        {
            //开启采集协程
            StartCoroutine(Gather(1,position, () =>
            {
                Idle();
                state = State.WaitingForNextTask;
            }));
        });
    }

    /// <summary>
    /// 回收资源点生成新任务,并由事件中心发布
    /// </summary>
    /// <param name="_position"></param>
    private void restoreResource(Vector3 _position)
    {
        
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

    private IEnumerator ExcuteTask_CarryItem(CarryItemTask _task)
    {
        if(PathManager.Instance.IsHaveItemOnGround())
        {
            Item item = PathManager.Instance.GetNearestItem(gameObject.transform.position);
            Vector3 storePosition = _task.StorePosition;
            int itemCode = item.ItemCode;
            ItemDetails itemDetails = InventoryManager.Instance().GetItemDeatails(itemCode);
            while (unitData.Item == null || unitData.Item.ItemQuantity < itemDetails.MaxItemQuantity) 
            {
                item = PathManager.Instance.GetNearestItem(gameObject.transform.position, itemCode);
                if(item == null)
                {
                    break;
                }
                bool notGrabDone = true;
                Vector3 position = item.Position;
                moveTo(position,(() =>
                {
                    Grab(position,(() =>
                    {
                        notGrabDone = false;
                    }));
                }));
                yield return new WaitWhile(() => { return notGrabDone; });
            }
            moveTo(storePosition,(() =>
            {
                Drop((() =>
                {
                    Idle();
                    state = State.WaitingForNextTask;
                }));
            }));
        }
        else
        {
            Idle();
            state = State.WaitingForNextTask;
        }
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
    /// 单位的采集行为，根据采集资源不同，播放不同的动画
    /// </summary>
    /// <param name="_amount">工人一次采集动作所采集的数量</param>
    /// <param name="_position">资源位置</param>
    /// <param name="_onGatherEnd">采集完成后的回调函数</param>
    /// <returns></returns>
    public IEnumerator Gather(int _amount,Vector3 _position, Action _onGatherEnd = null)
    {
        Item ResourcePoint = PathManager.Instance.GetItemInGrid(_position);
        
        if(ResourcePoint != null && ResourcePoint.ItemQuantity != 0)
        {

            //根据资源点的类型决定其播放的动画，资源点也是一中堆叠类型，每播放完一次动画，触发一次回调，减少所在位置的堆叠类型内容物的数量
            switch (ResourcePoint.ItemCode)
            {
                case Item.GoldPoint:
                    characterAnimation.PlayobjectAnimaiton(unitData.CharacterId,ObjectAnimationType.Mine,() =>{
                        ResourcePoint.RemoveQuantity(1);
                    });
                    break;
                case Item.WoodPoint:
                    characterAnimation.PlayobjectAnimaiton(unitData.CharacterId, ObjectAnimationType.Cut, () =>
                    {
                        ResourcePoint.RemoveQuantity(1);
                    });
                    break;
            }
            //暂存所在位置堆叠类型的内容物数量
            int temp = ResourcePoint.ItemQuantity;
            int tempAmount = _amount;
            int produceCode = InventoryManager.Instance().GetItemDeatails(ResourcePoint.ItemCode).productItemCode;
            while (ResourcePoint.IsHasContent())
            {

                //协程直到内容物数量发生变化，即一次采集动作完毕,才继续执行后面的代码
                yield return new WaitWhile(() =>
                {
                    if(temp != ResourcePoint.ItemQuantity)
                    {
                        temp = ResourcePoint.ItemQuantity;
                        return false;
                    }
                    return true;
                });
                bool hasPut = false;
                int CircleTime = 1;
                while (tempAmount > 0)
                {
                    for (int i = -1 * CircleTime; i <= CircleTime; i++)
                    {
                        if(Mathf.Abs(i) != CircleTime)
                        {
                            int j = -1 * CircleTime;
                            int count = 0; 
                            while (count < 2)
                            {
                                tempAmount = putTest(i, j, produceCode, tempAmount);
                                
                                if(tempAmount == 0)
                                {
                                    break;
                                }
                                j *= -1;
                                count++;
                            }
                            if(tempAmount == 0)
                            {
                                break;
                            }
                        }
                        else
                        {
                            for (int j = -1 * CircleTime; j <= CircleTime; j++)
                            {
                                tempAmount = putTest(i, j, produceCode, tempAmount);
                                if(tempAmount == 0)
                                {
                                    break;
                                }
                            }
                            if(tempAmount == 0)
                            {
                                break;
                            }
                        }

                    }
                    CircleTime++;
                }
                tempAmount = _amount;
            }
            //采集完毕的回调;
            Debug.Log("GatherOver");
        }
        _onGatherEnd?.Invoke();
    }

    private int putTest(int i, int j, int _prouduceCode, int tempAmount)
    {
        UnityEngine.Vector3 position = PathManager.Instance.GetMoveXYPosition(gameObject.transform.position, i, j);
        Item _item = PathManager.Instance.GetItemInGrid(position);
        ItemDetails  produceDetails = InventoryManager.Instance().GetItemDeatails(_prouduceCode);
        if(_item != null)
        {
            if(_item.ItemCode == _prouduceCode)
                tempAmount =  _item.AddQuantity(tempAmount);
        }
        else
        {
            _item = InventoryManager.Instance().CreateItem(_prouduceCode, tempAmount);
            PathManager.Instance.SetItemOnGrid(_item,position);
            tempAmount = 0;
        }
        return tempAmount;
    }

    public void Grab(Vector3 _position, Action OnGrabEnd = null)
    {

        Item itemInGrid = PathManager.Instance.GetItemInGrid(_position);
        if(itemInGrid != null && itemInGrid.IsHasContent())
        {
            Item itemOnUnit = unitData.Item;
            if(itemOnUnit == null)
            {
                Item newItem = InventoryManager.Instance().CreateItem(itemInGrid.ItemCode, itemInGrid.ItemQuantity);
                unitData.SetItemOnUnit(newItem);
                itemInGrid.RemoveQuantity(itemInGrid.ItemQuantity);
            }
            else
            {
                int left = unitData.Item.AddQuantity(itemInGrid.ItemQuantity);
                itemInGrid.RemoveQuantity(itemInGrid.ItemQuantity- left);
                if(left > 0)
                {
                    PathManager.Instance.SetItemOnGrid(itemInGrid,itemInGrid.Position);
                }
            }
        }
        OnGrabEnd?.Invoke();
    }
    
    

    public void Drop(Action OnDropEnd = null)
    {
        unitData.Item.RemoveQuantity(InventoryManager.Instance().GetItemDeatails(unitData.Item.ItemCode).MaxItemQuantity);
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
