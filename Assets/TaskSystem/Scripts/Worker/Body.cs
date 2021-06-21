using System;
using System.Collections;
using System.Collections.Generic;
using CodeMonkey;
using CodeMonkey.Utils;
using UnityEngine;
using UnityEngine.UI;

public class Body
{
    public const int MaxCarryAmount = 3;
    public GameObject gameObject;
    private int characterId;
    private IMovePosition moveWay;
    private IMoveVelocity moveVelocity;

    private CharacterAnimation characterAnimation;
    
    private int carryAmount;
    // Start is called before the first frame update
    public static Body Create(int Id,Vector3 position)
    {
        return new Body(Id, position); 
    }

    private Body(int Id,Vector3 position)
    {
       GameObject unit =  GameAssets.Instance.createUnit(null, position);
       gameObject = unit;
       moveWay = gameObject.GetComponent<IMovePosition>();
       characterAnimation = gameObject.GetComponent<CharacterAnimation>();
       moveVelocity = gameObject.GetComponent<IMoveVelocity>();
       carryAmount = 0;
       characterId = Id;
    }
    //worker的移动行为之所以是行为 是因为做这件事可能不止表现层，还可能有逻辑层
    public void moveTo(Vector3 position, Action onArriveAtPosition = null)
    {
        //启动移动方式,到达目标 事件赋值给移动方式的移动结束后处理,赋值而不是加,避免上次移动结束的事件仍会被触发
        moveWay.Enable();
        moveWay.BindOnPostMoveEnd(onArriveAtPosition);
        moveWay.SetMovePosition(position);
    }

    public void moveTowards(Vector3 direction)
    {
        if(direction.Equals(Vector3.zero))
        {
            moveVelocity.Disable();
        }
        else
        {
            moveVelocity.Enable();
            moveVelocity.SetVelocity(direction);
            characterAnimation.PlayDirectMoveAnimation(characterId,direction,false);
        }
    }
    //worker的闲置行为
    public void Idle()
    {
        characterAnimation.PlayobjectAnimaiton(characterId,0,ObjectAnimationType.Idle);
    }
    //worker的胜利行为
    public void Victory(int loopTimes,Action onVictoryEnd)
    {
        characterAnimation.OnAnimationEnd = onVictoryEnd;
        characterAnimation.PlayobjectAnimaiton(characterId,0,ObjectAnimationType.Throw);
    }
    //worker的清扫行为
    public void CleanUp(int loopTimes,Action onCleanEnd)
    {
        characterAnimation.OnAnimationEnd = onCleanEnd;
        characterAnimation.PlayobjectAnimaiton(characterId,loopTimes, ObjectAnimationType.Clean);
    }
    /// <summary>
    /// 工人的采集行为,根据资源类型不同,采集动画也不一样
    /// </summary>
    /// <param name="resourceType"></param>
    /// <param name="OnGatherEnd"></param>
    public void Gather(int actTimes,ResourceType resourceType, Action OnGatherEnd = null)
    {
        characterAnimation.OnAnimationEnd = OnGatherEnd;
        switch (resourceType)
        {
            case ResourceType.Gold:
                characterAnimation.PlayobjectAnimaiton(characterId,actTimes,ObjectAnimationType.Mine);
                break;
            case ResourceType.Wood:
                characterAnimation.PlayobjectAnimaiton(characterId,actTimes,ObjectAnimationType.Cut);
                break;
        }
    }

    public void Grab(int amount, Action OnGrabEnd = null)
    {
        carryAmount += amount;
        OnGrabEnd?.Invoke();
    }
    
    

    public void Drop(Action OnDropEnd = null)
    {
        carryAmount = 0;
        OnDropEnd?.Invoke();
    }
    

    public void Drop(GameObject gameObject,Action OnDropEnd = null)
    {
        gameObject.SetActive(false);
        Drop(OnDropEnd);
    }

    public int GetMaxCarryAmount()
    {
        return MaxCarryAmount;
    }

    public int GetCarryAmount()
    {
        return carryAmount;
    }

    public void AddCarryAomunt()
    {
        carryAmount++;
    }

    public int GetId()
    {
        return characterId;
    }
}
