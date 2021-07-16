using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public enum EventType
{
    GetSomeKeyDown,//键盘按下
    GetSomeKeyUp, //键盘抬起
    GetAxis,    //轴移动
    SceneLoading, //场景加载中
    ClickGoldResource,//点击金矿资源点
    ClickWoodResource,
    RightClick,//右击事件
    ChangeMode,//改变玩家控制模式事件
    CreatMinePoit,
    CreateUnit,
    CreateItem,
    ItemOnGround,
    UnitOccur,
    ItemRemoveFromUnit,
    Test //测试专用事件
}
public interface IArgs
{
    
}

public class EventParameter<T> : IArgs
{
    public T t;
    public EventParameter(T t)
    {
        this.t = t;
    }
}

public class EventParameter<T1, T2> : IArgs
{
    public T1 t1;
    public T2 t2;
    public EventParameter(T1 t1,T2 t2)
    {
        this.t1 = t1;
        this.t2 = t2;
    }
}
public class EventParameter<T1, T2,T3> : IArgs
{
    public T1 t1;
    public T2 t2;
    public T3 t3;

    public EventParameter(T1 t1,T2 t2,T3 _t3)
    {
        this.t1 = t1;
        this.t2 = t2;
        this.t3 = _t3;
    }
}
public class EventParameter<T1, T2,T3,T4> : IArgs
{
    public T1 t1;
    public T2 t2;
    public T3 t3;
    public T4 t4;
    public EventParameter(T1 t1,T2 t2,T3 _t3,T4 _t4)
    {
        this.t1 = t1;
        this.t2 = t2;
        this.t3 = _t3;
        this.t4 = _t4;
    }
}
public interface IevnetInfo
{
    
}
public class EventInfo<T> : IevnetInfo
{
    public Action<T> action;
}

public class EventInfo<T1, T2> : IevnetInfo
{
    public Action<T1, T2> action;
}
public class EventInfo<T1, T2,T3> : IevnetInfo
{
    public Action<T1, T2,T3> action;
}
public class EventInfo<T1, T2,T3,T4> : IevnetInfo
{
    public Action<T1, T2,T3,T4>action;
}
public class EventCenter : BaseManager<EventCenter>
{
    //key-事件的名字（比如怪物死亡)
    //value - 对应的是监听这个事件对应的委托函数
    private Dictionary<EventType, IevnetInfo>eventDic  = new Dictionary<EventType, IevnetInfo>();
    
    /// <summary>
    /// 添加事件监听
    /// </summary>
    /// <param name="name">事件的名字</param>
    /// <param name="action">准备用来处理事件的委托函数</param>
    public void AddEventListener<T>(EventType name, Action<T> action)
    {
        if(eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T>).action += action;
        }
        else
        {
            eventDic.Add(name,new EventInfo<T>{action = action});;
        }
    }
    public void AddEventListener<T1,T2>(EventType name, Action<T1,T2> action)
    {
        if(eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T1,T2>).action += action;
        }
        else
        {
            eventDic.Add(name,new EventInfo<T1,T2>{action = action});;
        }
    }
    public void AddEventListener<T1,T2,T3>(EventType name, Action<T1,T2,T3> action)
    {
        if(eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T1,T2,T3>).action += action;
        }
        else
        {
            eventDic.Add(name,new EventInfo<T1,T2,T3>{action = action});;
        }
    }
    public void AddEventListener<T1,T2,T3,T4>(EventType name, Action<T1,T2,T3,T4> action)
    {
        if(eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T1,T2,T3,T4>).action += action;
        }
        else
        {
            eventDic.Add(name,new EventInfo<T1,T2,T3,T4>{action = action});;
        }
    }

    /// <summary>
    /// 事件的触发
    /// </summary>
    /// <param name="name">事件名字</param>
    /// <param name="args">事件传递的参数 由泛型类EventParamter包裹</param>
    public void EventTrigger<T>(EventType name,  T args)
    {
        if(eventDic.ContainsKey(name))
        {
            (eventDic[name]as EventInfo<T>).action?.Invoke(args);
        }
    }
    public void EventTrigger<T1,T2>(EventType name,  T1 args1,T2 args2)
    {
        if(eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T1, T2>).action?.Invoke(args1, args2);
        }
    }
    public void EventTrigger<T1,T2,T3>(EventType name,  T1 args1,T2 args2,T3 args3)
    {
        if(eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T1, T2, T3>).action?.Invoke(args1, args2, args3);
        }
    }
    public void EventTrigger<T1,T2,T3,T4>(EventType name,  T1 args1,T2 args2,T3 args3,T4 args4)
    {
        if(eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T1, T2, T3,T4>).action?.Invoke(args1, args2, args3,args4);
        }
    }
    /// <summary>
    /// 移除事件监听
    /// </summary>
    /// <param name="name">事件的名字</param>
    /// <param name="action">准备用来处理事件的委托函数</param>
    public void RemoveEventListener<T>(EventType name, Action<T> action)
    {
        if(eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T>).action -= action;
        }
    }
    public void RemoveEventListener<T1,T2>(EventType name, Action<T1,T2> action)
    {
        if(eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T1,T2>).action -= action;
        }
    }
    public void RemoveEventListener<T1,T2,T3>(EventType name, Action<T1,T2,T3> action)
    {
        if(eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T1,T2,T3>).action -= action;
        }
    }
    public void RemoveEventListener<T1,T2,T3,T4>(EventType name, Action<T1,T2,T3,T4> action)
    {
        if(eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T1,T2,T3,T4>).action -= action;
        }
    }
    /// <summary>
    /// 用于场景切换
    /// </summary>
    public void Clear()
    {
        eventDic.Clear();
    }
    
}


