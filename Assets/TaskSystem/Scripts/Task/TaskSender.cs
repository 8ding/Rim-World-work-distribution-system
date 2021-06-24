using System;
using System.Collections.Generic;
using CodeMonkey;
using CodeMonkey.Utils;


public class QueueTask<TaskBase>
{
    private Func<TaskBase> tryGetTaskFunc;

    public QueueTask(Func<TaskBase> tryGetTaskFunc)
    {
        this.tryGetTaskFunc = tryGetTaskFunc;
    }

    public TaskBase TryDequeueTask()
    {
        return this.tryGetTaskFunc();
    }
}
/// <summary>
/// 任务发布者,存储各类型的任务并发派给对应的工人
/// </summary>
public class TaskSender
{
   

    private List<TaskBase> taskList;
    //队列任务，需要满足调节才能出队的任务
    private List<QueueTask<TaskBase>> queueTaskList;
    public TaskSender()
    {
        taskList = new List<TaskBase>();
        queueTaskList = new List<QueueTask<TaskBase>>();
        //每0.2s执行一次任务出队
//        FunctionPeriodic.Create(dequeueTasks, 0.2f);
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
