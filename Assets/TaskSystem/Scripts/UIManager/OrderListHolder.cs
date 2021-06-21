using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OrderListHolder : MonoBehaviour
{
    private WorkerAI boundWorkerAI;
    
    private  Dictionary<JobType, ButtonManager> joTypeButtonDictionary;
    [SerializeField] private ButtonManager buttonTemplate;
    [SerializeField] private Transform buttonRoot;

    private void Awake()
    {
        joTypeButtonDictionary = new Dictionary<JobType, ButtonManager>();
        buttonTemplate.gameObject.SetActive(false);
    }

    public void Bind(WorkerAI _workerAi)
    {

        boundWorkerAI = _workerAi;
        boundWorkerAI.OnJobOrderChanged += handleOrderChanged;
        boundWorkerAI.OnNotWorker += cancelOnPanel;
        ButtonManager temp;
        for (int i = 0; i < (int)JobType.enumcount; i++)
        {
            if (joTypeButtonDictionary.TryGetValue((JobType)i, out temp))
            {
                break;
            }
            else
            {
                createButton((JobType)i,_workerAi);
            }
        }
        handleOrderChanged();
    }

    private void createButton(JobType jobType,WorkerAI _workerAi)
    {
        var newbutton = Instantiate(buttonTemplate, buttonRoot);
        newbutton.gameObject.SetActive(true);
        newbutton.SetData(_workerAi, jobType);
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

    private void cancelOnPanel()
    {
        gameObject.SetActive(false);
    }

}
