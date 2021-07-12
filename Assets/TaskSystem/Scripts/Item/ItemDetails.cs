using System;
using UnityEngine;

[System.Serializable]
public class ItemDetails 
{
    public int itemCode;
    public ItemType itemType;
    public string itemDescription;
    public Sprite itemSpritelittle;
    public Sprite itemSpriteful;
    public int MaxItemQuantity;
    public int productItemCode;
    public string itemLongDescription;
    public short itemUseGridRadius;
    public float itemUseRadius;
    public bool isStartingItem;
    public bool canBePickedUp;
    public bool canBeDropped;
    public bool canBeEaten;
    public bool canBeCarried;
}