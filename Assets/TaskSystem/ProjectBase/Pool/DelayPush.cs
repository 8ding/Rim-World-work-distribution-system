using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayPush : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        Invoke("Push",1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Push()
    {
        PoolMgr.Instance.PushObj( gameObject);
    }
    
}
