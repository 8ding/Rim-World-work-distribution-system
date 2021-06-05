using System;
using System.Collections;
using System.Collections.Generic;
using CodeMonkey;
using CodeMonkey.Utils;
using UnityEngine;

public class PL_TaskSystem
{
    //队列任务，需要满足调节才能出队的任务
    public class QueueTask
    {
        private Func<Task> tryGetTaskFunc;

        public QueueTask(Func<Task> tryGetTaskFunc)
        {
            this.tryGetTaskFunc = tryGetTaskFunc;
        }

        public Task TryDequeueTask()
        {
            return this.tryGetTaskFunc();
        }
    }
    public abstract class Task
    {
        public class MovePosition : Task
        {
            public Vector3 targetPosition;
        }
        public class Victory:Task
        {
        }
        public class Clean : Task
        {
            public Vector3 TargetPosition;
            public Action CleanOver;

            public Clean(Vector3 targetPosition, Action action)
            {
                this.TargetPosition = targetPosition;
                CleanOver = action;
            }
        }
        public class CarryWeapon:Task
        {
            public Vector3 WeaponPosition;
            public Vector3 WeaponSlotPosition;
            public Action<Transform> grabWeapon;
            public Action dropWeapon;
        }
    }

    private List<Task> taskList;
    private List<QueueTask> queueTaskList;
    public PL_TaskSystem()
    {
        taskList = new List<Task>();
        queueTaskList = new List<QueueTask>();
        //每0.2s执行一次任务出队
        FunctionPeriodic.Create(dequeueTasks, 0.2f);
    }

    public void EnqueueTask(QueueTask queueTask)
    {
        queueTaskList.Add(queueTask);
    }

    public void EnqueueTask(Func<Task> tryGetTaskFunc)
    {
        QueueTask queueTask = new QueueTask(tryGetTaskFunc);
        queueTaskList.Add(queueTask);
    }
    
    public  void dequeueTasks()
    {
        for (int i = 0; i < queueTaskList.Count; i++)
        {
            QueueTask queueTask = queueTaskList[i];
            Task task = queueTask.TryDequeueTask();
            if (task != null)
            {
                AddTask(task);
                queueTaskList.RemoveAt(i);
                i--;
                CMDebug.TextPopupMouse("Dequeue Task");
            }
            else
            {
                
            }
        }
    }

    public Task RequestTask()
    {
        if (taskList != null && taskList.Count > 0)
        {
            //给予请求者第一个任务
            Task task = taskList[0];
            taskList.Remove(task);
            return task;
        }
        else
        {
            return null;
        }
    }

    public void AddTask(Task task)
    {
        taskList.Add(task);
    }
}
