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
    }
    public class GameHandler : MonoBehaviour
    {
        private PL_TaskSystem<TaskBase> taskSystem;
        private static PL_TaskSystem<TaskBase> transportTaskSystem;
        private PL_TaskSystem<TaskBase> gatherTaskSystem;
        private Woker woker;
        private ITaskAI taskAI;
        private GameObject weaponSlotGameObject;
        private GameObject minePointGameObject;

        private List<MineManager> mineManagerList;
        private int idleMinerAmount;
        private MineManager oneMineManager;
        
        private Transform MineButton;
        private MouseType mouseType;
        private GameObject attachMouseSprite;

        private void Awake()
        {
            GameResource.Init();
        }

        private void Start()
        {
             taskSystem = new PL_TaskSystem<TaskBase>();
             transportTaskSystem = new PL_TaskSystem<TaskBase>();
             gatherTaskSystem = new PL_TaskSystem<TaskBase>();

             mineManagerList = new List<MineManager>();
             
             MineManager.OnMinePointClicked += handleMinePointClicked;
             
             MineButton = GameObject.Find("MineButton").transform;
             MineButton.GetComponent<Button_UI>().ClickFunc += handleMineButtonClick;


             createWorker(new Vector3(0,0,0),WokerType.Miner);

             mouseType = MouseType.None;
             
        }
        //取消点击采矿按钮的状态
        private void cancleHitMine()
        {
            mouseType = MouseType.None;
            Destroy(attachMouseSprite);
        }
        //处理点击矿点事件
        private void handleMinePointClicked(MineManager mineManager)
        {
            if(mouseType == MouseType.HitMine)
            {
                mineManagerList.Add(mineManager);
                Destroy(mineManager.GetGoldPointTransform().gameObject.GetComponent<Button_Sprite>());
            }
        }
        //处理点击采矿按钮事件
        private void handleMineButtonClick()
        {
            mouseType = MouseType.HitMine;
            if(attachMouseSprite == null)
            {
                attachMouseSprite = MyClass.CreateWorldSprite(null, "mineAttachMouse", "AttachIcon", GameAssets.Instance.MiningShovel,
                    MyClass.GetMouseWorldPosition(0, Camera.main) - Vector3.up , Vector3.one, 1, Color.white);
            }
        }


        private GameObject createMinePoint(Vector3 position)
        {
            minePointGameObject = GameAssets.Instance.createItem(null, MyClass.GetMouseWorldPosition(0, Camera.main),
                ItemType.MinePoint);
            oneMineManager = new MineManager(minePointGameObject.transform);
            return minePointGameObject;
        }

        private void createWorker(Vector3 position,WokerType wokerType)
        {
            woker = Woker.Create(position);
            switch (wokerType)
            {
                case WokerType.Miner:
                    taskAI = woker.gameObject.AddComponent<WorkGatherTaskAI>();
                    taskAI.setUp(woker,gatherTaskSystem);
                    idleMinerAmount++;
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
            //采矿图标跟随鼠标
            if(attachMouseSprite != null)
                attachMouseSprite.transform.position = MyClass.GetMouseWorldPosition(0, Camera.main) - Vector3.up;
            
            if (idleMinerAmount > 0 && mineManagerList.Count > 0)
            {
                idleMinerAmount--;
                MineManager mineManager = mineManagerList[0];
                mineManagerList.RemoveAt(0);
                GatherTask.GatherGold task = new GatherTask.GatherGold
                {
                    mineManagerList = this.mineManagerList,
                    mineManager =  mineManager,
                    StorePosition = GameObject.Find("Crate").transform.position,
                    GoldGrabed = (amount,minemanager) =>
                    {
                        minemanager.GiveGold(amount);
                    },
                    GoldDropde = () =>
                    {
                        idleMinerAmount++;
                    },
                };
                gatherTaskSystem.AddTask(task);
            }

            
            //生成矿点的操作
            if (Input.GetKeyDown(KeyCode.Space))
            {
                createMinePoint(MyClass.GetMouseWorldPosition(0, Camera.main));
            }
            //生成工人操作
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                createWorker(MyClass.GetMouseWorldPosition(0,Camera.main),WokerType.Miner);
            }
        }
        
        //矿点管理类对象
        public class MineManager
        {
            private Transform minePointTransform;
            private int GoldAmount;
            public static int MaxAmount = 2;
            public static Action<MineManager> OnMinePointClicked;
            public MineManager(Transform minePointTransform)
            {
                this.minePointTransform = minePointTransform;
                GoldAmount = MaxAmount;
                minePointTransform.GetComponent<Button_Sprite>().ClickFunc = () =>
                {
                    OnMinePointClicked?.Invoke(this);
                };
            }

            public bool IsHasGold()
            {
                return GoldAmount > 0;
            }

            public Transform GetGoldPointTransform()
            {
                return minePointTransform;
            }

            public void GiveGold(int amount)
            {
                GoldAmount -= amount;
                if (GoldAmount < 1)
                {
                    GameObject.Destroy(GetGoldPointTransform().gameObject);
                }
            }

            public int getGoldAmount()
            {
                return GoldAmount;
            }
        }
    }
    
}


