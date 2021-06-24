using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonoControler : MonoBehaviour
{
    // Start is called before the first frame update
    public event UnityAction updateEvent;
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {    
        updateEvent?.Invoke();
    }
    /// <summary>
    /// 给外部提供的添加帧更新的函数
    /// </summary>
    /// <param name="fun"></param>
    public void AddUpdateListener(UnityAction fun)
    {
        updateEvent += fun;
    }

    public void RemoveUpdateListener(UnityAction fun)
    {
        updateEvent -= fun;
    }
}
