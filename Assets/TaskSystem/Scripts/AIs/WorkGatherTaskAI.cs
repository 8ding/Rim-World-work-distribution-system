using System.Collections;
using System.Collections.Generic;
using TaskSystem;
using TaskSystem.GatherResource;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;

public enum JobType
{
    GatherGold,
    GatherWood,
}

public struct JobOrder
{
    public JobType type;
    private int order;

    public JobOrder(JobType jobType, int order)
    {
        this.type = jobType;
        this.order = order;
    }
}
public class WorkGatherTaskAI : MonoBehaviour,ITaskAI
{
    private Woker worker;
    private PL_TaskSystem<TaskBase> taskSystem;
    private State state;
    private float waitingTimer;
    private GameObject ResourceGameObject;
    private TextMesh inventoryTextMesh;
    
    private List<JobOrder> jobOrderList;
    private Dictionary<ResourceType, GameObject> resourceTypeIconDictionary;
    
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

    public void setUp(IWorker worker)
    {
        this.worker = worker as Woker;
        state = State.WaitingForNextTask;
        worker.Idle();
        inventoryTextMesh = transform.Find("CarryAmount").GetComponent<TextMesh>();
        jobOrderList = new List<JobOrder>
        {
            new JobOrder(JobType.GatherGold, 4),
            new JobOrder(JobType.GatherWood, 4)
        };
        resourceTypeIconDictionary = new Dictionary<ResourceType, GameObject>();
    }

    private void updateInventory()
    {
        inventoryTextMesh.text = worker.GetCarryAmount().ToString();
    }

    
    public void RequestNextTask()
    {
        TaskBase task = null;
        for (int i = 0; i < jobOrderList.Count; i++)
        {
            switch (jobOrderList[i].type)
            {
                case JobType.GatherGold:
                    task = GameHandler.JobTypeTaskSystemDictionary[JobType.GatherGold]
                        .RequestTask();
                    break;
                case JobType.GatherWood:
                    task = GameHandler.JobTypeTaskSystemDictionary[JobType.GatherWood]
                        .RequestTask();
                    break;
            }
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
            if(task is TaskSystem.GatherResourceTask)
            {
                ExecuteTask_Gather(task as TaskSystem.GatherResourceTask);
            }
        }
    }

    private void ExecuteTask_Gather(GatherResourceTask task)
    {
        GameHandler.ResourceManager resourceManager = task.resourceManager;
        Vector3 storePosition = task.StorePosition;
        int canCarryAmount = worker.GetMaxCarryAmount() - worker.GetCarryAmount();
        //工人前往资源点
        worker.moveTo(resourceManager.GetResourcePointTransform().position, () =>
        {
            int mineTimes = resourceManager.GetResourceAmount() < canCarryAmount ? resourceManager.GetResourceAmount() : canCarryAmount;
            //工人采集资源
            worker.Mine(mineTimes, (() =>
            {
                worker.Grab(null);
            }), () =>
            {
                //资源被工人捡起
                task.ResourceGrabed(mineTimes,resourceManager);
                //工人背满了
                if (worker.GetMaxCarryAmount()  - worker.GetCarryAmount() < 1)
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
                                .RequestTask() as  GatherResourceTask;
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
    }
    /// <summary>
    /// 回收资源点生成新任务,并插入任务队列头部
    /// </summary>
    /// <param name="resourceManager"></param>
    private  void restoreResource(GameHandler.ResourceManager resourceManager)
    {
        //资源点仍剩余资源,生成新任务,插入任务系统头部
        GatherResourceTask task = new GatherResourceTask
        {
            resourceManager = resourceManager,
            StorePosition = GameObject.Find("Crate").transform.position,
            ResourceGrabed = (amount, minemanager) => { minemanager.GiveResource(amount); }
        };
        GameHandler.JobTypeTaskSystemDictionary[JobType.GatherGold].InsertTask(0, task);
        
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
                inventoryTextMesh.text = "";
                worker.Idle();
                state = State.WaitingForNextTask;
            }));
        }));
    }
}
