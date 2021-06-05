using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PL_TaskSystem
{
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
            public Transform rubbish;
        }
    }

    private List<Task> taskList;
    
    public PL_TaskSystem()
    {
        taskList = new List<Task>();
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
