using System;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

namespace StateMachine
{
    public enum StateType
    {
        Idle,
        Move,
        Chase,
        React,
        Attack,
    }

    public enum FaceDirectionType
    {
        Side,
        Up,
        Down,

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
        public int patrolPositon = -1;
        public Animator animator;

        public FaceDirectionType FaceDirection = FaceDirectionType.Side;
        public Seeker seeker;
        public Vector3 Target;
    }
    public class FSM : MonoBehaviour
    {
        private IState currentState;

        public Parameter parameter;

        private InputCheck.InputCheck inputTest;
        //状态类型与状态对应的键值对
        private Dictionary<StateType,IState> states = new Dictionary<StateType, IState>();
        private void Start()
        {
            states.Add(StateType.Idle,new IdleState(this));
            states.Add(StateType.Move,new MoveState(this));
            states.Add(StateType.Attack, new AttackState(this));

            states.Add(StateType.Chase, new ChaseState(this));

            states.Add(StateType.React, new ReactState(this));
            
            parameter.animator = GetComponent<Animator>();
            parameter.seeker = GetComponent<Seeker>();
            
            inputTest = Camera.main.GetComponent<InputCheck.InputCheck>();
            inputTest.OnMouseClick += handleMouseClick;
            Transition(StateType.Idle);
        }

        private void Update()
        {
            currentState.OnUpdate();
        }

        private void handleMouseClick(Vector3 _target)
        {
            parameter.Target = _target;
            Transition(StateType.Move);
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

//        public void FlipTo(Transform _transform)
//        {
//            if(_transform != null)
//            {
//                if(_transform.position.x > transform.position.x)
//                {
//                    transform.localScale = new Vector3(-1, 1, 1);
//                }
//                else if(_transform.position.x < transform.position.x)
//                {
//                    transform.localScale = new Vector3(1, 1, 1);
//                }
//            }
//        }
    }
}