using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    private ItemType itemType;
    public ItemType ItemType { get { return itemType; } set { itemType = value; } }
    private int pileUpNumber;
    public int PileUpNumber { get { return pileUpNumber; } set { pileUpNumber = value; } }
}
