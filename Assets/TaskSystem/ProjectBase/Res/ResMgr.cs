using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// 资源加载模块
/// </summary>
public class ResMgr : BaseManager<ResMgr>
{
    /// <summary>
    /// 同步加载资源
    /// </summary>
    /// <param name="name"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T Load<T>(string name) where T:Object
    {
        T res = Resources.Load<T>(name);
        //如果对象是GameObject,实例化后返回
        if(res is GameObject)
            return GameObject.Instantiate(res);
        else
            return res;
    }
    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <param name="name">资源路径名称</param>
    /// <param name="callBack">加载资源完成后的回调函数</param>
    /// <typeparam name="T"></typeparam>
    public void LoadAsync<T>(string name,Action<T> callBack) where T : Object
    {
        MonoMgr.Instance.StartCoroutine(realLoadAsync<T>(name,callBack));
    }

    private IEnumerator realLoadAsync<T>(string name,Action<T> callBack)where T : Object
    {
        ResourceRequest resourceRequest = Resources.LoadAsync<T>(name);
        yield return resourceRequest;
        if(resourceRequest.asset is GameObject)
            callBack?.Invoke(GameObject.Instantiate(resourceRequest.asset) as T);
        else
        {
            callBack?.Invoke(resourceRequest.asset as T);
        }
        
    }
}
