using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitData : MonoBehaviour
{
    private int characterId;
    private float speed;
    private Item item;


    public GameHandler.ItemManager itemManager;

    private void Awake()
    {
        itemManager = new GameHandler.ItemManager();
    
    }

    private void OnEnable()
    {
        EventCenter.Instance.AddEventListener<IArgs>(EventType.ItemRemoveFromUnit,RemovItemOnUnit);
    }

    private void OnDisable()
    {
        EventCenter.Instance.RemoveEventListener<IArgs>(EventType.ItemRemoveFromUnit,RemovItemOnUnit);
    }

    public int CharacterId
    {
        get
        {
            return characterId;
        }
        set
        {
            characterId = value;
        }
    }
    public float Speed
    {
        get
        {
            return speed;
        }
        set
        {
            speed = value;
        }
    }

    public void SetItemOnUnit(Item _item)
    {
        item = _item;
        _item.UnitCode = characterId;
        _item.itemState = ItemState.OnUnit;
    }

    public Item GetItemOnUnit()
    {
        return item;
    }

    public void RemovItemOnUnit(IArgs _iArgs)
    {
        if((_iArgs as EventParameter<int>).t == characterId)
        {
            item = null;
        }
    }
//    public int GetMaxCarryAmount()
//    {
//        return MaxCarryAmount;
//    }
//
//    public int GetCarryAmount()
//    {
//        return carryAmount;
//    }
//    
//    public void AddCarryAmount(int amount,ItemType _itemType)
//    {
//        carryAmount += amount;
//        if(itemManager != null)
//    }
//
//    public int GetCarryLeft()
//    {
//        return MaxCarryAmount - carryAmount;
//    }
//    public void ClearCarry()
//    {
//        carryAmount = 0;
//        placedObjectType = PlacedObjectType.none;
//        handleOjbect = CreateThingManager.Instance.ProducePlacedObject(handleOjbect, placedObjectType, carryAmount);
//    }
}
