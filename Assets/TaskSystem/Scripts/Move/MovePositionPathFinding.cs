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

public class MovePositionPathFinding : MonoBehaviour, IMovePosition {

    private Vector3 movePosition;
    public Vector3 moveDir;
    private IMoveVelocity moveVelocity;
    public event Action OnMovEnd;//当移动停止,移动方式需要做的事情
    public Action OnPostMoveEnd;//移动停止的后处理
    public PathFinding pathFinding;
    private MyGrid<PathNode> myGrid;
    private List<PathNode> pathNodes;
    private int currentNodeIndex;

    private void Awake()
    {
        movePosition = gameObject.transform.position;
        moveVelocity = GetComponent<IMoveVelocity>();
        myGrid = (Resources.Load("New Grid Setting") as GridSetting).grid;
    }

    private void Start()
    {
        pathFinding = new PathFinding(myGrid);
        pathNodes = new List<PathNode>();
    }

    public void SetMovePosition(Vector3 movePosition) {
        
        this.movePosition = movePosition;
        pathNodes = pathFinding.FindPath(transform.position, movePosition);
        pathFinding.DrawPath(pathNodes);
        if (pathNodes != null)
        {
            currentNodeIndex = 0;
        }
        OnMovEnd += Disable;
    }

    public void BindOnPostMoveEnd(Action postMoveEnd)
    {
        OnPostMoveEnd = postMoveEnd;
    }

    public void Enable()
    {
        this.enabled = true;
    }

    private void Update() {
        if (pathNodes != null && pathNodes.Count > 0)
        {
            moveDir = (pathNodes[currentNodeIndex].worldPosition - transform.position).normalized;
            if (Vector3.Distance(pathNodes[currentNodeIndex].worldPosition, transform.position) < .1f)
            {
                moveDir = Vector3.zero;
                moveVelocity.SetVelocity(moveDir);
                if (currentNodeIndex < pathNodes.Count - 1)
                {
                    currentNodeIndex++;
                }
                else
                {
                    OnMovEnd?.Invoke();
                    OnPostMoveEnd?.Invoke();
                }
            }
            moveVelocity.SetVelocity(moveDir);
        }
    }
    
    public void Disable()
    {
        this.enabled = false;
    }
    

    public void OnDestroy()
    {
        OnMovEnd -= Disable;
        OnMovEnd -= moveVelocity.Disable;
    }
    
}
