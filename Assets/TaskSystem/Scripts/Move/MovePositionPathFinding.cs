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
    
    public Vector3 moveDir;
    private IMoveVelocity moveVelocity;
    public event Action OnMovEnd;//当移动停止,移动方式需要做的事情
    public Action OnPostMoveEnd;//移动停止的后处理
    public PathFinding pathFinding;
    private MyGrid<PathNode> myGrid;
    private List<PathNode> pathNodes;
    private int currentNodeIndex;
    private CharacterAnimation characterAnimation;
    private int characterId;

    private void Awake()
    {
        moveVelocity = GetComponent<IMoveVelocity>();
        characterAnimation = GetComponent<CharacterAnimation>();
        myGrid = (Resources.Load("New Grid Setting") as GridSetting).grid;

    }

    private void Start()
    {
        pathFinding = new PathFinding(myGrid);
        pathNodes = new List<PathNode>();
        characterId = GetComponent<WorkerAI>().GetId();
    }

    public void SetMovePosition(Vector3 movePosition) {
        pathNodes = pathFinding.FindPath(transform.position, movePosition);
        OnMovEnd += Disable;
        if (pathNodes != null)
        {
            pathFinding.DrawPath(pathNodes);
            currentNodeIndex = 0;
            characterAnimation.PlayDirectMoveAnimation(characterId,pathNodes[currentNodeIndex].worldPosition);
        }
        else
        {
            OnMovEnd?.Invoke();
            OnPostMoveEnd?.Invoke();
        }
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
                    characterAnimation.PlayDirectMoveAnimation(characterId,pathNodes[currentNodeIndex].worldPosition);
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
