using System.Collections;
using System.Collections.Generic;
using TaskSystem;
using TaskSystem.GatherResource;
using UnityEngine;

public class WorkGatherTaskAI : MonoBehaviour,ITaskAI
{
    private Woker worker;
    private PL_TaskSystem<TaskBase> taskSystem;
    private State state;
    private float waitingTimer;

    private TextMesh inventoryTextMesh;
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

    public void setUp(IWorker worker, PL_TaskSystem<TaskBase> taskSystem)
    {
        this.worker = worker as Woker;
        this.taskSystem = taskSystem;
        state = State.WaitingForNextTask;
        worker.Idle();
        inventoryTextMesh = transform.Find("CarryAmount").GetComponent<TextMesh>();
    }

    private void updateInventory()
    {
        inventoryTextMesh.text = worker.GetCarryAmount().ToString();
    }
    public void RequestNextTask()
    {
        TaskSystem.GatherTask task = taskSystem.RequestTask() as GatherTask;
        if (task == null)
        {
            state = State.WaitingForNextTask;
        }
        else
        {
            state = State.ExecutingTask;
            if(task is TaskSystem.GatherTask.GatherGold)
            {
                ExecuteTask_GatherGold(task as TaskSystem.GatherTask.GatherGold);
            }
        }
    }

    private void ExecuteTask_GatherGold(GatherTask.GatherGold task)
    {
        List<GameHandler.MineManager> mineManagers = task.mineManagerList;
        GameHandler.MineManager mineManager = task.mineManager;
        int canCarryAmount = worker.GetMaxCarryAmount() - worker.GetCarryAmount();
        //工人前往采矿点
        worker.moveTo(mineManager.GetGoldPointTransform().position, () =>
        {
            int mineTimes = mineManager.getGoldAmount() < canCarryAmount ? mineManager.getGoldAmount() : canCarryAmount;
            //工人挥舞采矿镰刀
            worker.Mine(mineTimes, (() =>
            {
                worker.Grab(null);
                updateInventory();
            }), () =>
            {
                //黄金被工人捡起
                task.GoldGrabed(mineTimes,mineManager);

                //工人背满了或者矿点没有了
                if (worker.GetMaxCarryAmount()  - worker.GetCarryAmount() < 1 || mineManagers.Count < 1)
                {
                    //工人回储存点
                    if (mineManager.IsHasGold())
                    {
                        mineManagers.Insert(0,mineManager);
                    }
                    GameObject goldGameObject = MyClass.CreateWorldSprite(worker.gameObject.transform, "gold", "Item", GameAssets.Instance.gold,
                        new Vector3(0, 0.5f, 0), new Vector3(1, 1, 1), 1, Color.white);
                    worker.moveTo(task.StorePosition, (() =>
                    {
                        task.GoldDropde();
                        GameResource.AddAmount(GameResource.ResourceType.Gold,worker.GetCarryAmount());
                        worker.Drop(goldGameObject,(() =>
                        {
                            inventoryTextMesh.text = "";
                            worker.Idle();
                            state = State.WaitingForNextTask;
                        }));
                    }));
                }
                else
                {
                    //工人没背满建立新任务,寻找下一个矿点
                    if (mineManagers.Count > 0)
                    {
                        mineManager = mineManagers[0];
                        mineManagers.RemoveAt(0);
                        GatherTask.GatherGold gatherGold = new GatherTask.GatherGold
                        {
                            mineManagerList = mineManagers,
                            mineManager = mineManager,
                            StorePosition = GameObject.Find("Crate").transform.position,
                            GoldGrabed = (amount, minemanager) =>
                            {
                                minemanager.giveGold(amount);
                            },
                            GoldDropde = task.GoldDropde
                        };
                        ExecuteTask_GatherGold(gatherGold);
                    }
                }
            });
        });
    }
    
}
