using System;
using System.Collections.Generic;
using UnityEngine;


public  class AIBase : MonoBehaviour
{
    protected Body worker;
    protected int characterId;

    public void setUp(Body worker)
    {
        this.worker = worker;
        characterId = worker.GetId();
        worker.Idle();
    }

    public int GetId()
    {
        return characterId;
    }
}