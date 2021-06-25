using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    private UnitController boundAI;
    private TaskType _m_boundTaskType;
    [SerializeField] public Text orderText;

    public void SetData(UnitController _unitController, TaskType _taskType)
    {
        boundAI = _unitController;
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
