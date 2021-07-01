
using System.Collections.Generic;
using UnityEngine;


public interface IArgPack
{
    
}
/// <summary>
/// 任务参数封装对象，封装后接口改为可传任意参数
/// </summary>
/// <typeparam name="_TT"></typeparam>
public class TaskArgs<_TT> : IArgPack
{
    private _TT t1;
    public TaskArgs(_TT _t)
    {
        t1 = _t;
    }

    public _TT T1
    {
        get
        {
            return t1;
        }
        set
        {
            t1 = value;
        }
    }
}
public class TaskArgs<_TT1,_TT2> : IArgPack
{
    private _TT1 t1;
    private _TT2 t2;
    public TaskArgs(_TT1 _t1,_TT2 _t2)
    {
        t1 = _t1;
        t2 = _t2;
    }

    public _TT1 T1
    {
        get
        {
            return t1;
        }
        set
        {
            t1 = value;
        }
    }

    public _TT2 T2
    {
        get
        {
            return t2;
        }
        set
        {
            t2 = value;
        }
    }
}
public class TaskArgs<_TT1,_TT2,_TT3> : IArgPack
{
    private _TT1 t1;
    private _TT2 t2;
    private _TT3 t3;
    public TaskArgs(_TT1 _t1,_TT2 _t2,_TT3 _t3)
    {
        t1 = _t1;
        t2 = _t2;
        t3 = _t3;
    }

    public _TT1 T1
    {
        get
        {
            return t1;
        }
        set
        {
            t1 = value;
        }
    }

    public _TT2 T2
    {
        get
        {
            return t2;
        }
        set
        {
            t2 = value;
        }
    }

    public _TT3 T3
    {
        get
        {
            return t3;
        }
        set
        {
            t3 = value;
        }
    }
}
    
/// <summary>
/// 任务中心，生成任务并分发给相应的任务发布器，单位向任务中心请求任务，任务中心制定对应的任务发布器发布任务
/// </summary>
public class TaskCenter : BaseManager<TaskCenter>
{
    public static Dictionary<TaskType, TaskSender> taskDic= new Dictionary<TaskType, TaskSender>();


    public void BuildTask<T>(T _arg1, TaskType _taskType)
    {
        buildTask(new TaskArgs<T>(_arg1),_taskType);
    }

    public void BuildTask<T1, T2>(T1 _arg1, T2 _arg2, TaskType _taskType)
    {
        buildTask(new TaskArgs<T1,T2>(_arg1,_arg2), _taskType);
    }

    private void buildTask(IArgPack _argPack, TaskType _taskType)
    {
        if(_argPack != null)
        {
            if(!taskDic.ContainsKey(_taskType))
            {
                taskDic[_taskType] = new TaskSender();
            }
            switch (_taskType)
            {
                case TaskType.GatherGold:
                    case TaskType.GatherWood:
                    TaskArgs<Vector3> taskArgs1 = _argPack as TaskArgs<Vector3>;
                    taskDic[_taskType].AddTask(new GatherResourceTask(taskArgs1.T1, _taskType));
                    break;
                case TaskType.GoToPlace:
                    TaskArgs<Vector3> taskArgs2 = _argPack as TaskArgs<Vector3>;
                    taskDic[_taskType].AddTask(new WorkerMoveTask(taskArgs2.T1, _taskType));
                    break;
                case TaskType.CarryItem:
                    TaskArgs<Vector3> taskArgs3 = _argPack as TaskArgs<Vector3>;
                    taskDic[_taskType].AddTask(new CarryItemTask(taskArgs3.T1, _taskType));
                    break;
            }
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
