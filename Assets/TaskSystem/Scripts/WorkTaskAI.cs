using System;
using System.Collections;
using System.Collections.Generic;
using CodeMonkey;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class WorkerTaskAI : MonoBehaviour
{
    private enum State
    {
        WaitingForNextTask,
        ExecutingTask,
    }
    private IWorker worker;
    private PL_TaskSystem taskSystem;
    private State state;
    private float waitingTimer;
    
    public void setUp(IWorker worker, PL_TaskSystem taskSystem)
    {
        this.worker = worker;
        this.taskSystem = taskSystem;
        state = State.WaitingForNextTask;
    }

    private void Update()
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

    private void RequestNextTask()
    {
        //方便的Debug方式，留着后面看怎么实现的
        Debug.Log(gameObject.name + "Request");
        PL_TaskSystem.Task task = taskSystem.RequestTask();
        if (task == null)
        {
            state = State.WaitingForNextTask;
        }
        else
        {
            state = State.ExecutingTask;
            if(task is PL_TaskSystem.Task.MovePosition)
            {
                ExecuteTask_MovePosition(task as PL_TaskSystem.Task.MovePosition);
            }
            else if(task is PL_TaskSystem.Task.Victory)
            {
                ExecuteTask_Victory(task as PL_TaskSystem.Task.Victory);
            }
            else if(task is PL_TaskSystem.Task.Clean)
            {
                ExcuteTask_Clean(task as  PL_TaskSystem.Task.Clean);
            }
        }
    }

    private void ExecuteTask_MovePosition(PL_TaskSystem.Task.MovePosition task)
    {
        CMDebug.TextPopupMouse("Excute Task");
        worker.moveTo(task.targetPosition,(() =>
        {
            state = State.WaitingForNextTask;
        }));
    }
    private void ExecuteTask_Victory(PL_TaskSystem.Task.Victory task)
    {
        CMDebug.TextPopupMouse("Excute Task");
        worker.Victory(() =>
        {
            state = State.WaitingForNextTask;
        });
    }

    private void ExcuteTask_Clean(PL_TaskSystem.Task.Clean task)
    {
        CMDebug.TextPopupMouse("Excute Task");
        worker.moveTo(task.rubbish.position, () =>
        {
            worker.CleanUp(() =>
            {
                Destroy(task.rubbish.gameObject);
                state = State.WaitingForNextTask;
            });
        });
    }
}
