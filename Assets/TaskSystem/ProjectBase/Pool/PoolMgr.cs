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

    public GameObject GetObj()
    {
        
        GameObject obj = null;
        //抽屉里有衣服，直接拿走
        if(poolList.Count > 0)
        {
            obj = poolList[0];
            obj.transform.SetParent(null);
            poolList.RemoveAt(0);
        }
        //抽屉里没衣服直接按照抽屉的衣架标签上的商场地址去买一套
        else
        {
            obj = GameObject.Instantiate(Resources.Load<GameObject>(fatherObj.name));
            //也给衣服贴上商场标签和衣服名字
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
    public GameObject GetObj(string name)
    {
        GameObject obj = null;
        //没有抽屉
        if(!poolDic.ContainsKey(name))
        {
            //根据地址和名字去商场买一件衣服，等放的时候再造抽屉
            obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
            //在衣服上贴上商场、名字的标签
            obj.name = name;
        }
        //已经有抽屉了，从抽屉里拿出衣服
        else
        {
            obj = poolDic[name].GetObj();
        }
        //穿上衣服
        obj.SetActive(true);
        return obj;
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
