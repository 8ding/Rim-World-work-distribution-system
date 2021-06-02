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

    public event Action OnMoveEnd;
    public void SetMovePosition(Vector3 movePosition) {
        this.movePosition = movePosition;
    }

    private void Update() {
        moveDir = (movePosition - transform.position).normalized;
        if (Vector3.Distance(movePosition, transform.position) < 1f)
        {
            moveDir = Vector3.zero; // Stop moving when near
            OnMoveEnd?.Invoke();
        }
        GetComponent<IMoveVelocity>().SetVelocity(moveDir);
    }

}
