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
    public PlacedObjectType placedObjectType;
    public GameObject handleOjbect;
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
    public int GetMaxCarryAmount()
    {
        return MaxCarryAmount;
    }

    public int GetCarryAmount()
    {
        return carryAmount;
    }
    
    public void AddCarryAmount(int amount,PlacedObjectType _placedObjectType)
    {
        carryAmount += amount;
        placedObjectType = _placedObjectType;
        handleOjbect = CreateThingManager.Instance.ProducePlacedObject(handleOjbect, placedObjectType, carryAmount);
        handleOjbect.transform.SetParent(gameObject.transform);
        handleOjbect.transform.localPosition = Vector3.zero;
    }

    public int GetCarryLeft()
    {
        return MaxCarryAmount - carryAmount;
    }
    public void ClearCarry()
    {
        carryAmount = 0;
        placedObjectType = PlacedObjectType.none;
        handleOjbect = CreateThingManager.Instance.ProducePlacedObject(handleOjbect, placedObjectType, carryAmount);
    }
}
