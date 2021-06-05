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
    private PL_TaskSystem<Task> taskSystem;
    private State state;
    private float waitingTimer;
    
    public void setUp(IWorker worker, PL_TaskSystem<Task> taskSystem)
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
        CMDebug.TextPopupMouse("RequestTask");
        Task task = taskSystem.RequestTask();
        if (task == null)
        {
            state = State.WaitingForNextTask;
        }
        else
        {
            state = State.ExecutingTask;
            if(task is Task.MovePosition)
            {
                ExecuteTask_MovePosition(task as Task.MovePosition);
            }
            else if(task is Task.Victory)
            {
                ExecuteTask_Victory(task as Task.Victory);
            }
            else if(task is Task.Clean)
            {
                ExcuteTask_Clean(task as  Task.Clean);
            }
            else if(task is Task.CarryWeapon)
            {
                ExcuteTask_CarryWeapon(task as  Task.CarryWeapon);
            }
        }
    }

    private void ExecuteTask_MovePosition(Task.MovePosition task)
    {
        CMDebug.TextPopupMouse("Excute Task");
        worker.moveTo(task.targetPosition,(() =>
        {
            state = State.WaitingForNextTask;
        }));
    }
    private void ExecuteTask_Victory(Task.Victory task)
    {
        CMDebug.TextPopupMouse("Excute Task");
        worker.Victory(() =>
        {
            state = State.WaitingForNextTask;
        });
    }

    private void ExcuteTask_Clean(Task.Clean task)
    {
        CMDebug.TextPopupMouse("Excute Task");
        worker.moveTo(task.TargetPosition, () =>
        {
            worker.CleanUp(() =>
            {
                task.CleanOver();
                state = State.WaitingForNextTask;
            });
        });
    }

    private void ExcuteTask_CarryWeapon(Task.CarryWeapon task)
    {
        worker.moveTo(task.WeaponPosition,(() =>
        {
            task.grabWeapon(transform);
            worker.moveTo(task.WeaponSlotPosition,(() =>
            {
                task.dropWeapon();
                state = State.WaitingForNextTask;
            }));
        }));
    }
}
