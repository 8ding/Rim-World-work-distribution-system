using System;
using System.Collections.Generic;

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
        enumcount
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
        public Collider2D Coll;
        public FaceDirectionType FaceDirection = FaceDirectionType.Side;

        public Vector3 Target;
        public LayerMask obstacle;
    }
    public class FSM : MonoBehaviour
    {
        private IState currentState;

        public Parameter parameter;
        private GameObject selecteGameObject;

        public bool IsEntered = false;//在切换状态时，有时enter中会存在需要时间完成的操作，比如现在的寻路过程，因此需要这些过程完成以后再执行onUpdate，此标记用于此目的
        // private InputCheck.InputCheck inputTest;
        //状态类型与状态对应的键值对
        private Dictionary<StateType,IState> states;

        private void Awake()
        {
            states = new Dictionary<StateType, IState>();
            selecteGameObject = transform.Find("Selected").gameObject;
            SetSelectedVisible(false);
        }

        private void Start()
        {
            states.Add(StateType.Idle,new IdleState(this));
            states.Add(StateType.Move,new MoveState(this));
            states.Add(StateType.Attack, new AttackState(this));

            states.Add(StateType.Chase, new ChaseState(this));

            states.Add(StateType.React, new ReactState(this));
            
            parameter.animator = GetComponent<Animator>();
            parameter.Coll = GetComponent<Collider2D>();
            

            // inputTest = Camera.main.GetComponent<InputCheck.InputCheck>();
            // inputTest.OnMouseClick += handleMouseClick;
            transform.position = new Vector3(Mathf.Floor(transform.position.x) + 0.5f,
                Mathf.Floor(transform.position.y) + 0.01f, 0f);
            Transition(StateType.Idle);
        }

        private void Update()
        {
            if(IsEntered)
                currentState.OnUpdate();
        }

        public void SetSelectedVisible(bool visible)
        {
            selecteGameObject.SetActive(visible);
        }
        private void handleMouseClick(Vector3 _target)
        {
            var rayUp = Physics2D.Raycast(_target, Vector2.up, 0.6f, parameter.obstacle);
            var rayDown = Physics2D.Raycast(_target, Vector2.down, 0.6f, parameter.obstacle);
            var rayLeft = Physics2D.Raycast(_target, Vector2.left, 0.6f, parameter.obstacle);
            var rayRight = Physics2D.Raycast(_target, Vector2.right, 0.6f, parameter.obstacle);


            if(!rayUp || !rayDown || !rayLeft || !rayRight)
            {
                parameter.Target = _target;
                Transition(StateType.Move);
            }
        }
        public void Transition(StateType _type)
        {

            //退出前一个状态
            if(currentState != null)
            {
                currentState.OnExit();
            }
            //此时旧状态已退出新状态还未进入完成
            IsEntered = false;
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