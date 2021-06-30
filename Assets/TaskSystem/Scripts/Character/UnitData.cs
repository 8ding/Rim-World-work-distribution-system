using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitData : MonoBehaviour
{
    private int characterId;
    private float speed;
    private const int MaxCarryAmount = 3;
    private int carryAmount = 0;
    public GameHandler.ItemManager itemManager;

    private void Awake()
    {
        itemManager = new GameHandler.ItemManager();
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
