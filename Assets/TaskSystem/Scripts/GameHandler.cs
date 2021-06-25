using System;
using System.Collections.Generic;
using CodeMonkey.Utils;
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
    private JobOrderPanel orderPanel;
    private GameObject resourcePointGameObject;
    private Body _m_body;
    private WorkerAI taskAI;
    private PlayerControlAI playerAI;
    private PathFinding pathFInding;

   
    
    private Transform MineButton;
    private Transform cutButton;
    private MouseState mouseState;
    private GameObject attachMouseSprite;

    
    private void Awake()
    {
        GameResource.Init();
        //任务中心监听任务事件
        ResourceManager.OnResourceClicked += handleMinePointClicked;
         
        MineButton = GameObject.Find("MineButton").transform;
        MineButton.GetComponent<Button_UI>().ClickFunc += handleMineButtonClick;
        cutButton = GameObject.Find("CutButton").transform;
        cutButton.GetComponent<Button_UI>().ClickFunc += handleCutButtonClick;
        orderPanel = GameObject.Find("OrderPanel").GetComponent<JobOrderPanel>();

        camera1.enabled = true;
        camera2.enabled = false;
        mouseState = MouseState.None;
    }

    private void Start()
    {
        createUnit(new Vector3(0,0,0));
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

                    EventCenter.Instance.EventTrigger<IArgs>("ClickGoldResource",new EventParameter<ResourceManager>(resourceManager));
                    Destroy(resourceManager.GetResourcePointTransform().gameObject.GetComponent<Button_Sprite>());
                }
                break;
            case ResourceType.Wood:
                if (mouseState == MouseState.HitWood)
                {
                    EventCenter.Instance.EventTrigger("ClickWoodResource",new EventParameter<ResourceManager>(resourceManager));
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

    private void createUnit(Vector3 position)
    {
        _m_body = Body.Create(0,position);
//            playerAI = _m_body.gameObject.AddComponent<PlayerControlAI>();
        taskAI = _m_body.gameObject.AddComponent<WorkerAI>();
        taskAI.setUp(_m_body);
        orderPanel.AddWorkerOnPanel(taskAI);
//            taskAI.Disable();
//            playerAI.setUp(_m_body);
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
                    EventCenter.Instance.EventTrigger<IArgs>("RightClick",new EventParameter<Vector3>(MyClass.GetMouseWorldPosition(0, camera1)));
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
    



