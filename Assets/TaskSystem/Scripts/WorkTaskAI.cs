using System;
using System.Collections;
using System.Collections.Generic;
using CodeMonkey;
using UnityEngine;

public class WorkerTaskAI : MonoBehaviour
{
    private enum State
    {
        WaitingForNextTask,
        ExecutingTask,
    }
    private IWorker worker;
    private State state;
    private float waitingTimer;
    
    public void setUp(IWorker worker)
    {
        this.worker = worker;
        state = State.WaitingForNextTask;
    }

    private void Update()
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
                
        }
    }

    private void RequestNextTask()
    {
        //方便的Debug方式，留着后面看怎么实现的
        CMDebug.TextPopupMouse("RequestNextTask");
    }
}
