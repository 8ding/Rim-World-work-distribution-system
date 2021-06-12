using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OrderHolder : MonoBehaviour
{
    private WorkGatherTaskAI boundWorkerAI;
    public SettingOfJobType settingOfJobType;
    private static Dictionary<JobType, Text> joTypeTextDictionary;

    private void Awake()
    {
        settingOfJobType = Resources.Load("SettingOfJobTypeOrder") as SettingOfJobType;
        joTypeTextDictionary = new Dictionary<JobType, Text>();
        if (settingOfJobType.JobTypeList != null && joTypeTextDictionary == null)
        {
            joTypeTextDictionary = new Dictionary<JobType, Text>();
            for (int i = 0; i < settingOfJobType.JobTypeList.Count; i++)
            {
                joTypeTextDictionary.Add(settingOfJobType.JobTypeList[i].jobType, GameObject.Find(settingOfJobType.JobTypeList[i].OrderTextName).GetComponent<Text>());
            }
        }
    }

    public void Bind(WorkGatherTaskAI workGatherTaskAI)
    {
        boundWorkerAI = workGatherTaskAI;
        
    }

    private void handleOrderChanged()
    {
        foreach (var jobTypeOrder in boundWorkerAI.jobtypeOrderDictionary)
        {
            Text orderText;
            if (joTypeTextDictionary.TryGetValue(jobTypeOrder.Key, out orderText))
            {
                orderText.text = jobTypeOrder.Value.ToString();
            }
            else
            {
                
            }
        }
        
    }
}
