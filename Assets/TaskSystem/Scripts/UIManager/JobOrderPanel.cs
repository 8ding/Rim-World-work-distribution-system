using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class JobOrderPanel : MonoBehaviour
{
    public Dictionary<UnitController, OrderListHolder> holders = new Dictionary<UnitController, OrderListHolder>();
    [FormerlySerializedAs("orderHolderTemplate")] [SerializeField]private OrderListHolder orderListHolderTemplate;
    [SerializeField] private Transform orderHolderRoot;

    private void Awake()
    {
        orderListHolderTemplate.gameObject.SetActive(false);
    }

    public void CreateHolder(UnitController _unitController)
    {
        var orderHolder = Instantiate(orderListHolderTemplate, orderHolderRoot);
        orderHolder.gameObject.SetActive(true);
        orderHolder.Bind(_unitController);
        holders.Add(_unitController,orderHolder);
    }

    public void AddWorkerOnPanel(UnitController _unitController)
    {
        OrderListHolder temp;
        if (holders.TryGetValue(_unitController, out temp))
        {
            temp.gameObject.SetActive(true);
            return;
        }
        else
        {
            CreateHolder(_unitController);
        }
    }
    
}
