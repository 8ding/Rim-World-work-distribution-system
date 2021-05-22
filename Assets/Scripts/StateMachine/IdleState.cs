

using System.Collections.Generic;
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
    private Vector3 lastPoint;
    private bool reachedEndOfPath = false;
    private Vector2 moveOffset;
    private Vector2 modifyDestination;
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
        //检测路径点有无能够卡住角色的地方,利用射线检测并挪动路径点
        for (int i = 0; i < path.vectorPath.Count; i++)
        {
            Vector3 origin = path.vectorPath[i];
            path.vectorPath[i] =  CheckPoint(path.vectorPath[i]);
            Vector3 end = path.vectorPath[i];
            Debug.DrawRay(origin, end - origin, Color.red, 1f);
        }
    }

    Vector3 CheckPoint(Vector3  point)
    {
        point.x = Mathf.Floor(point.x) + 0.5f;
        point.y = Mathf.Floor(point.y) + 0.01f;
        // RaycastHit2D rayUp;
        // if (rayUp = Physics2D.Raycast(point, Vector2.up, int.MaxValue,parameter.obstacle))
        // {
        //     
        //     if (rayUp.distance < parameter.Coll.bounds.size.y)
        //     {
        //         Debug.DrawRay(point, Vector3.up * rayUp.distance, Color.blue, 1f);
        //         point.y += (rayUp.distance - parameter.Coll.bounds.size.y - 0.1f);
        //     }
        // }
        
        // if (Physics2D.Raycast(point, Vector2.left, parameter.Coll.bounds.extents.x,
        //     parameter.obstacle))
        // {
        //     point.x += parameter.Coll.bounds.extents.x + 0.1f;
        // }
        
        // if (Physics2D.Raycast(point, Vector2.right, parameter.Coll.bounds.extents.x,
        //     parameter.obstacle))
        // {
        //     point.x -= parameter.Coll.bounds.extents.x + 0.1f;
        // }
        
        // if (Physics2D.Raycast(point, Vector2.left + Vector2.up, parameter.Coll.bounds.extents.magnitude,
        //     parameter.obstacle))
        // {
        //     point += new Vector3(parameter.Coll.bounds.extents.x, -1 * parameter.Coll.bounds.extents.y, 0f);
        // }
        // if (Physics2D.Raycast(point, Vector2.right + Vector2.up, parameter.Coll.bounds.extents.magnitude,
        //     parameter.obstacle))
        // {
        //     point += new Vector3(-1 * parameter.Coll.bounds.extents.x, -1 * parameter.Coll.bounds.extents.y, 0f);
        // }
        return point;
    }


    void switchAnim()
    {
        //移动时的播放动画选择与状态机枚举修改，角色的朝向需要传递给Idle状态
        if (currentPoint < path.vectorPath.Count)
        {
            if (path.vectorPath[currentPoint].x - lastPoint.x > 0f)
            {
                parameter.animator.Play("WalkSide");
                parameter.FaceDirection = FaceDirectionType.Side;
                manager.transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (path.vectorPath[currentPoint].x - lastPoint.x < 0f)
            {
                parameter.animator.Play("WalkSide");
                parameter.FaceDirection = FaceDirectionType.Side;
                manager.transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                if (path.vectorPath[currentPoint].y - lastPoint.y > 0f)
                {
                    parameter.animator.Play("WalkUp");
                    parameter.FaceDirection = FaceDirectionType.Up;
                }
                else if(path.vectorPath[currentPoint].y - lastPoint.y < 0f)
                {
                    parameter.animator.Play("WalkDown");
                    parameter.FaceDirection = FaceDirectionType.Down;
                }
            }
        }
    }
    bool checkTransition()
    {
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
        //向路径点移动
        manager.transform.position = Vector2.MoveTowards(manager.transform.position, path.vectorPath[currentPoint],
            parameter.moveSpeed * Time.deltaTime);
        float distance = Vector2.Distance(manager.transform.position, path.vectorPath[currentPoint]);
        //路径点索引增加，向下一个路径点移动
        if(distance < NextWayPointDistance)
        {
            manager.transform.position = path.vectorPath[currentPoint];
            lastPoint = manager.transform.position;
            currentPoint++;
        }
    }
    public void OnEnter()
    {
        parameter.Target = CheckPoint(parameter.Target);
        parameter.seeker.StartPath(manager.transform.position, parameter.Target, OnPathComplete);
        if(path == null)
        {
            manager.Transition(StateType.Idle);
            return;
        }

        lastPoint = manager.transform.position;
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

