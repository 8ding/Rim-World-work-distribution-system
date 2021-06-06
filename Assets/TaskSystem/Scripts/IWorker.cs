using System;
using System.Collections;
using System.Collections.Generic;
using TaskSystem;
using UnityEngine;

public interface IWorker
{
    void moveTo(Vector3 position, Action onArriveAtPosition = null);
    void Idle();
    void Victory(Action onVictoryEnd = null);

    void CleanUp(Action onCleanEnd = null);

    void Grab(Action OnGrabEnd = null);
    void Mine(Action OnMineEnd = null);

    void Drop(Action OnDropEnd = null);
}
