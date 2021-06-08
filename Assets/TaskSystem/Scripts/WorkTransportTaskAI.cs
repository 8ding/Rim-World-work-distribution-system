using System;
using System.Collections;
using System.Collections.Generic;
using CodeMonkey;
using TaskSystem;
using UnityEditor.PackageManager.Requests;
using UnityEngine;


public class WorkTransportTaskAI : MonoBehaviour,ITaskAI
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

    public void RequestNextTask()
    {
        //方便的Debug方式，留着后面看怎么实现的
        CMDebug.TextPopupMouse("RequestTask");
        TaskSystem.TransportTask task = taskSystem.RequestTask() as TransportTask;
        if (task == null)
        {
            state = State.WaitingForNextTask;
        }
        else
        {
            state = State.ExecutingTask;
            if (task is TaskSystem.TransportTask.TakeWeaponFromSlotPosition)
            {
                ExcuteTask_TakeWeaponFromSlotPosition(task as TaskSystem.TransportTask.TakeWeaponFromSlotPosition);
            }
        }
    }


    private void ExcuteTask_TakeWeaponFromSlotPosition(TaskSystem.TransportTask.TakeWeaponFromSlotPosition task)
    {
        worker.moveTo(task.weaponSlotPosition,(() =>
        {
            task.GrabWeapon(transform);
            worker.Grab(1,(() =>
            {
                worker.moveTo(task.targetPosition,(() =>
                {
                    task.dropWeapon();
                    worker.Drop((() =>
                    {
                        state = State.WaitingForNextTask;
                    }));
                }));
            }));
        }));
    }
}
