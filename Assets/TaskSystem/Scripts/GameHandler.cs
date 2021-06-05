using System;
using CodeMonkey;
using CodeMonkey.Utils;
using StateMachine;
using UnityEngine;

namespace TaskSystem
{
    public class GameHandler : MonoBehaviour
    {
        private PL_TaskSystem taskSystem;
        private Woker woker;
        private WeaponSlotManager weaponSlotManager;
        private GameObject weaponSlotGameObject;
        private void Start()
        {
             taskSystem = new PL_TaskSystem();
            //Woker.Create创造了一个新的Woker对象,在setUp里面TaskAI绑定了这个实例对象，因此是两个实例对象，也绑定了两个实例对象
            woker = Woker.Create(new Vector3(0, 0));
            WorkerTaskAI workerTaskAI = woker.gameObject.AddComponent<WorkerTaskAI>();
            workerTaskAI.setUp(woker, taskSystem);
            
            weaponSlotGameObject = MyClass.CreateWorldSprite(null, "武器存储", "Environment",
                GameAssets.Instance.WeaponSlot,
                new Vector3(-5,-5,0),
                new Vector3(1, 1, 1), 0, Color.white);
            

            weaponSlotManager = new WeaponSlotManager(weaponSlotGameObject.transform);

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
            PL_TaskSystem.Task.MovePosition movePosition = new PL_TaskSystem.Task.MovePosition
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
                PL_TaskSystem.Task task = new PL_TaskSystem.Task.MovePosition {targetPosition = (MyClass.GetMouseWorldPosition(woker.gameObject.transform.position.z,Camera.main))};
                taskSystem.AddTask(task);
            }
            if(Input.GetMouseButtonDown(0))
            {

                // PL_TaskSystem.Task task = new PL_TaskSystem.Task.Victory();
                // taskSystem.AddTask(task);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                GameObject weaponGameObject = MyClass.CreateWorldSprite(null, "手枪", "Item",
                    GameAssets.Instance.rifle,
                    new Vector3(5,5,0),
                    new Vector3(1f, 1f, 1), 1, Color.white);
                PL_TaskSystem.Task.CarryWeapon task = new PL_TaskSystem.Task.CarryWeapon
                {
                    WeaponPosition = weaponGameObject.transform.position,
                    WeaponSlotPosition = weaponSlotGameObject.transform.position,
                    grabWeapon = (Transform transform) =>
                    {
                        weaponGameObject.transform.SetParent(transform);
                    },
                    dropWeapon = (() =>
                    {
                        weaponGameObject.transform.SetParent(null);
                        weaponSlotManager.setWeaponTransform(weaponGameObject.transform);
                        weaponGameObject.transform.position = weaponSlotGameObject.transform.position;
                    })
                };
                taskSystem.EnqueueTask((() =>
                {
                    if (weaponSlotManager.IsEmpty())
                    {
                        weaponSlotManager.SetWeaponInComing(true);
                        return task;
                    }
                    else
                    {
                        return null;
                    }
                }));
            }

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                weaponSlotManager.setWeaponTransform(null);
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
                        PL_TaskSystem.Task task = new PL_TaskSystem.Task.Clean(gameObject.transform.position, (() =>
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
                        return task;
                    }
                    else
                    {
                        return null;
                    }
                }));

                // taskSystem.AddTask(task);
            }
            
            
        }
    }
    //武器槽放置管理类
    public class WeaponSlotManager
    {
        private Transform weaponSlotTransform;
        private Transform weaponTransform;
        private bool isWeaponInComing;

        public WeaponSlotManager(Transform weaponSlotTransform)
        {
            this.weaponSlotTransform = weaponSlotTransform;
            setWeaponTransform(null);
        }

        public bool IsEmpty()
        {
            return weaponTransform == null && !isWeaponInComing;
        }

        public void SetWeaponInComing(bool isWeaponInComing)
        {
            this.isWeaponInComing = isWeaponInComing;
            UpdateSprite();

        }
        public void setWeaponTransform(Transform weaponTransform)
        {
            if (weaponTransform == null)
            {
                if (this.weaponTransform != null)
                {
                    GameObject.Destroy(this.weaponTransform.gameObject);
                }
            }
            this.weaponTransform = weaponTransform;
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