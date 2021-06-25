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
    public Action OnPostMoveEnd;//移动停止的后处理
    
  
    private List<PathNode> pathNodes;
    private int currentNodeIndex;
    private CharacterAnimation characterAnimation;
    private UnitData unitData;

    private void Awake()
    {
        moveVelocity = GetComponent<IMoveVelocity>();
        characterAnimation = GetComponent<CharacterAnimation>();
        unitData = GetComponent<UnitData>();
    }

    private void OnEnable()
    {
        pathNodes = new List<PathNode>(); 
    }

    public void SetMovePosition(Vector3 movePosition) {
        pathNodes = PathManager.Instance.findPath(transform.position, movePosition);
        if (pathNodes != null && pathNodes.Count > 0)
        {
            currentNodeIndex = 0;
            characterAnimation.PlayDirectMoveAnimation(unitData.CharacterId,pathNodes[currentNodeIndex].worldPosition);
        }
        else
        {
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

    public void Disable()
    {
        this.enabled = false;
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
                    characterAnimation.PlayDirectMoveAnimation(unitData.CharacterId,pathNodes[currentNodeIndex].worldPosition);
                }
                else
                {
                    OnPostMoveEnd?.Invoke();
                }
            }
            moveVelocity.SetVelocity(moveDir);
        }
    }
    

    public void OnDestroy()
    {
        OnPostMoveEnd = null;
    }
    
}
