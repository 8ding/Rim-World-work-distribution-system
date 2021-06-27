using System;
using System.Collections.Generic;
using TaskSystem;
using UnityEngine;

public  class TaskBase
{
    public TaskType taskType;

    public TaskBase(TaskType _taskType)
    {
        taskType = _taskType;
    }
}

public class GatherResourceTask : TaskBase
{
    private Vector3 resourcePosition;

    public GatherResourceTask(Vector3 _position, TaskType _taskType) : base(_taskType)
    {
        resourcePosition = _position;
    }

    public Vector3 ResourcePosition
    {
        get
        {
            return resourcePosition;
        }
        set
        {
            resourcePosition = value;
        }
    }
}

public class WorkerMoveTask : TaskBase
{
    private Vector3 destination;

    public WorkerMoveTask(Vector3 _posiiton, TaskType _taskType) : base(_taskType)
    {
        destination = _posiiton;
    }
    public Vector3 Destination
    {
        get
        {
            return destination;
        }
        set
        {
            destination = value;
        }
    }
}


public class CarryItemTask : TaskBase
{
    private Vector3 itemPosition;
    private Vector3 storePosition;

    public CarryItemTask(Vector3 _itemPosition, Vector3 _storePosition, TaskType _taskType) : base(_taskType)
    {
        itemPosition = _itemPosition;
        storePosition = _storePosition;
    }

    public Vector3 ItemPosition
    {
        get
        {
            return itemPosition;
        }
        set
        {
            itemPosition = value;
        }
    }

    public Vector3 StorePosition
    {
        get
        {
            return storePosition;
        }
        set
        {
            storePosition = value;
        }
    }
}
