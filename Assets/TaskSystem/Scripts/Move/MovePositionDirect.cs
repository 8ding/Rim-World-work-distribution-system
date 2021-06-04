/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePositionDirect : MonoBehaviour, IMovePosition {

    private Vector3 movePosition;
    public Vector3 moveDir;
    private IMoveVelocity moveVelocity;
    public event Action OnMoveEnd;

    private void Start()
    {
        movePosition = gameObject.transform.position;
        moveVelocity = GetComponent<IMoveVelocity>();
        OnMoveEnd += Disable;
        OnMoveEnd += moveVelocity.Disable;
    }

    public void SetMovePosition(Vector3 movePosition) {
        moveVelocity.Enable();
        this.movePosition = movePosition;
    }

    private void Update() {
        moveDir = (movePosition - transform.position).normalized;
        if (Vector3.Distance(movePosition, transform.position) < .1f)
        {
            OnMoveEnd?.Invoke();
        }
        moveVelocity.SetVelocity(moveDir);
    }

    public void Disable()
    {
        this.enabled = false;
    }

    public void Enalbe()
    {
        this.enabled = true;
    }
}
