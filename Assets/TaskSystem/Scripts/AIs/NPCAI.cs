using System.Collections;
using System.Collections.Generic;
using TaskSystem;
using UnityEngine;

public class NPCAI : MonoBehaviour
{
    private Body worker;
    private State state;
    private float waitingTimer;

   
    void Update()
    {
        switch (state)
        {
            case State.WaitingForNextTask:
                //等待请求新任务
                waitingTimer -= Time.deltaTime;
                if (waitingTimer <= 0)
                {
                    float waitingTimerMax = .2f;
                    waitingTimer = waitingTimerMax;
                    RequestNextTask();
                }
                break;
            case State.ExecutingTask:
                break;
        }
    }
    public void RequestNextTask()
    {
        TaskBase task = null;

        if (task == null)
        {
            state = State.WaitingForNextTask;
        }
        else
        {
            state = State.ExecutingTask;
            
        }
    }

    
}
