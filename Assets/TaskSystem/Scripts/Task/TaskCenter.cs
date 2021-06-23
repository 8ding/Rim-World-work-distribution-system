
using System.Collections.Generic;
//任务中心，从事件中心订阅事件，并在触发事件时，获得任务，并给予相应的任务发布者

public class TaskCenter : BaseManager<TaskCenter>
{
    public Dictionary<TaskType, TaskSender> taskDic = new Dictionary<TaskType, TaskSender>();
    //订阅任务相关事件的监听
    public void ListenTaskEvent()
    {
        for (int i = 0; i < (int) TaskType.enumcount; i++)
        {
            TaskType taskType = (TaskType) i;
            //监听任务相关事件，事件触发后，获得任务，并分发给相应的的任务发布器
            EventCenter.Instance.AddEventListener(((TaskType)i).ToString(), (_o =>
            {
                if(!taskDic.ContainsKey(taskType))
                {
                    taskDic[taskType] = new TaskSender();
                }
                taskDic[taskType].AddTask(_o as  TaskBase);
            }));
        }
    }
    
    /// <summary>
    /// 处理任务请求
    /// </summary>
    /// <param name="_taskType"></param>
    /// <returns></returns>
    public TaskBase handleTaskRequest(TaskType _taskType)
    {
        if(taskDic.ContainsKey(_taskType))
        {
            return taskDic[_taskType].RequestTask();
        }
        return null;
    }
    
}
