
using System.Collections.Generic;


//任务中心，从事件中心订阅事件，并在触发事件时，获得任务，并给予相应的任务发布者

public class TaskCenter : BaseManager<TaskCenter>
{
    public Dictionary<TaskType, TaskSender> taskDic;
    //构造函数中监听任务事件
    public TaskCenter ()
    {
        taskDic = new Dictionary<TaskType, TaskSender>();
        EventCenter.Instance.AddEventListener<IArgs>(EventType.ClickGoldResource, (_o =>
        {
            if(!taskDic.ContainsKey(TaskType.GatherGold))
            {
                taskDic[TaskType.GatherGold] = new TaskSender();
            }
            taskDic[TaskType.GatherGold].AddTask(new GatherResourceTask
            {
                taskType =  TaskType.GatherGold,
                resourceManager = (_o as EventParameter<GameHandler.ResourceManager>).t
            });
        }));
        
        EventCenter.Instance.AddEventListener<IArgs>(EventType.ClickWoodResource, (_o =>
        {
            
            if(!taskDic.ContainsKey(TaskType.GatherWood))
            {
                taskDic[TaskType.GatherWood] = new TaskSender();
            }
            taskDic[TaskType.GatherWood].AddTask(new GatherResourceTask
            {
                taskType = TaskType.GatherWood,
                resourceManager = (_o as EventParameter<GameHandler.ResourceManager>).t,
            });
        }));
        
        EventCenter.Instance.AddEventListener<IArgs>(EventType.RightClick,(_o =>
        {
            if(!taskDic.ContainsKey(TaskType.GoToPlace))
            {
                taskDic[TaskType.GoToPlace] = new TaskSender();
            }
            taskDic[TaskType.GoToPlace].AddTask(new WorkerMoveTask
            {
                taskType = TaskType.GoToPlace,
                Destination = (_o as EventParameter<UnityEngine.Vector3>).t
            });
        }));
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
