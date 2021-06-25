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

    public void AddCarryAomunt()
    {
        carryAmount++;
    }

    public void AddCarryAmount(int amount)
    {
        carryAmount += amount;
    }

    public void ClearCarry()
    {
        carryAmount = 0;
    }
}
