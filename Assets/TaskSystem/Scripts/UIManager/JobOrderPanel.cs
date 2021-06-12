using System;
using System.Collections.Generic;
using UnityEngine;


public class JobOrderPanel : MonoBehaviour
{
    public Dictionary<WorkGatherTaskAI, OrderHolder> holders = new Dictionary<WorkGatherTaskAI, OrderHolder>();
    [SerializeField]private OrderHolder orderHolderTemplate;
    [SerializeField] private Transform orderHolderRoot;

    private void Awake()
    {
        orderHolderTemplate.gameObject.SetActive(false);
    }

    public void CreateHolder(WorkGatherTaskAI workerAI)
    {
        var orderHolder = Instantiate(orderHolderTemplate, orderHolderRoot);
        orderHolder.GetComponent<OrderHolder>().Bind(workerAI);
    }

    public void AddWorkerOnPanel(WorkGatherTaskAI workGatherTaskAI)
    {
        OrderHolder temp;
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
