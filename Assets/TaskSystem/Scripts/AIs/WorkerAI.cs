
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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


public class WorkerAI : AIBase
{
 
    private State state;
    private float waitingTimer;
    private GameObject ResourceGameObject;

    private Dictionary<ResourceType, GameObject> resourceTypeIconDictionary;
    
    //工作类型与优先级对应的字典
    public  Dictionary<TaskType, int> jobtypeOrderDictionary;
    //按照工作优先级排列的工作列表
    private List<TaskType> jobTypeList;
    
    private IComparer<TaskType> compareType;
    
    public Action OnJobOrderChanged;

    public Action OnNotWorker;
    // Update is called once per frame
    private void Awake()
    {
        jobTypeList = new List<TaskType>();
        jobtypeOrderDictionary = new Dictionary<TaskType, int>();
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
    
    
    #region GatherResourceTask
    private void ExecuteTask_Gather(GatherResourceTask task)
    {
        GameHandler.ResourceManager resourceManager = task.resourceManager;
        Vector3 storePosition = task.StorePosition;
        int canCarryAmount = worker.GetMaxCarryAmount() - worker.GetCarryAmount();
        //工人前往资源点
        worker.moveTo(resourceManager.GetResourcePointTransform().position, () =>
        {
            int mineTimes = resourceManager.GetResourceAmount() < canCarryAmount
                ? resourceManager.GetResourceAmount()
                : canCarryAmount;
            //工人采集资源
            worker.Gather(mineTimes, resourceManager.ResourceType, () =>
            {
                //资源被工人捡起
                task.ResourceGrabed(mineTimes, resourceManager);
                //工人捡起资源
                worker.Grab(mineTimes, () =>
                {
                    //工人背满了
                    if (worker.GetMaxCarryAmount() - worker.GetCarryAmount() < 1)
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
        //资源点仍剩余资源,生成新任务,插入任务系统头部
        GatherResourceTask task = new GatherResourceTask
        {
            resourceManager = resourceManager,
            StorePosition = GameObject.Find("Crate").transform.position,
            ResourceGrabed = (amount, minemanager) => { minemanager.GiveResource(amount); }
        };
        switch (resourceManager.ResourceType)
        {
            case ResourceType.Gold:
                EventCenter.Instance.EventTrigger(TaskType.GatherGold.ToString(),task);
                break;
            case ResourceType.Wood:
                EventCenter.Instance.EventTrigger(TaskType.GatherWood.ToString(),task);
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
                    ResourceGameObject = GameAssets.Instance.createItemSprite(worker.gameObject.transform,
                        new Vector3(0, 0.5f, 0), ItemType.Gold);
                    break;
                case ResourceType.Wood:
                    ResourceGameObject = GameAssets.Instance.createItemSprite(worker.gameObject.transform,
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
        worker.moveTo(storePosition, (() =>
        {
            GameResource.AddAmount(GameResource.ResourceType.Gold, worker.GetCarryAmount());
            worker.Drop(ResourceGameObject, (() =>
            {
                worker.Idle();
                state = State.WaitingForNextTask;
            }));
        }));
    }
    #endregion

    #region MoveTask

    private void ExcuteTask_Move(WorkerMoveTask task)
    {
        worker.moveTo(task.Destination,(() =>
        {
            worker.Idle();
            state = State.WaitingForNextTask;
        }));
    }
    

    #endregion

    public void Disable()
    {
        OnNotWorker?.Invoke();
        this.enabled = false;
    }
}
