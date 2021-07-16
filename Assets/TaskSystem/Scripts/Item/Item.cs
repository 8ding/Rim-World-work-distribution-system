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
    [SerializeField]
    private int itemQuantity;
    //持有物品的单位码
    public int UnitCode;

    private bool isInTask = false;
    //物品码
    public const int GoldPoint = 1001;
    public const int WoodPoint = 1002;
    public const int Gold = 1003;
   

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
        if(!isInTask)
        {
            EventCenter.Instance.EventTrigger(EventType.ClickOnItem, this);
        }
    }

    public void SetWhetherInTask(bool res)
    {
        isInTask = res;
    }
    public void SetContent(int _itemCodeParam,int _itemQuantity)
    {
        if (_itemCodeParam != 0)
        {
            ItemCode = _itemCodeParam;
            
            ItemDetails itemDetails = InventoryManager.Instance().GetItemDeatails(ItemCode);
            itemQuantity = _itemQuantity;
            SetWhetherInTask(false);
            Position = gameObject.transform.position;
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

    public bool IsHasContent()
    {
        return itemQuantity > 0;
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
            itemQuantity = 0;
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
