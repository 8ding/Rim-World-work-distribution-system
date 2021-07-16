using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitData : MonoBehaviour
{
    private int characterId;
    private float speed;
    private Item item;

    private void OnEnable()
    {
        EventCenter.Instance.AddEventListener<int>(EventType.ItemRemoveFromUnit,RemovItemOnUnit);
    }

    private void OnDisable()
    {
        EventCenter.Instance.RemoveEventListener<int>(EventType.ItemRemoveFromUnit,RemovItemOnUnit);
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

    public Item Item
    {
        get
        {
            return item;
        }
        set
        {
            item = value;
        }
    }

    public void SetItemOnUnit(Item _item)
    {
        item = _item;
        _item.UnitCode = characterId;
        _item.itemState = ItemState.OnUnit;
        _item.gameObject.transform.SetParent(this.gameObject.transform);
        _item.gameObject.transform.localPosition = Vector3.zero;
    }

    public Item GetItemOnUnit()
    {
        return item;
    }

    public void RemovItemOnUnit(int unitCode) 
    {
        if(unitCode == characterId)
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
