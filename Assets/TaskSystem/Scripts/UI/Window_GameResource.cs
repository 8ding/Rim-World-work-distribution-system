using System;
using System.Collections;
using System.Collections.Generic;
using TaskSystem.GatherResource;
using UnityEngine;
using UnityEngine.UI;

public class Window_GameResource : MonoBehaviour
{
    // Start is called before the first frame update
    public Text GoldAmount;

    private void Awake()
    {
        GameResource.OnGoldAmountChanged += handldeGoldAmountChanged;
        handldeGoldAmountChanged();
    }

    private void handldeGoldAmountChanged()
    {
        GoldAmount.text = "Gold Amount:" + GameResource.GetGoldAmount();
    }
}
