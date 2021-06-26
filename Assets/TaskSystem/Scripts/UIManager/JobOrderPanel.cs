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
        EventCenter.Instance.AddEventListener<IArgs>(EventType.UnitOccur, AddWorkerOnPanel);
    }

    public void CreateHolder(UnitController _unitController)
    {
        var orderHolder = Instantiate(orderListHolderTemplate, orderHolderRoot);
        orderHolder.gameObject.SetActive(true);
        orderHolder.Bind(_unitController);
        holders.Add(_unitController,orderHolder);
    }

    public void AddWorkerOnPanel(IArgs _iArgs)
    {
        
        OrderListHolder temp;
        if (holders.TryGetValue((_iArgs as EventParameter<UnitController>).t, out temp))
        {
            temp.gameObject.SetActive(true);
            return;
        }
        else
        {
            CreateHolder((_iArgs as EventParameter<UnitController>).t);
        }
    }

    private void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener<IArgs>(EventType.UnitOccur,AddWorkerOnPanel);
    }
}
