using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class JobOrderPanel : MonoBehaviour
{
    public Dictionary<WorkGatherTaskAI, OrderListHolder> holders = new Dictionary<WorkGatherTaskAI, OrderListHolder>();
    [FormerlySerializedAs("orderHolderTemplate")] [SerializeField]private OrderListHolder orderListHolderTemplate;
    [SerializeField] private Transform orderHolderRoot;

    private void Awake()
    {
        orderListHolderTemplate.gameObject.SetActive(false);
    }

    public void CreateHolder(WorkGatherTaskAI workerAI)
    {
        var orderHolder = Instantiate(orderListHolderTemplate, orderHolderRoot);
        orderHolder.gameObject.SetActive(true);
        orderHolder.Bind(workerAI);
        holders.Add(workerAI,orderHolder);
    }

    public void AddWorkerOnPanel(WorkGatherTaskAI workGatherTaskAI)
    {
        OrderListHolder temp;
        if (holders.TryGetValue(workGatherTaskAI, out temp))
        {
            return;
        }
        else
        {
            CreateHolder(workGatherTaskAI);
        }
    }
}
