
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using CodeMonkey;
using TaskSystem;
using TaskSystem.GatherResource;
using UnityEngine;

public class Test
{
    public void SayHello()
    {
        Debug.Log("SayHello");
    }
}
public class compare : IComparer<JobType>
{
    public Dictionary<JobType, int> jobTypeOrderDictionary;
    public int Compare(JobType x, JobType y)
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
    public compare(Dictionary<JobType, int> dictionary)
    {
        this.jobTypeOrderDictionary = dictionary;
    }
}


public class WorkGatherTaskAI : MonoBehaviour,ITaskAI
{
    private Woker worker;
    private State state;
    private float waitingTimer;
    private GameObject ResourceGameObject;


    private Dictionary<ResourceType, GameObject> resourceTypeIconDictionary;
    
    //工作类型的配置
    public SettingOfJobType settingOfJobType;
    //工作类型与优先级对应的字典
    public  Dictionary<JobType, int> jobtypeOrderDictionary;
    //按照工作优先级排列的工作列表
    private List<JobType> jobTypeList;
    
    private IComparer<JobType> compareType;

    public Action OnJobOrderChanged;

    public Type t;
    // Update is called once per frame
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

    public void setUp(Woker worker)
    {
        this.worker = worker as Woker;
        settingOfJobType = Resources.Load("SettingOfJobTypeOrder") as SettingOfJobType;
        state = State.WaitingForNextTask;
        
        
        jobTypeList = new List<JobType>();
        jobtypeOrderDictionary = new Dictionary<JobType, int>();
        //依照scripableObject的数据对工作类型顺序列表 与 工作类型与优先级对应表 以及 工作类型与执行方法对应表进行初始化
        for (int i = 0; i < settingOfJobType.JobTypeList.Count; i++)
        {
            jobTypeList.Add(settingOfJobType.JobTypeList[i].jobType);
            jobtypeOrderDictionary.Add(settingOfJobType.JobTypeList[i].jobType,4);
        }
        //实例一个比较方法
        compareType = new compare(jobtypeOrderDictionary);
        resourceTypeIconDictionary = new Dictionary<ResourceType, GameObject>();
        //依照比较方法对工人的工作类型进行排序,工人按照该顺序去领取工作
        jobTypeList.Sort(compareType);
        
        worker.Idle();
        t = typeof(Test);
        // for (int i = 0; i < jobTypeList.Count; i++)
        // {
        //     Debug.Log(jobTypeList[i]);
        // }
    }

    public void ModifyOrder(JobType jobType)
    {
        int order;
        if (jobtypeOrderDictionary.TryGetValue(jobType, out order))
        {
            jobtypeOrderDictionary[jobType] = (jobtypeOrderDictionary[jobType] ) % 4 + 1;
        }
        else
        {
            jobtypeOrderDictionary[jobType] = 1;
        }
        jobTypeList.Sort(compareType);
        OnJobOrderChanged?.Invoke();
    }
    
    public void RequestNextTask()
    {
        TaskBase task = null;
        for (int i = 0; i < jobTypeList.Count; i++)
        {
            task = GameHandler.JobTypeTaskSystemDictionary[jobTypeList[i]]
                        .RequestTask();
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
            switch (task.jobType)
            {
                case JobType.GatherGold:
                case JobType.GatherWood:
                    ExecuteTask_Gather(task as GatherResourceTask);
                    break;
                default:
                    Debug.Log(task.jobType + "尚未编写执行方法");
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
                                task = GameHandler.JobTypeTaskSystemDictionary[JobType.GatherGold]
                                    .RequestTask() as GatherResourceTask;
                                break;
                            case ResourceType.Wood:
                                task = GameHandler.JobTypeTaskSystemDictionary[JobType.GatherWood]
                                    .RequestTask() as GatherResourceTask;
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
    /// 回收资源点生成新任务,并插入任务队列头部
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
                GameHandler.JobTypeTaskSystemDictionary[JobType.GatherGold].InsertTask(0, task);
                break;
            case ResourceType.Wood:
                GameHandler.JobTypeTaskSystemDictionary[JobType.GatherWood].InsertTask(0, task);
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

    
}
