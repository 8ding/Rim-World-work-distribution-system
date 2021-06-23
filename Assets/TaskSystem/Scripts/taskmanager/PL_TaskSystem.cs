using System;
using System.Collections;
using System.Collections.Generic;
using CodeMonkey;
using CodeMonkey.Utils;
using TaskSystem;
using UnityEngine;



public class QueueTask<TTask> where TTask : TaskBase
{
    private Func<TTask> tryGetTaskFunc;

    public QueueTask(Func<TTask> tryGetTaskFunc)
    {
        this.tryGetTaskFunc = tryGetTaskFunc;
    }

    public TTask TryDequeueTask()
    {
        return this.tryGetTaskFunc();
    }
}
public class PL_TaskSystem
{
    //队列任务，需要满足调节才能出队的任务

   

    private List<TaskBase> taskList;
    private List<QueueTask<TaskBase>> queueTaskList;
    public PL_TaskSystem()
    {
        taskList = new List<TaskBase>();
        queueTaskList = new List<QueueTask<TaskBase>>();
        //每0.2s执行一次任务出队
        FunctionPeriodic.Create(dequeueTasks, 0.2f);
    }

    public void EnqueueTask(QueueTask<TaskBase> queueTask)
    {
        queueTaskList.Add(queueTask);
    }

    public void EnqueueTask(Func<TaskBase> tryGetTaskFunc)
    {
        QueueTask<TaskBase> queueTask = new QueueTask<TaskBase>(tryGetTaskFunc);
        queueTaskList.Add(queueTask);
    }
    
    public  void dequeueTasks()
    {
        for (int i = 0; i < queueTaskList.Count; i++)
        {
            QueueTask<TaskBase> queueTask = queueTaskList[i];
            TaskBase TTask = queueTask.TryDequeueTask();
            if (TTask != null)
            {
                AddTask(TTask);
                queueTaskList.RemoveAt(i);
                i--;
                CMDebug.TextPopupMouse("Dequeue Task");
            }
            else
            {
                
            }
        }
    }

    public TaskBase RequestTask()
    {
        if (taskList != null && taskList.Count > 0)
        {
            //给予请求者第一个任务
            TaskBase TTask = taskList[0];
            taskList.Remove(TTask);
            return TTask;

        }
        else
        {
            return null;
        }
    }

    public void AddTask(TaskBase TTask)
    {
        taskList.Add(TTask);
    }

    public void InsertTask(int index, TaskBase ttTask)
    {
        taskList.Insert(index,ttTask);
    }
}
