using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
///  提供外部添加帧更新的函数
/// </summary>
public class MonoMgr : BaseManager<MonoMgr>
{
    private MonoControler controler;

    public MonoMgr()
    {
        GameObject obj = new GameObject("MonoController");
        controler = obj.AddComponent<MonoControler>();
    }
    /// <summary>
    /// 给外部提供的添加帧更新的函数
    /// </summary>
    /// <param name="fun"></param>
    public void AddUpdateListener(UnityAction fun)
    {
        controler.updateEvent += fun;
       
    }

    public void RemoveUpdateListener(UnityAction fun)
    {
        controler.updateEvent -= fun;
    }

    public Coroutine StartCoroutine(IEnumerator routine)
    {
        return controler.StartCoroutine(routine);
    }
}
