using System;
using System.Collections;
using System.Collections.Generic;
using CodeMonkey.Utils;
using JetBrains.Annotations;
using TaskSystem.GatherResource;
using UnityEngine;



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
        TaskCenter.Init();
        CreateThingManager.Init();
        ResourceManager.OnResourceClicked += handleMinePointClicked;
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
                EventCenter.Instance.EventTrigger<IArgs>(EventType.CreatMinePoit, new EventParameter<Vector3,ResourceType>(MyClass.GetMouseWorldPosition(0, camera1),ResourceType.Gold));
                break;
            case KeyCode.Q:
                EventCenter.Instance.EventTrigger<IArgs>(EventType.Test,new EventParameter<Vector3>((MyClass.GetMouseWorldPosition(0, camera1))));
                break;
        }
    }
    //处理点击资源点事件
    private void handleMinePointClicked(ResourceManager resourceManager)
    {
        switch (resourceManager.ResourceType)
        {
            case ResourceType.Gold:
                if(mouseState == MouseState.HitMine)
                {

                    EventCenter.Instance.EventTrigger<IArgs>(EventType.ClickGoldResource,new EventParameter<ResourceManager>(resourceManager));
                    resourceManager.GetResourcePointTransform().gameObject.GetComponent<Button_Sprite>().enabled = false;
                }
                break;
            case ResourceType.Wood:
                if (mouseState == MouseState.HitWood)
                {
                    EventCenter.Instance.EventTrigger<IArgs>(EventType.ClickWoodResource,new EventParameter<ResourceManager>(resourceManager));
                    resourceManager.GetResourcePointTransform().gameObject.GetComponent<Button_Sprite>().enabled = false;
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
        PoolMgr.Instance.GetObj("Enemy",(_o =>
                {
                    StartCoroutine(Circle(_o, position));
                }
                ));
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
        
        //采集资源图标跟随鼠标
        if(attachMouseSprite != null)
            attachMouseSprite.transform.position = MyClass.GetMouseWorldPosition(0, Camera.main) - Vector3.up;
        
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
            resourcePointTransform.GetComponent<Button_Sprite>().enabled = true;
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
               PoolMgr.Instance.PushObj(GetResourcePointTransform().gameObject);
            }
        }

        public int GetResourceAmount()
        {
            return resourceAmount;
        }
    }
}
    



