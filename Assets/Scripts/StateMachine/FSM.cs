using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    public enum StateType
    {
        Idle,
        Patrol,
        Chase,
        React,
        Attack,
    }
    [Serializable]
    public class Parameter
    {
        public int health;
        public float moveSpeed;
        public float chaseSpee;
        public float idleTime;
        public Transform[] patrolPoints;
        public Transform[] chasePoints;

        public Animator animator;
    }
    public class FSM : MonoBehaviour
    {
        private IState currentState;

        public Parameter parameter;
        //状态类型与状态对应的键值对
        private Dictionary<StateType,IState> states = new Dictionary<StateType, IState>();
        private void Start()
        {
            states.Add(StateType.Idle,new IdleState(this));
            states.Add(StateType.Attack, new AttackState(this));
            states.Add(StateType.Chase, new ChaseState(this));
            states.Add(StateType.Patrol, new PatrolState(this));
            states.Add(StateType.React, new ReactState(this));
            
            parameter.animator = GetComponent<Animator>();
            
            Transition(StateType.Idle);
        }

        private void Update()
        {
            currentState.OnUpdate();
        }

        public void Transition(StateType _type)
        {
            //退出前一个状态
            if(currentState != null)
            {
                currentState.OnExit();
            }
            //取出传入的状态
            currentState = states[_type];
            //进入新的状态
            currentState.OnEnter();
        }

        public void FlipTo(Transform _transform)
        {
            if(_transform != null)
            {
                if(_transform.position.x > transform.position.x)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                }
                else if(_transform.position.x < transform.position.x)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }
            }
        }
    }
}