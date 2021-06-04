using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWorker
{
    void moveTo(Vector3 position, Action onArriveAtPosition = null);
    void Idle();
    void Victory(Action onVictoryEnd);
}
