using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Setting 
{
    //物品名字与物品码对应表
    public static Dictionary<string, int> itemNameCodeDictionary = new Dictionary<string, int>
    {
        {"GoldPoint", 1001},
        {"WoodPoint", 1002},
        {"Gold",1003}
    };

    public static int GetItemCodeWithName(string name)
    {
        if(itemNameCodeDictionary.ContainsKey(name))
        {
            return itemNameCodeDictionary[name];
        }
        else
        {
            return 0;
        }
    }
}
