using System;
using System.Collections.Generic;
using System.Linq;
using CodeMonkey;
using CodeMonkey.Utils;
using StateMachine;
using TaskSystem.GatherResource;
using TaskSystem.PathFInding;
using UnityEngine;


namespace TaskSystem
{
    public enum WokerType
    {
        Miner
    }

    public enum MouseState
    {
        None,
        HitMine,
        HitWood,
    }
    public class GameHandler : MonoBehaviour
    {
        public Camera camera1;
        public Camera camera2;
        private Camera currentCamera;
        private JobOrderPanel orderPanel;
        private GameObject weaponSlotGameObject;
        private GameObject resourcePointGameObject;
        private Woker woker;
        private ITaskAI taskAI;
        MyGrid<PathNode> grid;

        public static Dictionary<JobType, PL_TaskSystem<TaskBase>> JobTypeTaskSystemDictionary;

        private Transform MineButton;
        private Transform cutButton;
        private MouseState mouseState;
        private GameObject attachMouseSprite;

        
        private void Awake()
        {
            GameResource.Init();
            JobTypeTaskSystemDictionary = new Dictionary<JobType, PL_TaskSystem<TaskBase>>();

            for (int i = 0; i < (int) JobType.enumcount; i++)
            {
                JobTypeTaskSystemDictionary.Add((JobType)i,new PL_TaskSystem<TaskBase>());
            }
            
            ResourceManager.OnResourceClicked += handleMinePointClicked;
             
            MineButton = GameObject.Find("MineButton").transform;
            MineButton.GetComponent<Button_UI>().ClickFunc += handleMineButtonClick;
            cutButton = GameObject.Find("CutButton").transform;
            cutButton.GetComponent<Button_UI>().ClickFunc += handleCutButtonClick;
            orderPanel = GameObject.Find("OrderPanel").GetComponent<JobOrderPanel>();

            camera1.enabled = true;
            camera2.enabled = false;
            mouseState = MouseState.None;
            grid = new MyGrid<PathNode>(10, 10, (position) => { return new PathNode(position, true); });
        }

        private void Start()
        {
            createWorker(new Vector3(0,0,0),WokerType.Miner);
        }

        //取消点击采矿按钮的状态
        private void cancleHitMine()
        {
            mouseState = MouseState.None;
            Destroy(attachMouseSprite);
        }
        //处理点击资源点事件
        private void handleMinePointClicked(ResourceManager resourceManager)
        {
            switch (resourceManager.ResourceType)
            {
                case ResourceType.Gold:
                    if(mouseState == MouseState.HitMine)
                    {
                        GatherResourceTask task = new GatherResourceTask
                        {
                            jobType = JobType.GatherGold,
                            resourceManager =  resourceManager,
                            StorePosition = GameObject.Find("Crate").transform.position,
                            ResourceGrabed = (amount,minemanager) =>
                            {
                                minemanager.GiveResource(amount);
                            }
                        };
                        JobTypeTaskSystemDictionary[JobType.GatherGold].AddTask(task);
                        Destroy(resourceManager.GetResourcePointTransform().gameObject.GetComponent<Button_Sprite>());
                    }
                    break;
                case ResourceType.Wood:
                    if (mouseState == MouseState.HitWood)
                    {
                        GatherResourceTask task = new GatherResourceTask
                        {
                            jobType = JobType.GatherWood,
                            resourceManager = resourceManager,
                            StorePosition = GameObject.Find("Crate").transform.position,
                            ResourceGrabed = ((amount, woodManager) =>
                            {
                                woodManager.GiveResource(amount);
                            })
                        };
                        JobTypeTaskSystemDictionary[JobType.GatherWood].AddTask(task);;
                        Destroy(resourceManager.GetResourcePointTransform().gameObject.GetComponent<Button_Sprite>());
                    }
                    break;
            }

        }
        //处理点击采矿按钮事件
        private void handleMineButtonClick()
        {
            mouseState = MouseState.HitMine;
            attachMouseSprite = MyClass.CreateWorldSprite(null, "mineAttachMouse", "AttachIcon", GameAssets.Instance.MiningShovel,
                    MyClass.GetMouseWorldPosition(0, Camera.main) - Vector3.up , Vector3.one, 1, Color.white);
        }
        private void handleCutButtonClick()
        {
            mouseState = MouseState.HitWood;
            attachMouseSprite = MyClass.CreateWorldSprite(null, "cutAttachMouse", "AttachIcon", GameAssets.Instance.CutKnife,
                    MyClass.GetMouseWorldPosition(0, Camera.main) - Vector3.up , Vector3.one, 1, Color.white);
        }


        private GameObject createResourcePoint(Vector3 position,ResourceType resourceType)
        {
            resourcePointGameObject = GameAssets.Instance.createResource(null, MyClass.GetMouseWorldPosition(0, Camera.main),
                resourceType);
            new ResourceManager(resourcePointGameObject.transform, resourceType);
            return resourcePointGameObject;
        }

        private void createWorker(Vector3 position,WokerType wokerType)
        {
            woker = Woker.Create(position);
            switch (wokerType)
            {
                case WokerType.Miner:
                    taskAI = woker.gameObject.AddComponent<WorkGatherTaskAI>();
                    taskAI.setUp(woker);
                    orderPanel.AddWorkerOnPanel(taskAI as WorkGatherTaskAI);
                    break;
            }
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                //根据鼠标状态确定左键点击的效果
                switch (mouseState)
                {
                    case MouseState.None:
                        break;
                    case MouseState.HitMine:
                        cancleHitMine();
                        break;
                }
            }

            //采集资源图标跟随鼠标
            if(attachMouseSprite != null)
                attachMouseSprite.transform.position = MyClass.GetMouseWorldPosition(0, Camera.main) - Vector3.up;
            
            //生成矿点的操作
            if (Input.GetKeyDown(KeyCode.Space))
            {
                createResourcePoint(MyClass.GetMouseWorldPosition(0, Camera.main),ResourceType.Gold);
            }
            //生成工人操作
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                createWorker(MyClass.GetMouseWorldPosition(0,Camera.main),WokerType.Miner);
            }
            //生成树木操作
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                createResourcePoint(MyClass.GetMouseWorldPosition(0, Camera.main),ResourceType.Wood);
            }
            

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (camera1.isActiveAndEnabled)
                {
                    camera1.enabled = false;
                    camera2.enabled = true;
                }
                else
                {
                    camera1.enabled = true;
                    camera2.enabled = false;
                }
            }
        }

        //资源点管理类对象
        public class ResourceManager
        {
            private ResourceType resourceType;
            private Transform ResourcePointTransform;
            private int resourceAmount;
            public static int MaxAmount = 4;
            public static Action<ResourceManager> OnResourceClicked;
            public ResourceManager(Transform resourcePointTransform, ResourceType resourceType)
            {
                this.ResourceType = resourceType;
                this.ResourcePointTransform = resourcePointTransform;
                resourceAmount = MaxAmount;
                resourcePointTransform.GetComponent<Button_Sprite>().ClickFunc = () =>
                {
                    OnResourceClicked?.Invoke(this);
                };
            }

            public ResourceType ResourceType
            {
                set
                {
                    resourceType = value;
                }
                get
                {
                    return resourceType;
                }
            }
            public bool IsHasResource()
            {
                return resourceAmount > 0;
            }

            public Transform GetResourcePointTransform()
            {
                return ResourcePointTransform;
            }

            public void GiveResource(int amount)
            {
                resourceAmount -= amount;
                if (resourceAmount < 1)
                {
                    GameObject.Destroy(GetResourcePointTransform().gameObject);
                }
            }

            public int GetResourceAmount()
            {
                return resourceAmount;
            }
        }
    }
    
}


