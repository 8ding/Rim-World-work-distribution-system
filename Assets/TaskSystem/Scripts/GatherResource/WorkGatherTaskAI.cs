using System.Collections;
using System.Collections.Generic;
using TaskSystem;
using TaskSystem.GatherResource;
using UnityEngine;

public class WorkGatherTaskAI : MonoBehaviour,ITaskAI
{
    private IWorker worker;
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
        this.worker = worker;
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
        worker.moveTo(task.GoldPosition, () =>
        {
            worker.Mine(() =>
            {
                task.GoldGrabed(transform);
                worker.Grab((() =>
                {
                    worker.moveTo(task.StorePosition,(() =>
                    {
                        task.GoldDropde();
                        worker.Drop((() =>
                        {
                            GameResource.AddAmount(1);
                            state = State.WaitingForNextTask;
                        }));
                    }));
                }));

            });
        });
    }
}
