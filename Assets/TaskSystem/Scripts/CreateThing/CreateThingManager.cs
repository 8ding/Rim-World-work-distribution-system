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
        string name = (_iArgs as EventParameter<string,Vector3,int,float>).t1;
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
    public void  CreatePlacedObject(Vector3 position,PlacedObjectType _placedObjectType,int amount)
    {
        if(positionGameObjectDictionary.ContainsKey(position))
        {
            PoolMgr.Instance.PushObj(positionGameObjectDictionary[position]);
        }
        PoolMgr.Instance.GetObj(_placedObjectType.ToString() ,(_o =>
        {
            _o.transform.position = position;
            ChangeSpriteWithAmount(_o, amount, _placedObjectType);
            positionGameObjectDictionary[position] = _o;
        }));
    }
    /// <summary>
    /// 根据数量变换为应显示的堆叠物体精灵名字，本来应该根据表得来，现在什么都先不做用原版
    /// </summary>
    /// <param name="amount">堆叠数量</param>
    /// <param name="_placedObjectType">堆叠物体类型</param>
    /// <returns></returns>
    private void ChangeSpriteWithAmount(GameObject _gameObject,int amount, PlacedObjectType _placedObjectType)
    {
        _gameObject.GetComponent<SpriteRenderer>().sprite = ResMgr.Instance.Load<Sprite>(_placedObjectType.ToString());
    }

    private void  createItem(PlacedObjectType _placedObjectType, Action<GameObject> _action)
    {
        
        PoolMgr.Instance.GetObj(_placedObjectType.ToString(),(_o =>
        {
            _action(_o);
        }));
    }
    
    
}
