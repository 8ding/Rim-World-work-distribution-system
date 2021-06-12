using System;
using System.Collections.Generic;
using System.Linq;
using CodeMonkey;
using CodeMonkey.Utils;
using StateMachine;
using TaskSystem.GatherResource;
using UnityEngine;


namespace TaskSystem
{
    public enum WokerType
    {
        Miner
    }

    public enum MouseType
    {
        None,
        HitMine,
        HitWood,
    }
    public class GameHandler : MonoBehaviour
    {
     
        private GameObject weaponSlotGameObject;
        private GameObject resourcePointGameObject;

        private static PL_TaskSystem<TaskBase> gatherWoodTaskSystem;
        private PL_TaskSystem<TaskBase> gatherGoldTaskSystem;
        private Woker woker;
        private ITaskAI taskAI;

        private List<ResourceManager> mineManagerList;
        private List<ResourceManager> woodManagerList;

        public static Dictionary<JobType, PL_TaskSystem<TaskBase>> JobTypeTaskSystemDictionary;
        
        private Transform MineButton;
        private Transform cutButton;
        private MouseType mouseType;
        private GameObject attachMouseSprite;

        private void Awake()
        {
            GameResource.Init();
            gatherWoodTaskSystem = new PL_TaskSystem<TaskBase>();
            gatherGoldTaskSystem = new PL_TaskSystem<TaskBase>();
            JobTypeTaskSystemDictionary = new Dictionary<JobType, PL_TaskSystem<TaskBase>>
            {
                {JobType.GatherGold, gatherGoldTaskSystem},
                {JobType.GatherWood, gatherWoodTaskSystem}
            };
                         
            mineManagerList = new List<ResourceManager>();
            woodManagerList = new List<ResourceManager>();

            ResourceManager.OnResourceClicked += handleMinePointClicked;
             
            MineButton = GameObject.Find("MineButton").transform;
            MineButton.GetComponent<Button_UI>().ClickFunc += handleMineButtonClick;
            cutButton = GameObject.Find("CutButton").transform;
            cutButton.GetComponent<Button_UI>().ClickFunc += handleCutButtonClick;

            createWorker(new Vector3(0,0,0),WokerType.Miner);

            mouseType = MouseType.None;
        }

        //取消点击采矿按钮的状态
        private void cancleHitMine()
        {
            mouseType = MouseType.None;
            Destroy(attachMouseSprite);
        }
        //处理点击资源点事件
        private void handleMinePointClicked(ResourceManager resourceManager)
        {
            switch (resourceManager.ResourceType)
            {
                case ResourceType.Gold:
                    if(mouseType == MouseType.HitMine)
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
                        gatherGoldTaskSystem.AddTask(task);
                        Destroy(resourceManager.GetResourcePointTransform().gameObject.GetComponent<Button_Sprite>());
                    }
                    break;
                case ResourceType.Wood:
                    if (mouseType == MouseType.HitWood)
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
                        gatherWoodTaskSystem.AddTask(task);
                        Destroy(resourceManager.GetResourcePointTransform().gameObject.GetComponent<Button_Sprite>());
                    }
                    break;
            }

        }
        //处理点击采矿按钮事件
        private void handleMineButtonClick()
        {
            mouseType = MouseType.HitMine;
            attachMouseSprite = MyClass.CreateWorldSprite(null, "mineAttachMouse", "AttachIcon", GameAssets.Instance.MiningShovel,
                    MyClass.GetMouseWorldPosition(0, Camera.main) - Vector3.up , Vector3.one, 1, Color.white);
        }
        private void handleCutButtonClick()
        {
            mouseType = MouseType.HitWood;
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
                    break;
            }
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                //根据鼠标状态确定左键点击的效果
                switch (mouseType)
                {
                    case MouseType.None:
                        break;
                    case MouseType.HitMine:
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

            if (Input.GetKeyDown(KeyCode.E))
            {
                WorkGatherTaskAI workGatherTaskAI = woker.gameObject.GetComponent<WorkGatherTaskAI>();
                workGatherTaskAI.ModifyOrder(JobType.GatherWood);
                CMDebug.TextPopupMouse("采木头顺序" + workGatherTaskAI.jobtypeOrderDictionary[JobType.GatherWood]);
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


