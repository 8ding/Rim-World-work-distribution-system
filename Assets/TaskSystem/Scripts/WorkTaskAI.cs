using System;
using System.Collections;
using System.Collections.Generic;
using CodeMonkey;
using TaskSystem;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public enum State
{
    WaitingForNextTask,
    ExecutingTask,
}
public class WorkerTaskAI : MonoBehaviour,ITaskAI
{

    private IWorker worker;
    private PL_TaskSystem<TaskBase> taskSystem;
    private State state;
    private float waitingTimer;
    
    public void setUp(IWorker worker, PL_TaskSystem<TaskBase> taskSystem)
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



    public void RequestNextTask()
    {
        //方便的Debug方式，留着后面看怎么实现的
        CMDebug.TextPopupMouse("RequestTask");
        TaskSystem.Task task = taskSystem.RequestTask() as Task;
        if (task == null)
        {
            state = State.WaitingForNextTask;
        }
        else
        {
            state = State.ExecutingTask;
            if(task is TaskSystem.Task.MovePosition)
            {
                ExecuteTask_MovePosition(task as TaskSystem.Task.MovePosition);
            }
            else if(task is TaskSystem.Task.Victory)
            {
                ExecuteTask_Victory(task as TaskSystem.Task.Victory);
            }
            else if(task is TaskSystem.Task.Clean)
            {
                ExcuteTask_Clean(task as  TaskSystem.Task.Clean);
            }
            else if(task is TaskSystem.Task.CarryWeapon)
            {
                ExcuteTask_CarryWeapon(task as  TaskSystem.Task.CarryWeapon);
            }
        }
    }

    private void ExecuteTask_MovePosition(TaskSystem.Task.MovePosition task)
    {
        CMDebug.TextPopupMouse("Excute Task");
        worker.moveTo(task.targetPosition,(() =>
        {
            worker.Idle();
            state = State.WaitingForNextTask;
        }));
    }
    private void ExecuteTask_Victory(TaskSystem.Task.Victory task)
    {
        CMDebug.TextPopupMouse("Excute Task");
        worker.Victory(() =>
        {
            worker.Idle();
            state = State.WaitingForNextTask;
        });
    }

    private void ExcuteTask_Clean(TaskSystem.Task.Clean task)
    {
        CMDebug.TextPopupMouse("Excute Task");
        worker.moveTo(task.TargetPosition, () =>
        {
            worker.CleanUp(() =>
            {
                worker.Idle();
                task.CleanOver();
                state = State.WaitingForNextTask;
            });
        });
    }

    private void ExcuteTask_CarryWeapon(TaskSystem.Task.CarryWeapon task)
    {
        worker.moveTo(task.WeaponPosition,(() =>
        {
            //两个对象,task中存储的是客体被抓起的执行，而worker这边是主体执行的抓取行为
            task.weaponGrabed(transform);
            worker.Grab((() =>
            {
                worker.moveTo(task.WeaponSlotPosition,(() =>
                {
                    //两个对象,task中存储的是客体被放下的执行，而worker这边是主体执行的放下行为
                    task.weaponDroped();
                    worker.Drop((() =>
                    {
                        worker.Idle();
                        state = State.WaitingForNextTask;
                    }));
                }));
            }));
        }));
    }
}
