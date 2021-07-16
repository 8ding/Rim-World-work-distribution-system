using System;
using System.Collections;
using System.Collections.Generic;
using CodeMonkey.Utils;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField]
    private int _itemCode;
    //物品所处状态
    public ItemState itemState;
    //物品位置
    public Vector3 Position;
    private SpriteRenderer spriteRenderer;
    //物品堆积数量
    private int itemQuantity;
    //持有物品的单位码
    public int UnitCode;
   

    public int ItemCode
    {
        get
        {
            return _itemCode;
        }
        set
        {
            _itemCode = value;
        }
    }

    public int ItemQuantity
    {
        get
        {
            return itemQuantity;
        }
        set
        {
            itemQuantity = value;
        }
    }

    private void Awake()
    {
        Physics2D.queriesHitTriggers = true;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }


    private void OnMouseDown()
    {
        Debug.Log("TestClickFunc");
    }

   
    public void SetContent(int _itemCodeParam,int _itemQuantity)
    {
        if (_itemCodeParam != 0)
        {
            ItemCode = _itemCodeParam;
            
            ItemDetails itemDetails = InventoryManager.Instance().GetItemDeatails(ItemCode);
            itemQuantity = _itemQuantity;
            SetContentWithNumber();
            if (itemDetails.itemType == ItemType.Reapable_Scenary)
            {
                
            }
        }
    }
    
    public void SetContentWithNumber()
    {
        ItemDetails itemDetails = InventoryManager.Instance().GetItemDeatails(ItemCode);
        if(itemQuantity == itemDetails.MaxItemQuantity)
        {
            spriteRenderer.sprite = itemDetails.itemSpriteful;
        }
        else
        {
            spriteRenderer.sprite = itemDetails.itemSpritelittle;
        }
    }

    public int AddQuantity(int number)
    {
        int res = 0;
        if(itemQuantity + number <= InventoryManager.Instance().GetItemDeatails(ItemCode).MaxItemQuantity)
        {
            itemQuantity += number;
            res = 0;
        }
        else
        {
            res = number - (InventoryManager.Instance().GetItemDeatails(ItemCode).MaxItemQuantity - itemQuantity);
            itemQuantity = InventoryManager.Instance().GetItemDeatails(ItemCode).MaxItemQuantity;
        }
        SetContentWithNumber();
        return res;
    }

    public int RemoveQuantity(int number)
    {
        int res = 0;
        if(itemQuantity <= number)
        {
            res = number - itemQuantity;
            switch (itemState)
            {
                case ItemState.OnGround:
                    PathManager.Instance.RemoveItemOnGrid(Position);
                    break;
                case ItemState.OnUnit:
                    EventCenter.Instance.EventTrigger(EventType.ItemRemoveFromUnit, UnitCode);
                    break;
            }
            PoolMgr.Instance.PushObj(gameObject);
        }
        else
        {
            res = 0;
            itemQuantity -= number;
            SetContentWithNumber();
        }
        return res;
    }
}
