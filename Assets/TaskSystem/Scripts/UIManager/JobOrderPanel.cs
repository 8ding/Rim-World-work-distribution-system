using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class JobOrderPanel : MonoBehaviour
{
    public Dictionary<WorkerAI, OrderListHolder> holders = new Dictionary<WorkerAI, OrderListHolder>();
    [FormerlySerializedAs("orderHolderTemplate")] [SerializeField]private OrderListHolder orderListHolderTemplate;
    [SerializeField] private Transform orderHolderRoot;

    private void Awake()
    {
        orderListHolderTemplate.gameObject.SetActive(false);
    }

    public void CreateHolder(WorkerAI workerAI)
    {
        var orderHolder = Instantiate(orderListHolderTemplate, orderHolderRoot);
        orderHolder.gameObject.SetActive(true);
        orderHolder.Bind(workerAI);
        holders.Add(workerAI,orderHolder);
    }

    public void AddWorkerOnPanel(WorkerAI _workerAi)
    {
        OrderListHolder temp;
        if (holders.TryGetValue(_workerAi, out temp))
        {
            temp.gameObject.SetActive(true);
            return;
        }
        else
        {
            CreateHolder(_workerAi);
        }
    }
    
}
