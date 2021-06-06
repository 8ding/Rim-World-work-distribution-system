using System;
using System.Collections;
using System.Collections.Generic;
using CodeMonkey;
using CodeMonkey.Utils;
using UnityEngine;

public abstract class TaskBase
{
    
}

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
public class PL_TaskSystem<TTask> where TTask : TaskBase
{
    //队列任务，需要满足调节才能出队的任务

   

    private List<TTask> taskList;
    private List<QueueTask<TTask>> queueTaskList;
    public PL_TaskSystem()
    {
        taskList = new List<TTask>();
        queueTaskList = new List<QueueTask<TTask>>();
        //每0.2s执行一次任务出队
        FunctionPeriodic.Create(dequeueTasks, 0.2f);
    }

    public void EnqueueTask(QueueTask<TTask> queueTask)
    {
        queueTaskList.Add(queueTask);
    }

    public void EnqueueTask(Func<TTask> tryGetTaskFunc)
    {
        QueueTask<TTask> queueTask = new QueueTask<TTask>(tryGetTaskFunc);
        queueTaskList.Add(queueTask);
    }
    
    public  void dequeueTasks()
    {
        for (int i = 0; i < queueTaskList.Count; i++)
        {
            QueueTask<TTask> queueTask = queueTaskList[i];
            TTask TTask = queueTask.TryDequeueTask();
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

    public TTask RequestTask()
    {
        if (taskList != null && taskList.Count > 0)
        {
            //给予请求者第一个任务
            TTask TTask = taskList[0];
            taskList.Remove(TTask);
            return TTask;
        }
        else
        {
            return null;
        }
    }

    public void AddTask(TTask TTask)
    {
        taskList.Add(TTask);
    }
}
