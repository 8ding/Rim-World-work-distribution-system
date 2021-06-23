using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    private WorkerAI boundAI;
    private TaskType _m_boundTaskType;
    [SerializeField] public Text orderText;

    public void SetData(WorkerAI _workerAi, TaskType _taskType)
    {
        boundAI = _workerAi;
        _m_boundTaskType = _taskType;
    }

    public void AddOrder()
    {
        if (boundAI != null)
        {
            boundAI.ModifyOrder(_m_boundTaskType);
        }
    }
}
