using System;
using UnityEngine;

namespace TaskSystem
{
    public class GameHandler : MonoBehaviour
    {
        private void Start()
        {
            PL_TaskSystem taskSystem = new PL_TaskSystem();

            Woker woker = Woker.Create(new Vector3(0, 0));
            WorkerTaskAI workerTaskAI = woker.gameObject.AddComponent<WorkerTaskAI>();
            workerTaskAI.setUp(woker);
        }
    }
}