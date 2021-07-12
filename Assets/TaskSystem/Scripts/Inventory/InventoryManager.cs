
using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : SingletonAutoMono<InventoryManager>
{
    private Dictionary<int, ItemDetails> itemDeatailsDictionary;
    [SerializeField] private SO_ItemList itemList = null;
    
    protected override void Awake()
    {
        base.Awake();
        itemList = ResMgr.Instance.Load<SO_ItemList>("SO/so_ItemList");
        CreateItemDeatailsDictionary();
    }
    
    /// <summary>
    /// 使用Scriptable objec 填充字典
    /// </summary>
    private void CreateItemDeatailsDictionary()
    {
        if(itemDeatailsDictionary == null)
        {
            itemDeatailsDictionary = new Dictionary<int, ItemDetails>();

            foreach (var itemDeatails in itemList.itemDetailsList)
            {
                itemDeatailsDictionary.Add(itemDeatails.itemCode, itemDeatails);
            }
        }
    }

    public Item CreateItem(int _ItemCode, int _ItemCount)
    {
        GameObject gameObject = PoolMgr.Instance.GetObj("Item");
        Item item = gameObject.GetComponent<Item>();
        item.SetContent(_ItemCode, _ItemCount);
        return item;
    }
    public ItemDetails GetItemDeatails(int itemCode)
    {
        ItemDetails itemDetails;
        if(itemDeatailsDictionary.TryGetValue(itemCode,out itemDetails))
        {
            return itemDetails;
        }
        return null;
    }
    
}
