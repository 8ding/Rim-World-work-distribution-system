
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
    public void OnEnter()
    {
//        if(parameter.animator != null)
            parameter.animator.Play("Idle");
    }

    public void OnUpdate()
    {
        timer += Time.deltaTime;
        if(timer >= parameter.idleTime)
        {
            manager.Transition(StateType.Patrol);
        }
    }

    public void OnExit()
    {
        timer = 0;
    }
}

public class PatrolState : IState
{
    private FSM manager;
    private Parameter parameter;

    private int patrolPositon;
    public PatrolState(FSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }
    public void OnEnter()
    {
//        if(parameter.animator != null)
            parameter.animator.Play("WalkSide");
    }

    public void OnUpdate()
    {
        manager.FlipTo(parameter.patrolPoints[patrolPositon]);

        Vector2 moveDirection = parameter.patrolPoints[patrolPositon].position - manager.transform.position;
        if(Mathf.Abs(moveDirection.x) < 0.1f)
        {
            if(moveDirection.y > 0.1f)
                parameter.animator.Play("WalkUp");
            else if(moveDirection.y < -0.1f)
                parameter.animator.Play("WalkDown");
        }
        manager.transform.position = Vector2.MoveTowards(manager.transform.position, parameter.patrolPoints[patrolPositon].position,
            parameter.moveSpeed * Time.deltaTime);
        if(Vector2.Distance(manager.transform.position, parameter.patrolPoints[patrolPositon].position) < .1f)
        {
            manager.Transition(StateType.Idle);
        }
    }

    public void OnExit()
    {
        patrolPositon++;
        if(patrolPositon >= parameter.patrolPoints.Length)
        {
            patrolPositon = 0;
        }
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

