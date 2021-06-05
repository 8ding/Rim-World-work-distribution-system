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
    public event Action OnMovEnd;//当移动停止,移动方式需要做的事情
    public Action OnPostMoveEnd;//移动停止的后处理

    private void Start()
    {
        movePosition = gameObject.transform.position;
        moveVelocity = GetComponent<IMoveVelocity>();

    }

    public void SetMovePosition(Vector3 movePosition) {
        this.movePosition = movePosition;
        OnMovEnd += Disable;
    }

    private void Update() {
        moveDir = (movePosition - transform.position).normalized;
        if (Vector3.Distance(movePosition, transform.position) < .1f)
        {
            moveDir = Vector3.zero;
            moveVelocity.SetVelocity(moveDir);
            OnMovEnd?.Invoke();
            OnPostMoveEnd?.Invoke();
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

    public void OnDestroy()
    {
        OnMovEnd -= Disable;
        OnMovEnd -= moveVelocity.Disable;
    }
    
}
