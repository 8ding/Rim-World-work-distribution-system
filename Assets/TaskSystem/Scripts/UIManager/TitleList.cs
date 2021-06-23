using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleList : MonoBehaviour
{
    [SerializeField] private Text textTemplate;
    [SerializeField] private Transform TitleRoot;

    private void Awake()
    {
        textTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        for (int i = 0; i < (int) TaskType.enumcount; i++)
        {
            CreateTitleText((TaskType)i);
        }
    }

    private void CreateTitleText(TaskType _taskType)
    {
        var title = Instantiate(textTemplate, TitleRoot);
        title.gameObject.SetActive(true);
        title.text = _taskType.ToString();
    }
}
