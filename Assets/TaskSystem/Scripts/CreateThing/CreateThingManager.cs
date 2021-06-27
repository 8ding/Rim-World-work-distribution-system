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
    /// <param name="amount">数量</param>
    public void  HandlePlacedObject(Vector3 position,PlacedObjectType _placedObjectType,int amount)
    {
        if(positionGameObjectDictionary.ContainsKey(position))
        {
            if(_placedObjectType == PlacedObjectType.none)
            {
                PoolMgr.Instance.PushObj(positionGameObjectDictionary[position]);
                positionGameObjectDictionary.Remove(position);
                return;
            }
            ChangeSpriteWithAmount(positionGameObjectDictionary[position], amount, _placedObjectType);
        }
        else
        {
            PoolMgr.Instance.GetObj(_placedObjectType.ToString() ,(_o =>
            {
                _o.transform.position = position;
                positionGameObjectDictionary[position] = _o;
                //生成了新的堆叠类型并且其枚举大于分界线则证明其为堆叠的物品而不是资源点
                //此时构建搬运任务
                if(_placedObjectType > PlacedObjectType.DividingLine)
                {
                    TaskCenter.Instance.BuildTask(position, GameObject.Find("Crate").transform.position, TaskType.CarryItem);
                }
            }));
        }
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
    
}
