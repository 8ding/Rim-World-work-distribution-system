using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum JobType
{
    GatherGold,
    GatherWood,
}

[Serializable]
public struct TestType
{
    public string name;
    public Action TextAction;
}
[Serializable]
public struct JobType_OrderTextName
{
    public JobType jobType;
    public string OrderTextName;
}
[CreateAssetMenu(menuName="MySubMenue/Create SettingOfJobType ")]
public class SettingOfJobType : ScriptableObject
{
    public List<JobType_OrderTextName> JobTypeList = new List<JobType_OrderTextName>();
    public TestType testType;
}
