

using StateMachine;
using UnityEngine;
using Pathfinding;
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
    private Path path;
    public float NextWayPointDistance = 0.1f;
    private int currentPoint = 0;

    private bool reachedEndOfPath = false;
    private Vector2 moveDirection;
    public MoveState(FSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }
    //寻路完成，传递路径点数据
    void OnPathComplete(Path _path)
    {
        if(!_path.error)
        {
            path = _path;
            currentPoint = 0;
        }
    }

    void switchAnim()
    {
        //移动时的播放动画选择与状态机枚举修改，角色的朝向需要传递给Idle状态
        if(Mathf.Abs(moveDirection.x) < Mathf.Abs(moveDirection.y))
        {
            if(moveDirection.y > 0f)
            {
                parameter.animator.Play("WalkUp");
                parameter.FaceDirection = FaceDirectionType.Up;
            }
            else if(moveDirection.y < 0f)
            {
                parameter.animator.Play("WalkDown");
                parameter.FaceDirection = FaceDirectionType.Down;
            }
        }
        else
        {
            parameter.animator.Play("WalkSide");
            parameter.FaceDirection = FaceDirectionType.Side;
        }
    }
    bool checkTransition()
    {
        if(path == null)
        {
            manager.Transition(StateType.Idle);
            return true;
        }
        if(currentPoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            manager.Transition(StateType.Idle);
            return true;
        }
        else
        {
            reachedEndOfPath = false;
            return false;
        }
    }
    private void handleMove()
    {
        moveDirection = (path.vectorPath[currentPoint] - manager.transform.position);
        //转向
        if(moveDirection.x > 0)
            manager.transform.localScale = new Vector3(-1, 1, 1);
        else if(moveDirection.x < 0)
            manager.transform.localScale = new Vector3(1,1,1);
        //向路径点移动
        manager.transform.position = Vector2.MoveTowards(manager.transform.position, path.vectorPath[currentPoint], parameter.moveSpeed * Time.deltaTime);

        float distance = Vector2.Distance(manager.transform.position, path.vectorPath[currentPoint]);
        //路径点索引增加，向下一个路径点移动
        if(distance < NextWayPointDistance)
        {
            currentPoint++;
        }
    }
    public void OnEnter()
    {
        parameter.seeker.StartPath(manager.transform.position, parameter.Target, OnPathComplete);
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
        currentPoint = 0;
    }
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

