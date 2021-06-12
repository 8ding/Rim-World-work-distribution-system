using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OrderListHolder : MonoBehaviour
{
    private WorkGatherTaskAI boundWorkerAI;
    private SettingOfJobType settingOfJobType;
    private  Dictionary<JobType, ButtonManager> joTypeButtonDictionary;
    [SerializeField] private ButtonManager buttonTemplate;
    [SerializeField] private Transform buttonRoot;

    private void Awake()
    {
        Debug.Log(gameObject.name);
        settingOfJobType = Resources.Load("SettingOfJobTypeOrder") as SettingOfJobType;
        joTypeButtonDictionary = new Dictionary<JobType, ButtonManager>();
        buttonTemplate.gameObject.SetActive(false);
    }

    public void Bind(WorkGatherTaskAI workGatherTaskAI)
    {
        Debug.Log(gameObject.name);
        boundWorkerAI = workGatherTaskAI;
        boundWorkerAI.OnJobOrderChanged += handleOrderChanged;
        ButtonManager temp;
        for (int i = 0; i < settingOfJobType.JobTypeList.Count; i++)
        {
            if (joTypeButtonDictionary.TryGetValue(settingOfJobType.JobTypeList[i].jobType, out temp))
            {
                break;
            }
            else
            {
                createButton(settingOfJobType.JobTypeList[i].jobType,workGatherTaskAI);
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
