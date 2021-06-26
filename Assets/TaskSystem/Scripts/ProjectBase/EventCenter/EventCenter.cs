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
public interface IevnetInfo
{
    
}
public class EventInfo<T> : IevnetInfo
{
    public Action<T> action;
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
    /// <summary>
    /// 移除事件监听
    /// </summary>
    /// <param name="name">事件的名字</param>
    /// <param name="action">准备用来处理事件的委托函数</param>
    public void RemoveEventListener<T>(EventType name, Action<T> action)
    {
        if(eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T>).action -= action;;
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


