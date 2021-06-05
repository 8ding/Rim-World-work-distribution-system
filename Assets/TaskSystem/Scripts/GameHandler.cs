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
                GameObject gameObject =  MyClass.CreateWorldSprite(null, "垃圾", "Environment",GameAssets.Instance.sprite,
                    MyClass.GetMouseWorldPosition(woker.gameObject.transform.position.z, Camera.main), new Vector3(0.1f, 0.1f, 1), 1, Color.white);
                SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                //任务构造函数，传入事件参数，事件参数包含行为是调用FunctionUpdater.Create,这个方法是每帧调用一次它的事件参数，而传入的事件执行的是给sprite的alpha值减少一个帧的时间量。
                PL_TaskSystem.Task task = new PL_TaskSystem.Task.Clean(gameObject.transform.position, (() =>
                {
                    float alpha = 1f;
                    FunctionUpdater.Create((() =>
                    {
                        alpha -= Time.deltaTime;
                        spriteRenderer.color = new Color(1f, 1, 1, alpha);
                        if (alpha <= 0f)
                            return true;
                        else
                        {
                            return false;
                        }
                    }));
                }));
                taskSystem.AddTask(task);
            }
            
        }
    }
}