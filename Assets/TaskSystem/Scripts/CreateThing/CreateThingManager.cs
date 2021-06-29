using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateThingManager : BaseManager<CreateThingManager>
{
    public static Dictionary<Vector3, GameObject> positionGameObjectDictionary;
    public  static void Init()
    {
        EventCenter.Instance.AddEventListener<IArgs>(EventType.CreateUnit,CreateUint);
        positionGameObjectDictionary = new Dictionary<Vector3, GameObject>();
    }

    public static void CreateUint(IArgs _iArgs)
    {
        string name = (_iArgs as EventParameter<string,Vector3,int,float>)?.t1;
        Vector3 position = (_iArgs as EventParameter<string, Vector3, int, float>).t2;
        int ID = (_iArgs as EventParameter<string, Vector3, int, float>).t3;
        float Speed = (_iArgs as EventParameter<string, Vector3, int, float>).t4;
        GameObject unit = ResMgr.Instance.Load<GameObject>(name);
        unit.transform.position = position;
        UnitData unitData =  unit.AddComponent<UnitData>();
        unitData.CharacterId = ID;
        unitData.Speed = Speed;

        unit.AddComponent<MoveTransformVelocity>();
        unit.AddComponent<MovePositionPathFinding>();

        UnitController unitController = unit.AddComponent<UnitController>();
        EventCenter.Instance.EventTrigger<IArgs>(EventType.UnitOccur,new EventParameter<UnitController>(unitController));
    }
    /// <summary>
    /// 处理指定位置的堆叠类型,其显示由其堆叠数量决定
    /// </summary>
    /// <param name="position">位置</param>
    /// <param name="_placedObjectType">堆叠物体类型</param>
    /// <param name="_amount">数量</param>
    public GameObject  ProducePlacedObject(GameObject _product,PlacedObjectType _placedObjectType,int _amount)
    {

        if(_amount == 0)
        {
            PoolMgr.Instance.PushObj(_product);
            return null;
        }
        else
        {
            if(_product == null)
            {
                _product = PoolMgr.Instance.GetObj(_placedObjectType.ToString());
            }
            ChangeSpriteWithAmount(_product, _amount, _placedObjectType);
        }
        return _product;
    }
    /// <summary>
    /// 根据数量变换为应显示的堆叠物体精灵名字，本来应该根据表得来，现在什么都先不做用原版
    /// </summary>
    /// <param name="_amount">堆叠数量</param>
    /// <param name="_placedObjectType">堆叠物体类型</param>
    /// <returns></returns>
    public void ChangeSpriteWithAmount(GameObject _gameObject,int _amount, PlacedObjectType _placedObjectType)
    {
        _gameObject.GetComponent<SpriteRenderer>().sprite = ResMgr.Instance.Load<Sprite>(_placedObjectType.ToString());
    }
    
}
