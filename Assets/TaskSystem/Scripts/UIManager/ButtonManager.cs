using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    private WorkerAI boundAI;
    private JobType boundJobType;
    [SerializeField] public Text orderText;

    public void SetData(WorkerAI _workerAi, JobType jobType)
    {
        boundAI = _workerAi;
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
