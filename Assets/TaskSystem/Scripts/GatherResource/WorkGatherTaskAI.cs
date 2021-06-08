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

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.WaitingForNextTask:
                //等待请求新任务
                worker.Idle();
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

        worker.moveTo(mineManager.GetGoldPointTransform().position, () =>
        {
            int mineTimes = mineManager.getGoldAmount() < canCarryAmount ? mineManager.getGoldAmount() : canCarryAmount;
            worker.Mine(mineTimes, () =>
            {
                task.GoldGrabed(mineTimes,mineManager);
                worker.Grab(mineTimes,(() =>
                {
                    if (worker.GetMaxCarryAmount()  - worker.GetCarryAmount() < 1 || mineManagers.Count < 1)
                    {
                        if (mineManager.IsHasGold())
                        {
                            mineManagers.Add(mineManager);
                        }
                        GameObject goldGameObject = MyClass.CreateWorldSprite(worker.gameObject.transform, "gold", "Item", GameAssets.Instance.gold,
                            new Vector3(0, 0.5f, 0), new Vector3(1, 1, 1), 1, Color.white);
                        worker.moveTo(task.StorePosition, (() =>
                        {
                            task.GoldDropde();
                            worker.Drop(goldGameObject,(() =>
                            {
                                GameResource.AddAmount(worker.GetCarryAmount());
                                state = State.WaitingForNextTask;
                            }));
                        }));
                    }
                    else
                    {
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
                }));

            });
        });
    }
    
}
