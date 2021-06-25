using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OrderListHolder : MonoBehaviour
{
    private UnitController _m_boundUnitController;
    
    private  Dictionary<TaskType, ButtonManager> joTypeButtonDictionary;
    [SerializeField] private ButtonManager buttonTemplate;
    [SerializeField] private Transform buttonRoot;

    private void Awake()
    {
        joTypeButtonDictionary = new Dictionary<TaskType, ButtonManager>();
        buttonTemplate.gameObject.SetActive(false);
    }

    public void Bind(UnitController _unitController)
    {

        _m_boundUnitController = _unitController;
        _m_boundUnitController.OnJobOrderChanged += handleOrderChanged;
        ButtonManager temp;
        for (int i = 0; i < (int)TaskType.enumcount; i++)
        {
            if (joTypeButtonDictionary.TryGetValue((TaskType)i, out temp))
            {
                break;
            }
            else
            {
                createButton((TaskType)i,_unitController);
            }
        }
        handleOrderChanged();
    }

    private void createButton(TaskType _taskType,UnitController _unitController)
    {
        var newbutton = Instantiate(buttonTemplate, buttonRoot);
        newbutton.gameObject.SetActive(true);
        newbutton.SetData(_unitController, _taskType);
        joTypeButtonDictionary.Add(_taskType, newbutton);
    }
    private void handleOrderChanged()
    {
        foreach (var jobTypeOrder in _m_boundUnitController.jobtypeOrderDictionary)
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
