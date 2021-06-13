using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OrderListHolder : MonoBehaviour
{
    private WorkGatherTaskAI boundWorkerAI;
    
    private  Dictionary<JobType, ButtonManager> joTypeButtonDictionary;
    [SerializeField] private ButtonManager buttonTemplate;
    [SerializeField] private Transform buttonRoot;

    private void Awake()
    {
        joTypeButtonDictionary = new Dictionary<JobType, ButtonManager>();
        buttonTemplate.gameObject.SetActive(false);
    }

    public void Bind(WorkGatherTaskAI workGatherTaskAI)
    {

        boundWorkerAI = workGatherTaskAI;
        boundWorkerAI.OnJobOrderChanged += handleOrderChanged;
        ButtonManager temp;
        for (int i = 0; i < (int)JobType.enumcount; i++)
        {
            if (joTypeButtonDictionary.TryGetValue((JobType)i, out temp))
            {
                break;
            }
            else
            {
                createButton((JobType)i,workGatherTaskAI);
            }
        }
        handleOrderChanged();
    }

    private void createButton(JobType jobType,WorkGatherTaskAI workGatherTaskAI)
    {
        var newbutton = Instantiate(buttonTemplate, buttonRoot);
        newbutton.gameObject.SetActive(true);
        newbutton.SetData(workGatherTaskAI, jobType);
        joTypeButtonDictionary.Add(jobType, newbutton);
    }
    private void handleOrderChanged()
    {
        foreach (var jobTypeOrder in boundWorkerAI.jobtypeOrderDictionary)
        {
            ButtonManager orderButton;
            if (joTypeButtonDictionary.TryGetValue(jobTypeOrder.Key, out orderButton))
            {
                orderButton.orderText.text = jobTypeOrder.Value.ToString();
            }
            else
            {
                Debug.Log( jobTypeOrder.Key + "类型未添加UIText到列表");
            }
        }
        
    }

}
