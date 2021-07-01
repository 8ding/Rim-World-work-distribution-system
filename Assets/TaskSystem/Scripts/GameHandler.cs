using System;
using System.Collections;
using System.Collections.Generic;
using CodeMonkey.Utils;
using JetBrains.Annotations;
using TaskSystem.GatherResource;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;


public enum PlayerControlWay
{
    WorkerWay,
    PlayerWay,
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
    private GameObject resourcePointGameObject;
    private UnitController taskAI;
    private PlayerControlAI playerAI;
    private PathFinding pathFInding;

   
    
    private Transform MineButton;
    private Transform cutButton;
    private MouseState mouseState;
    private GameObject attachMouseSprite;

    
    private void Awake()
    {
        GameResource.Init();
        CreateThingManager.Init();
        PathManager.Init();
        InputManager.Instance.StartOrEnd(true);
        EventCenter.Instance.AddEventListener<IArgs>(EventType.Test, HandleTest);
      
        EventCenter.Instance.AddEventListener<IArgs>(EventType.GetSomeKeyDown, handleKeyDown);
        MineButton = GameObject.Find("MineButton").transform;
        MineButton.GetComponent<Button_UI>().ClickFunc += handleMineButtonClick;
        cutButton = GameObject.Find("CutButton").transform;
        cutButton.GetComponent<Button_UI>().ClickFunc += handleCutButtonClick;


        camera1.enabled = true;
        camera2.enabled = false;
        mouseState = MouseState.None;
    }

    private void Start()
    {
        EventCenter.Instance.EventTrigger<IArgs>(EventType.CreateUnit,new EventParameter<string,Vector3,int,float>("Unit",Vector3.zero, 0,6f));
        EventCenter.Instance.EventTrigger<IArgs>(EventType.CreateUnit,new EventParameter<string,Vector3,int,float>("Unit",Vector3.zero, 1,6f));
    }

    //取消点击采矿按钮的状态
    private void cancleHitMine()
    {
        mouseState = MouseState.None;
        Destroy(attachMouseSprite);
    }
    /// <summary>
    /// 处理按键事件
    /// </summary>
    /// <param name="_iArgs"></param>
    public void handleKeyDown(IArgs _iArgs)
    {
        KeyCode keyCode = (_iArgs as EventParameter<KeyCode>).t;
        switch (keyCode )
        {
            case KeyCode.Tab:
                EventCenter.Instance.EventTrigger<IArgs>(EventType.ChangeMode, null);
                break;
            case KeyCode.Space:
                Vector3 position = MyClass.GetMouseWorldPosition(0, camera1);
                if(PathManager.Instance.GetResourceManager(position) == null)
                {
                    PathManager.Instance.NewResource(position);
                }
                ResourceManager resourceManager = PathManager.Instance.GetResourceManager(position);
                Vector3 fixedposition = PathManager.Instance.GetGridPosition(position);
                resourceManager.AddResourceContent(fixedposition, ResourcePointType.GoldPoint, 20);
                break;
            case KeyCode.Q:
                EventCenter.Instance.EventTrigger<IArgs>(EventType.Test,new EventParameter<Vector3>((MyClass.GetMouseWorldPosition(0, camera1))));
                break;
        }
    }
    //处理点击资源点事件
    private void handleMinePointClicked(ResourceManager resourceManager)
    {
        switch (resourceManager.resourcePointType)
        {
            case ResourcePointType.GoldPoint:
                if(mouseState == MouseState.HitMine)
                {
                    EventCenter.Instance.EventTrigger<IArgs>(EventType.ClickGoldResource,new EventParameter<ResourceManager>(resourceManager));
                    resourceManager.GetContentObj().gameObject.GetComponent<Button_Sprite>().enabled = false;
                }
                break;
            case ResourcePointType.WoodPoint:
                if (mouseState == MouseState.HitWood)
                {
                    EventCenter.Instance.EventTrigger<IArgs>(EventType.ClickWoodResource,new EventParameter<ResourceManager>(resourceManager));
                    resourceManager.GetContentObj().gameObject.GetComponent<Button_Sprite>().enabled = false;
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
    /// <summary>
    /// 专门用于测试的函数,用于在鼠标点击处测试
    /// </summary>
    /// <param name="_iArgs"></param>
    private void HandleTest(IArgs _iArgs)
    {
        Vector3 position = (_iArgs as EventParameter<Vector3>).t;
        if(PathManager.Instance.GetResourceManager(position) == null)
        {
            PathManager.Instance.NewResource(position);
        }
        ResourceManager resourceManager = PathManager.Instance.GetResourceManager(position);
        Vector3 fixedPosition = PathManager.Instance.GetGridPosition(position);
        resourceManager.AddResourceContent(fixedPosition, ResourcePointType.WoodPoint, 20);
    }

    /// <summary>
    /// 将游戏物体在八个方向每1秒放置一次的协程
    /// </summary>
    private IEnumerator Circle(GameObject _gameObject,Vector3 _position)
    {
        _gameObject.transform.position = PathManager.Instance.GetGridPosition(_position);
        for (int i = 0; i < (int) MoveDirection.enumCount; i++)
        {
            yield return new WaitForSeconds(1f);
            _gameObject.transform.position = PathManager.Instance.GetOneOffsetPositon(_position, (MoveDirection) i);
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
                    //任务事件触发
                    EventCenter.Instance.EventTrigger<IArgs>(EventType.RightClick,new EventParameter<Vector3>(MyClass.GetMouseWorldPosition(0, camera1)));
                    break;
                case MouseState.HitMine:
                    cancleHitMine();
                    break;
            }
        }
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 position = MyClass.GetMouseWorldPosition(0, camera1);
            ResourceManager resourceManager = PathManager.Instance.GetResourceManager(position);
            switch (mouseState)
            {

                case MouseState.HitMine:
                    if(resourceManager == null)
                    {
                        break;
                    }
                    if(resourceManager.resourcePointType == ResourcePointType.GoldPoint && resourceManager.IsHasContent())
                    {
                        TaskCenter.Instance.BuildTask(position,TaskType.GatherGold);
                    }
                    break;
                case MouseState.HitWood:
                    if(resourceManager == null)
                    {
                        break;
                    }
                    if(resourceManager.resourcePointType == ResourcePointType.WoodPoint && resourceManager.IsHasContent())
                    {
                        TaskCenter.Instance.BuildTask(position,TaskType.GatherWood);
                    }
                    break;
                default:
                    break;
            }
        }
        //采集资源图标跟随鼠标
        if(attachMouseSprite != null)
            attachMouseSprite.transform.position = MyClass.GetMouseWorldPosition(0, Camera.main) - Vector3.up;
        
    }

    //可放置物体管理对象
    public class PlacedObjectManager
    {
        public GameObject ContenObj;
        public int ContentAmount;
        public PlacedObjectType placedObjectType;
        public bool IsHasContent()
        {
            return ContentAmount > 0;
        }

        public GameObject GetContentObj()
        {
            return ContenObj;
        }
        
        public int GetContentAmount()
        {
            return ContentAmount;
        }


        public int GiveContent(int amount)
        {
            if(amount <= ContentAmount)
            {
                ContentAmount -= amount;
                return 0;
            }
            else
            {
                amount -= ContentAmount;
                ContentAmount = 0;
                return amount;
            }
        }

        protected  int AddContent(int amount)
        {
            if(amount <= GetAmountLeft())
            {
                ContentAmount += amount;
                SetPerformanceWithAmount();
                return 0;
            }
            else
            {
                amount -= GetAmountLeft();
                ContentAmount += GetAmountLeft();
                SetPerformanceWithAmount();
                return amount;
            }
        }

        protected int MinusContent(int _amount)
        {
            if(_amount <= ContentAmount)
            {
                ContentAmount -= _amount;
                SetPerformanceWithAmount();
                return 0;
            }
            else
            {
                _amount -= ContentAmount;
                ContentAmount = 0;
                SetPerformanceWithAmount();
                return _amount;
            }
        }
        protected virtual int GetAmountLeft()
        {
            return 0;
        }
        protected virtual void SetPerformanceWithAmount()
        {
            
        }

    }
    //资源点管理类对象
    public class ResourceManager : PlacedObjectManager
    {
        private ResourcePointType _m_resourcePointType;

        public static int MaxAmount = 20;
        public ResourceManager()
        {
            placedObjectType = PlacedObjectType.ResourcePoint;
        }

        public ResourcePointType resourcePointType
        {
            set
            {
                _m_resourcePointType = value;
            }
            get
            {
                return _m_resourcePointType;
            }
        }
        protected override int GetAmountLeft()
        {
            return MaxAmount - ContentAmount;
        }
        public  void SetNewResourcePointContent(Vector3 _position, ResourcePointType _resourcePointType, int amount)
        {
            this.resourcePointType = _resourcePointType;
            this.ContenObj = PoolMgr.Instance.GetObj(_m_resourcePointType.ToString());
            this.ContenObj.transform.position = _position;
            ContentAmount = MaxAmount;
            SetPerformanceWithAmount();
        }

        public int AddResourceContent(Vector3 _position, ResourcePointType _resourcePointType, int amount)
        {
            if(ContentAmount == 0)
            {
                SetNewResourcePointContent(_position,_resourcePointType,amount);
                return 0;
            }
            else if(_resourcePointType != resourcePointType)
            {
                return 0;
            }
            else
            {
                return base.AddContent(amount);
            }
        }

        public int MinusResourceContent(int amount)
        {
            return base.MinusContent(amount);
        }
        protected override void SetPerformanceWithAmount()
        {
            if(ContentAmount == 0)
            {
                PoolMgr.Instance.PushObj(ContenObj.gameObject);
                resourcePointType = ResourcePointType.None;
                return;
            }
            ContenObj.GetComponent<SpriteRenderer>().sprite = ResMgr.Instance.Load<Sprite>(_m_resourcePointType.ToString());
        }

    }
    //放置物品管理对象
    public class ItemManager: PlacedObjectManager
    {
        //一个单位和一个地面网格所能承载的最大物品数
        public static int MaxAmount = 10;
        public static int oneStepAmount = 5;
        public static int twoStepAmount = 10;
        public static int threeStepAmount = MaxAmount;
        private ItemType itemType;

        public ItemManager()
        {
            placedObjectType = PlacedObjectType.Item;
        }
        public  void SetNewItemContent(Vector3 position,ItemType _itemType,int amount)
        {
            this.itemType = _itemType;
            this.ContenObj = PoolMgr.Instance.GetObj(itemType.ToString());
            ContentAmount = amount;
            this.ContenObj.transform.position = position;
            SetPerformanceWithAmount();
            TaskCenter.Instance.BuildTask(position,GameObject.Find("Crate").transform.position,TaskType.CarryItem);
        }
        public  void SetNewItemContent(GameObject _gameObject,ItemType _itemType,int amount)
        {
            this.itemType = _itemType;
            this.ContenObj = PoolMgr.Instance.GetObj(itemType.ToString());
            ContentAmount = amount;
            this.ContenObj.transform.SetParent(_gameObject.transform);
            SetPerformanceWithAmount();
        }
        public int AddItemContent(Vector3 _position, ItemType _itemtype, int amount)
        {
            if(ContentAmount == 0)
            {
                SetNewItemContent(_position,_itemtype,amount);
                return 0;
            }
            else if(_itemtype != itemType)
            {
                return 0;
            }
            else
            {
                return base.AddContent(amount);
            }
        }
        public int AddItemContent(GameObject _gameObject, ItemType _itemtype, int amount)
        {
            if(ContentAmount == 0)
            {
                SetNewItemContent(_gameObject,_itemtype,amount);
                return 0;
            }
            else if(_itemtype != itemType)
            {
                return 0;
            }
            else
            {
                return base.AddContent(amount);
            }
        }
        protected override int GetAmountLeft()
        {
            return MaxAmount - ContentAmount;
        }

        protected override void SetPerformanceWithAmount()
        {
            if(ContentAmount == 0)
            {
                PoolMgr.Instance.PushObj(ContenObj.gameObject);
                itemType = ItemType.None;
                return;
            }
            ContenObj.GetComponent<SpriteRenderer>().sprite = ResMgr.Instance.Load<Sprite>(itemType.ToString());
        }
    }
}
    



