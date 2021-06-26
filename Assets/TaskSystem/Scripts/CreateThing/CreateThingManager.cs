using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateThingManager : BaseManager<CreateThingManager>
{
    public static Dictionary<Vector3, GameHandler.ItemManager> positionItemManagerDictionary;
    public  static void Init()
    {
        EventCenter.Instance.AddEventListener<IArgs>(EventType.CreateUnit,CreateUint);
        EventCenter.Instance.AddEventListener<IArgs>(EventType.CreatMinePoit,createResourcePoint);
        positionItemManagerDictionary = new Dictionary<Vector3, GameHandler.ItemManager>();
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
    private static void createResourcePoint(IArgs _iArgs)
    {

        switch ((_iArgs as EventParameter<Vector3,ResourceType>).t2)
        {
            case ResourceType.Gold:
                PoolMgr.Instance.GetObj("MinePoint",(_o => { 
                    _o .transform.position = (_iArgs as EventParameter<Vector3, ResourceType>).t1;
                    new GameHandler.ResourceManager(_o .transform, (_iArgs as EventParameter<Vector3, ResourceType>).t2);
                }));
                break;
            case ResourceType.Wood:
                PoolMgr.Instance.GetObj("004_tree",(_o => { 
                    _o .transform.position = (_iArgs as EventParameter<Vector3, ResourceType>).t1;
                    new GameHandler.ResourceManager(_o .transform, (_iArgs as EventParameter<Vector3, ResourceType>).t2);
                }));
                break;
        }
    }

    private void  createItem(ItemType _itemType, Action<GameObject> _action)
    {
        
        PoolMgr.Instance.GetObj(_itemType.ToString(),(_o =>
        {
            _action(_o);
        }));
    }
    

    public bool addItemAmount(Vector3 position, ItemType _itemType,ref int amount)
    {
        if(!positionItemManagerDictionary.ContainsKey(position))
        {
            if(amount <= GameHandler.ItemManager.MaxAmount) 
            {
                int temp = amount;
                createItem(_itemType,(_o =>
                {
                    _o.transform.position = position;
                    GameHandler.ItemManager itemManager = new GameHandler.ItemManager(_o.transform, _itemType,temp);
                    positionItemManagerDictionary.Add(position,itemManager);
                    EventCenter.Instance.EventTrigger<IArgs>(EventType.ItemOnGround,new EventParameter<GameHandler.ItemManager>(itemManager));
                }));
                amount = 0;
                return true;
            }
            else
            {
                int temp = GameHandler.ItemManager.MaxAmount;
                createItem(_itemType,(_o =>
                {
                    _o.transform.position = position;
                    GameHandler.ItemManager itemManager = new GameHandler.ItemManager(_o.transform, _itemType,temp);
                    positionItemManagerDictionary.Add(position,itemManager);
                    EventCenter.Instance.EventTrigger<IArgs>(EventType.ItemOnGround,new EventParameter<GameHandler.ItemManager>(itemManager));
                }));
                amount = amount - temp;
                return false;
            }
        }
        else if(positionItemManagerDictionary[position].ItemType != _itemType)
        {
            return false;
        }
        else
        {
            if(amount <= positionItemManagerDictionary[position].GetAmountLeft())
            {
                positionItemManagerDictionary[position].AddContent(amount);
                amount = 0;
                return true;
            }
            else
            {
                int temp = amount;
                positionItemManagerDictionary[position].AddContent(positionItemManagerDictionary[position].GetAmountLeft());
                amount = temp - positionItemManagerDictionary[position].GetAmountLeft();
                return false;
            }
        }
    }
}
