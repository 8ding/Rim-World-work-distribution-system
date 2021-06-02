using System;
using System.Collections;
using System.Collections.Generic;
using CodeMonkey.Utils;
using UnityEngine;

public class Woker:IWorker
{
    public GameObject gameObject;
    // Start is called before the first frame update
    public static Woker Create(Vector3 position)
    {
        return new Woker(position);
    }

    private Woker(Vector3 position)
    {
       GameObject unit =  GameAssets.Instance.createUnit(null, position);
       gameObject = unit;
    }

    public void moveTo(Vector3 position, Action onArriveAtPosition = null)
    {
        MovePositionDirect moveWay = gameObject.GetComponent<MovePositionDirect>();
        moveWay.SetMovePosition(position);
        moveWay.OnMoveEnd += onArriveAtPosition;
    }
}
