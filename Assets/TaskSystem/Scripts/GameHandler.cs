using System;
using System.Collections.Generic;
using CodeMonkey;
using CodeMonkey.Utils;
using StateMachine;
using UnityEngine;

namespace TaskSystem
{
    public class GameHandler : MonoBehaviour
    {
        private PL_TaskSystem<Task> taskSystem;
        private static PL_TaskSystem<TransportTask> transportTaskSystem;
        private Woker woker;
        private WeaponSlotManager weaponSlotManager;
        private GameObject weaponSlotGameObject;
        private List<WeaponSlotManager> weaponSlotManagerList;
        private void Start()
        {
             taskSystem = new PL_TaskSystem<Task>();
             transportTaskSystem = new PL_TaskSystem<TransportTask>();
             weaponSlotManagerList = new List<WeaponSlotManager>();
             //Woker.Create创造了一个新的Woker对象,在setUp里面TaskAI绑定了这个实例对象，因此是两个实例对象，也绑定了两个实例对象
            woker = Woker.Create(new Vector3(0, 0));
            WorkerTaskAI workerTaskAI = woker.gameObject.AddComponent<WorkerTaskAI>();
            workerTaskAI.setUp(woker, taskSystem);

            woker = Woker.Create(new Vector3(5, 0));
            WorkTransportTaskAI workTransportTaskAI = woker.gameObject.AddComponent<WorkTransportTaskAI>();
            workTransportTaskAI.setUp(woker,transportTaskSystem);
            

            
            weaponSlotGameObject = MyClass.CreateWorldSprite(null, "武器存储", "Environment",
                GameAssets.Instance.WeaponSlot,
                new Vector3(-5,0,0),
                new Vector3(1, 1, 1), 0, Color.white);
            weaponSlotManagerList.Add(new WeaponSlotManager(weaponSlotGameObject.transform));
            
            weaponSlotGameObject = MyClass.CreateWorldSprite(null, "武器存储", "Environment",
                GameAssets.Instance.WeaponSlot,
                new Vector3(-5,5,0),
                new Vector3(1, 1, 1), 0, Color.white);
            weaponSlotManagerList.Add(new WeaponSlotManager(weaponSlotGameObject.transform));

            // FunctionTimer.Create((() => weaponSlotManager.setWeaponTransform(weaponGameObject.transform)), 2f);

            // taskSystem.AddTask(task);
            
            // woker = Woker.Create(new Vector3(5, 5f));
            // workerTaskAI = woker.gameObject.AddComponent<WorkerTaskAI>();
            // workerTaskAI.setUp(woker,taskSystem);

            /* FunctionTimer.Create(() =>
             {
                 CMDebug.TextPopupMouse("Task Added");
                 PL_TaskSystem.Task task = new PL_TaskSystem.Task {targetPosition = new Vector3(10, 10)};
                 taskSystem.AddTask(task);
             }, 5f);*/
            Task.MovePosition movePosition = new Task.MovePosition
            {
                targetPosition = new Vector3(0, 0, 0f)
            };
            taskSystem.AddTask(movePosition);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                // CMDebug.TextPopupMouse("Task Added");
                Task.MovePosition task = new  Task.MovePosition {targetPosition = (MyClass.GetMouseWorldPosition(woker.gameObject.transform.position.z,Camera.main))};
                taskSystem.AddTask(task);
            }
            if(Input.GetMouseButtonDown(0))
            {
                weaponSlotGameObject = MyClass.CreateWorldSprite(null, "武器存储", "Environment",
                    GameAssets.Instance.WeaponSlot,
                    MyClass.GetMouseWorldPosition(woker.gameObject.transform.position.z,Camera.main),
                    new Vector3(1, 1, 1), 0, Color.white);
                weaponSlotManagerList.Add(new WeaponSlotManager(weaponSlotGameObject.transform));
                // Task task = new Task.Victory();
                // taskSystem.AddTask(task);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                GameObject weaponGameObject = MyClass.CreateWorldSprite(null, "手枪", "Item",
                    GameAssets.Instance.rifle,
                    MyClass.GetMouseWorldPosition(woker.gameObject.transform.position.z,Camera.main),
                    new Vector3(1f, 1f, 1), 1, Color.white);
                taskSystem.EnqueueTask((() =>
                {
                    //遍历所有的武器槽管理类,若未发现有空的武器槽则返回null，队列任务不会出队列，woker就无法搬运散落的武器
                    for (int i = 0; i < weaponSlotManagerList.Count; i++)
                    {
                        if (weaponSlotManagerList[i].IsEmpty())
                        {
                            weaponSlotManagerList[i].SetWeaponInComing(true);
                            Task.CarryWeapon task = new Task.CarryWeapon
                            {
                                WeaponPosition = weaponGameObject.transform.position,
                                WeaponSlotPosition = weaponSlotManagerList[i].GetPosition(),
                                grabWeapon = (Transform transform) =>
                                {
                                    weaponGameObject.transform.SetParent(transform);
                                },
                                dropWeapon = (() =>
                                {
                                    weaponGameObject.transform.SetParent(null);
                                    weaponSlotManagerList[i].setWeaponTransform(weaponGameObject.transform);
                                    weaponGameObject.transform.position = weaponSlotManagerList[i].GetPosition();
                                })
                            };
                            return task;
                        }
                    }
                    return null;
                }));
            }

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                woker = Woker.Create(MyClass.GetMouseWorldPosition(woker.gameObject.transform.position.z,Camera.main));
                WorkTransportTaskAI workTransportTaskAI = woker.gameObject.AddComponent<WorkTransportTaskAI>();
                workTransportTaskAI.setUp(woker,transportTaskSystem);
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                woker = Woker.Create(MyClass.GetMouseWorldPosition(woker.gameObject.transform.position.z,Camera.main));
                WorkerTaskAI workTransportTaskAI = woker.gameObject.AddComponent<WorkerTaskAI>();
                workTransportTaskAI.setUp(woker,taskSystem);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameObject gameObject = MyClass.CreateWorldSprite(null, "垃圾", "Environment", GameAssets.Instance.sprite,
                    MyClass.GetMouseWorldPosition(woker.gameObject.transform.position.z, Camera.main),
                    new Vector3(1f, 1f, 1), 1, Color.white);
                SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                float time = Time.time + 5f;
                //任务入队,EnqueueTask接收的是一个事件参数,它生成一个queueTask，保存这个事件，而每0.2s执行一次的出队操作会调用一次这个事件，
                //如果调节未达成就不会出队反之会出队变成正常的task，而这个事件里的条件就可以由你任意定义，队列任务那边是不管的，只由这个事件函数来控制是不是出队
                taskSystem.EnqueueTask((() =>
                {
                    if (Time.time >= time)
                    {
                        //任务构造函数，传入事件参数，事件参数包含行为是调用FunctionUpdater.Create,这个方法是每帧调用一次它的事件参数，而传入的事件执行的是给sprite的alpha值减少一个帧的时间量。
                        Task taskBase = new Task.Clean(gameObject.transform.position, (() =>
                        {
                            float alpha = 1f;
                            FunctionUpdater.Create((() =>
                            {
                                alpha -= Time.deltaTime;
                                spriteRenderer.color = new Color(1f, 1, 1, alpha);
                                if (alpha <= 0f)
                                {
                                    GameObject.Destroy(gameObject);
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }));
                        }));
                        return taskBase;
                    }
                    else
                    {
                        return null;
                    }
                }));

                // taskSystem.AddTask(task);
            }
            
            
        }
        public class WeaponSlotManager
        {
            private Transform weaponSlotTransform;
            private Transform weaponTransform;
            private bool isWeaponInComing;
            private bool isEmpty;

            public WeaponSlotManager(Transform weaponSlotTransform)
            {
                this.weaponSlotTransform = weaponSlotTransform;
                setWeaponTransform(null);
            }

            public bool IsEmpty()
            {
                return this.weaponTransform == null && !isWeaponInComing;
            }

            public void SetWeaponInComing(bool isWeaponInComing)
            {
                this.isWeaponInComing = isWeaponInComing;
                UpdateSprite();

            }
            public void setWeaponTransform(Transform weaponTransform)
            {
                this.weaponTransform = weaponTransform;
                if (this.weaponTransform != null)
                {
                    //有武器放置于武器槽位时，生成转运武器新任务
                    TransportTask.TakeWeaponFromSlotPosition task = new TransportTask.TakeWeaponFromSlotPosition
                    {
                        
                        weaponSlotPosition = GetPosition(),
                        targetPosition = GetPosition() + new Vector3(15, 0, 0),
                        GrabWeapon = (Transform transform) =>
                        {
                            weaponTransform.SetParent(transform); 
                            setWeaponTransform(null);
                        },
                        dropWeapon = (() =>
                        {
                            weaponTransform.SetParent(null);
                            weaponTransform.position = GetPosition() + new Vector3(15, 0, 0);
                        })
                    };
                    transportTaskSystem.AddTask(task);
                }
                isWeaponInComing = false;
                UpdateSprite();
                
                // FunctionTimer.Create((() =>
                // {
                //     if (weaponTransform != null)
                //     {
                //         GameObject.Destroy(weaponTransform.gameObject);
                //         setWeaponTransform(null);
                //     }
                // }), 4f);
            }

            public void UpdateSprite()
            {
                weaponSlotTransform.gameObject.GetComponent<SpriteRenderer>().color = IsEmpty() ? Color.gray : Color.red;
            }
            public Vector3 GetPosition()
            {
                return weaponSlotTransform.position;
            }
            
        }
    }
    //武器槽放置管理类
    
    public class Task : TaskBase
    {
        public class MovePosition : Task
        {
            public Vector3 targetPosition;
        }
        public class Victory:Task
        {
        }
        public class Clean : Task
        {
            public Vector3 TargetPosition;
            public Action CleanOver;

            public Clean(Vector3 targetPosition, Action action)
            {
                this.TargetPosition = targetPosition;
                CleanOver = action;
            }
        }
        public class CarryWeapon:Task
        {
            public Vector3 WeaponPosition;
            public Vector3 WeaponSlotPosition;
            public Action<Transform> grabWeapon;
            public Action dropWeapon;
        }
    }
    public class TransportTask : TaskBase
    {
        public class TakeWeaponFromSlotPosition: TransportTask
        {
            public Vector3 weaponSlotPosition;
            public Vector3 targetPosition;
            public Action<Transform> GrabWeapon;
            public Action dropWeapon;
        }
    }
}


