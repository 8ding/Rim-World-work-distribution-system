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
        EventCenter.Instance.AddEventListener<string>(EventType.Test, HandleTest);
      
        EventCenter.Instance.AddEventListener<IArgs>(EventType.GetSomeKeyDown, handleKeyDown);
        EventCenter.Instance.AddEventListener<Item>(EventType.ClickOnItem,handleItemClicked);
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
        EventCenter.Instance.EventTrigger<IArgs>(EventType.CreateUnit,new EventParameter<string,Vector3,int,float>("Unit",Vector3.one, 1,6f));
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
                if(PathManager.Instance.GetItemInGrid(position) == null)
                {
                    int itemCode = Item.GoldPoint;
                    Item item = InventoryManager.Instance().CreateItem(itemCode, InventoryManager.Instance().GetItemDeatails(itemCode).MaxItemQuantity);
                    PathManager.Instance.SetItemOnGrid(item,position);
                }
                break;
            case KeyCode.Q:
                EventCenter.Instance.EventTrigger(EventType.Test,"看看会不会输出");
                break;
        }
    }
    //处理点击资源点事件
    private void handleItemClicked(Item _item)
    {
        switch (_item.ItemCode)
        {
            case Item.GoldPoint:
                if(mouseState == MouseState.HitMine)
                {
                    TaskCenter.Instance.BuildTask(_item.Position, TaskType.GatherGold);
                    _item.SetWhetherInTask(true);
                }
                break;
            case Item.WoodPoint:
                if (mouseState == MouseState.HitWood)
                {
                    TaskCenter.Instance.BuildTask(_item.Position, TaskType.GatherWood);
                    _item.SetWhetherInTask(true);
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
    private void HandleTest(string _outPut)
    {
        Debug.Log(_outPut);
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
        
        }
        //采集资源图标跟随鼠标
        if(attachMouseSprite != null)
            attachMouseSprite.transform.position = MyClass.GetMouseWorldPosition(0, Camera.main) - Vector3.up;
        
    }

 
    
}
    



