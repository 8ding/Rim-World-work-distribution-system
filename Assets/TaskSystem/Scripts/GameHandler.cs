using System;
using CodeMonkey;
using CodeMonkey.Utils;
using UnityEngine;

namespace TaskSystem
{
    public class GameHandler : MonoBehaviour
    {
        private PL_TaskSystem taskSystem;
        private Woker woker;
        private void Start()
        {
             taskSystem = new PL_TaskSystem();

            woker = Woker.Create(new Vector3(0, 0));
            WorkerTaskAI workerTaskAI = woker.gameObject.AddComponent<WorkerTaskAI>();
            workerTaskAI.setUp(woker, taskSystem);
            

           /* FunctionTimer.Create(() =>
            {
                CMDebug.TextPopupMouse("Task Added");
                PL_TaskSystem.Task task = new PL_TaskSystem.Task {targetPosition = new Vector3(10, 10)};
                taskSystem.AddTask(task);
            }, 5f);*/
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                // CMDebug.TextPopupMouse("Task Added");
                PL_TaskSystem.Task task = new PL_TaskSystem.Task.MovePosition {targetPosition = (MyClass.GetMouseWorldPosition(woker.gameObject.transform.position.z,Camera.main))};
                taskSystem.AddTask(task);
            }
            if(Input.GetMouseButtonDown(0))
            {
                PL_TaskSystem.Task task = new PL_TaskSystem.Task.Victory();
                taskSystem.AddTask(task);
            }
            if(Input.GetKeyDown(KeyCode.Space))
            {
                GameObject gameObject =  MyClass.CreateWorldSprite(null, "垃圾", GameAssets.Instance.sprite,
                    MyClass.GetMouseWorldPosition(woker.gameObject.transform.position.z, Camera.main), new Vector3(0.1f, 0.1f, 1), 1, Color.white);
                PL_TaskSystem.Task task = new PL_TaskSystem.Task.Clean{rubbish = gameObject.transform};
                taskSystem.AddTask(task);
            }
            
        }
    }
}