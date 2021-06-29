using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 池子一列容器
/// </summary>
public class PoolData
{
    public GameObject fatherObj;
    public List<GameObject> poolList;

    public PoolData(string name, GameObject poolObj)
    {
        //衣架的名字也贴个标签跟衣服的名字一样
        fatherObj = new GameObject(name);
        //衣架固定在衣柜上
        fatherObj.transform.SetParent(poolObj.transform);
        //打造衣柜抽屉空间
        poolList = new List<GameObject>(){};
    }

    public void PushObj(GameObject obj)
    {
        poolList.Add(obj);
        obj.transform.SetParent(fatherObj.transform);
    }

    public void GetObjAsyn(Action<GameObject> callback)
    {
        
        GameObject obj = null;
        //抽屉里有衣服，直接拿走
        if(poolList.Count > 0)
        {
            obj = poolList[0];
            obj.transform.SetParent(null);
            poolList.RemoveAt(0);
            callback?.Invoke(obj);
        }
        //抽屉里没衣服直接按照抽屉的衣架标签上的商场地址去买一套
        else
        {
            ResMgr.Instance.LoadAsync<GameObject>(fatherObj.name,(o =>
            {
                o.name = fatherObj.name;
                callback?.Invoke(o);
            }));;
        }
    }

    public GameObject GetObj()
    {
        GameObject obj = null;
        if(poolList.Count > 0)
        {
            obj = poolList[0];
            obj.transform.SetParent(null);
            poolList.RemoveAt(0);
        }
        else
        {
            obj = ResMgr.Instance.Load<GameObject>(fatherObj.name);
            obj.name = fatherObj.name;
        }
        return obj;
    }
}
/// <summary>
/// 缓存池模块
/// </summary>
public class PoolMgr:BaseManager<PoolMgr>
{
    private GameObject poolObj;
    //缓存池容器
    public Dictionary<string,PoolData> poolDic = new Dictionary<string, PoolData>();
    
    //取对象
    public void GetObjAsync(string name,Action<GameObject> callback)
    {
        //没有抽屉
        if(!poolDic.ContainsKey(name))
        {
            //根据地址和名字去商场买一件衣服，等放的时候再造抽屉
            ResMgr.Instance.LoadAsync<GameObject>(name,(o => {  
               o.name = name;
               callback?.Invoke(o);
           }));
           
        }
        //已经有抽屉了，从抽屉里拿出衣服
        else
        {
             poolDic[name].GetObjAsyn((o =>
             {
                 o.SetActive(true);
                 callback?.Invoke(o);
             }));

        }
    }

    public GameObject GetObj(string name)
    {
        GameObject res;
        if(!poolDic.ContainsKey(name))
        {
            res = ResMgr.Instance.Load<GameObject>(name);
            res.name = name;
        }
        else
        {
           res = poolDic[name].GetObj();
        }
        res.SetActive(true);
        return res;
    }
    //还对象
    public void PushObj(GameObject obj)
    {
        if(obj != null)
        {
            //衣架安置点还没有，就先造一个
            if(poolObj == null)
            {
                poolObj = new GameObject("Pool");
            }
            //根据衣服标签地址名称找抽屉，如果还没有抽屉
            if(!poolDic.ContainsKey(obj.name))
            {
                //造一个抽屉
                poolDic[obj.name] = new PoolData(obj.name,poolObj);
            }
            //脱下衣服
            obj.SetActive(false);
            //衣服放入抽屉
            poolDic[obj.name].PushObj(obj);
        }
    }

    public void Clear()
    {
        poolDic.Clear();
        poolObj = null;
    }
}
