using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCenter : BaseManager<EventCenter>
{
    //key-事件的名字（比如怪物死亡)
    //value - 对应的是监听这个事件对应的委托函数
    private Dictionary<string, Action<object>>eventDic  = new Dictionary<string, Action<object>>();
    
    /// <summary>
    /// 添加事件监听
    /// </summary>
    /// <param name="name">事件的名字</param>
    /// <param name="action">准备用来处理事件的委托函数</param>
    public void AddEventListener(string name, Action<object> action)
    {
        if(eventDic.ContainsKey(name))
        {
            eventDic[name] += action;
        }
        else
        {
            eventDic.Add(name,action);
        }
    }

    /// <summary>
    /// 事件触发
    /// </summary>
    /// <param name="name">触发事件的名字</param>
    public void EventTrigger(string name, object objectInfo)
    {
        if(eventDic.ContainsKey(name))
        {
            eventDic[name]?.Invoke(objectInfo);
        }
    }
    /// <summary>
    /// 移除事件监听
    /// </summary>
    /// <param name="name">事件的名字</param>
    /// <param name="action">准备用来处理事件的委托函数</param>
    public void RemoveEventListener(string name, Action<object> action)
    {
        if(eventDic.ContainsKey(name))
        {
            eventDic[name] -= action;
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
