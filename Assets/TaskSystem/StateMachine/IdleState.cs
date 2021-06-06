

using System.Collections.Generic;
using System.Net;
using StateMachine;
using UnityEngine;

public class IdleState : IState
{
    private FSM manager;
    private Parameter parameter;

    private float timer;

    public IdleState(FSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }

    void switchAnim()
    {
        if(parameter.FaceDirection == FaceDirectionType.Up)
        {
            parameter.animator.Play("IdleUp");
        }
        else if(parameter.FaceDirection == FaceDirectionType.Down)
        {
            parameter.animator.Play("IdleDown");
        }
        else
        {
            parameter.animator.Play("IdleSide");
        }
    }
    public void OnEnter()
    {
        switchAnim();
    }

    public void OnUpdate()
    {
    }

    public void OnExit()
    {
        
    }
}




public class MoveState : IState
{
    private FSM manager;
    private Parameter parameter;

    public float NextWayPointDistance = 0.1f;

    private Vector3 lastPoint;
    private Vector2 moveOffset;
    private Vector2 modifyDestination;
    public MoveState(FSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }
    public void OnEnter()
    {
        
    }
    public void OnUpdate()
    {

            if(checkTransition())
                return;
            handleMove();
            switchAnim();
        
    }
    public void OnExit()
    {
        manager.GetComponent<IMoveVelocity>().SetVelocity(Vector3.zero);
    }
    /// <summary>
    /// 寻路完成的回调
    /// </summary>
    /// <param name="_path"></param>
    /// <summary>
    /// 移动路径点的方式
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    Vector3 CheckPoint(Vector3  point)
    {
        point.x = Mathf.Floor(point.x) + 0.5f;
        point.y = Mathf.Floor(point.y) + 0.01f;
        return point;
    }
    /// <summary>
    /// 根据路径点之间的方向切换动画
    /// </summary>
    void switchAnim()
    {
//        //移动时的播放动画选择与状态机枚举修改，角色的朝向需要传递给Idle状态
//        if (currentPoint < path.vectorPath.Count)
//        {
//            if (path.vectorPath[currentPoint].x - lastPoint.x > 0.1f)
//            {
//                parameter.animator.Play("WalkSide");
//                parameter.FaceDirection = FaceDirectionType.Side;
//                manager.transform.localScale = new Vector3(-1, 1, 1);
//            }
//            else if (path.vectorPath[currentPoint].x - lastPoint.x < -0.1f)
//            {
//                parameter.animator.Play("WalkSide");
//                parameter.FaceDirection = FaceDirectionType.Side;
//                manager.transform.localScale = new Vector3(1, 1, 1);
//            }
//            else
//            {
//                if (path.vectorPath[currentPoint].y - lastPoint.y > 0f)
//                {
//                    parameter.animator.Play("WalkUp");
//                    parameter.FaceDirection = FaceDirectionType.Up;
//                }
//                else if(path.vectorPath[currentPoint].y - lastPoint.y < 0f)
//                {
//                    parameter.animator.Play("WalkDown");
//                    parameter.FaceDirection = FaceDirectionType.Down;
//                }
//            }
//        }
    }
    /// <summary>
    /// 检测是否需要切换状态
    /// </summary>
    /// <returns></returns>
    bool checkTransition()
    {
//        if(currentPoint >= path.vectorPath.Count)
//        {
//            reachedEndOfPath = true;
//            manager.Transition(StateType.Idle);
//            return true;
//        }
//        else
//        {
//            reachedEndOfPath = false;
//            return false;
//        }
        return false;
    }
    /// <summary>
    /// 处理移动
    /// </summary>
    private void handleMove()
    {
//        //向路径点移动
//        Vector3 moveDir = (path.vectorPath[currentPoint] - manager.transform.position).normalized;
//        manager.GetComponent<IMoveVelocity>().SetVelocity(moveDir);
//        float distance = Vector2.Distance(manager.transform.position, path.vectorPath[currentPoint]);
//        //路径点索引增加，向下一个路径点移动
//        if(distance < NextWayPointDistance)
//        {
//            manager.transform.position = path.vectorPath[currentPoint];
//            lastPoint = manager.transform.position;
//            currentPoint++;
//        }
    }
    
}

public class WorkState : IState
{
    public void OnEnter()
    {
        
    }
    public void OnUpdate() { throw new System.NotImplementedException(); }
    public void OnExit() { throw new System.NotImplementedException(); }
}

public class ChaseState : IState
{
    private FSM manager;
    private Parameter parameter;

    public ChaseState(FSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }
    public void OnEnter()
    {
        
    }

    public void OnUpdate()
    {
        
    }

    public void OnExit()
    {
        
    }
}

public class ReactState : IState
{
    private FSM manager;
    private Parameter parameter;

    public ReactState(FSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }
    public void OnEnter()
    {
        
    }

    public void OnUpdate()
    {
        
    }

    public void OnExit()
    {
        
    }
}

public class AttackState : IState
{
    private FSM manager;
    private Parameter parameter;

    public AttackState(FSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }
    public void OnEnter()
    {
        
    }

    public void OnUpdate()
    {
        
    }

    public void OnExit()
    {
        
    }
}

