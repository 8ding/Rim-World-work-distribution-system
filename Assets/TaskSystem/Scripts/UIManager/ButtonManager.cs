using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    private WorkGatherTaskAI boundAI;
    private JobType boundJobType;
    [SerializeField] public Text orderText;

    public void SetData(WorkGatherTaskAI workGatherTaskAI, JobType jobType)
    {
        boundAI = workGatherTaskAI;
        boundJobType = jobType;
    }

    public void AddOrder()
    {
        if (boundAI != null)
        {
            boundAI.ModifyOrder(boundJobType);
        }
    }
}
