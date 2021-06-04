using System;
using System.Collections;
using System.Collections.Generic;
using CodeMonkey.Utils;
using UnityEngine;

public class Woker:IWorker
{
    public GameObject gameObject;

    private MovePositionDirect moveWay;

    private CharacterAnimation characterAnimation;
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
    }

    public void moveTo(Vector3 position, Action onArriveAtPosition = null)
    {
        moveWay.Enalbe();
        moveWay.SetMovePosition(position);
        characterAnimation.PlayDirectMoveAnimation(position);
        moveWay.OnMoveEnd += onArriveAtPosition;
    }

    public void Idle()
    {
        characterAnimation.PlayIdleAnimation();
    }

    public void Victory(Action onVictoryEnd)
    {
        characterAnimation.OnVictoryEnd += onVictoryEnd;
        characterAnimation.PlayVictoryAnimation();
    }
    
}
