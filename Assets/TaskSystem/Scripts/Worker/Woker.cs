using System;
using System.Collections;
using System.Collections.Generic;
using CodeMonkey;
using CodeMonkey.Utils;
using UnityEngine;

public class Woker:IWorker
{
    public const int MaxCarryAmount = 3;
    public GameObject gameObject;
        
    private MovePositionDirect moveWay;

    private CharacterAnimation characterAnimation;
    
    private int carryAmount;
    // Start is called before the first frame update
    public static Woker Create(Vector3 position)
    {
        return new Woker(position);
    }

    private Woker(Vector3 position)
    {
       GameObject unit =  GameAssets.Instance.createUnit(null, position);
       gameObject = unit;
       moveWay = gameObject.GetComponent<MovePositionDirect>();
       characterAnimation = gameObject.GetComponent<CharacterAnimation>();
       carryAmount = 0;
    }
    //worker的移动行为之所以是行为 是因为做这件事可能不止表现层，还可能有逻辑层
    public void moveTo(Vector3 position, Action onArriveAtPosition = null)
    {
        //启动移动方式,到达目标 事件赋值给移动方式的移动结束后处理,赋值而不是加,避免上次移动结束的事件仍会被触发
        moveWay.Enalbe();
        moveWay.OnPostMoveEnd = onArriveAtPosition;
        moveWay.SetMovePosition(position);
        characterAnimation.PlayDirectMoveAnimation(position);
    }
    //worker的闲置行为
    public void Idle()
    {
        characterAnimation.PlayIdleAnimation();
    }
    //worker的胜利行为
    public void Victory(Action onVictoryEnd)
    {
        characterAnimation.OnAnimationEnd = onVictoryEnd;
        characterAnimation.PlayVictoryAnimation();
    }
    //worker的清扫行为
    public void CleanUp(Action onCleanEnd)
    {
        characterAnimation.OnAnimationEnd = onCleanEnd;
        characterAnimation.PlayCleanAnimation(2);
    }

    public void Grab(int Grabtimes, Action OnGrabEnd = null)
    {
        throw new NotImplementedException();
    }

    public void Grab(Action OnGrabEnd = null)
    {
        carryAmount++;
        OnGrabEnd?.Invoke();
    }
    

    /// <summary>
    /// Woker的挖矿行为，mineTImes为挖矿的动作次数
    /// </summary>
    /// <param name="mineTimes"></param>
    /// <param name="OnMineEnd"></param>
    public void Mine(int mineTimes,Action OnMineOnce,Action OnMineEnd = null)
    {
        characterAnimation.OnAnimationEnd = OnMineEnd;
        characterAnimation.PlayMineAnimation(mineTimes,OnMineOnce);
    }

    public void Drop(Action OnDropEnd = null)
    {
        carryAmount = 0;
        OnDropEnd?.Invoke();
    }

    public void Cut(int cutTimes,Action OnCutEnd = null)
    {
        characterAnimation.OnAnimationEnd = OnCutEnd;
        characterAnimation.PlayCutAnimation(cutTimes);
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
}
