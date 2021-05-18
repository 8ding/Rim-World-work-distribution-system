using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    private float faceDirection;

    private Rigidbody2D _rigidbody2D;
    
    public float Speed;
    private Vector2 moveDirection;
    private Animator animator;

    private float moveH, moveV;
    // Start is called before the first frame update
    void Start()
    {
        faceDirection = 0f;

        animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        moveInput();
        switchAnimation();
    }

    private void FixedUpdate()
    {
        move();
    }

    //移动输入
    private void moveInput()
    {
        moveH = Input.GetAxis("Horizontal");
        moveV = Input.GetAxis("Vertical");
        moveDirection = new Vector2(moveH * Speed * Time.fixedDeltaTime, moveV * Speed * Time.fixedDeltaTime);
    }
    
    //人物动画
    private void switchAnimation()
    {
        //有移动才会对人物的朝向产生变化
        if(Mathf.Abs(_rigidbody2D.velocity.x) > 0.1f || Mathf.Abs(_rigidbody2D.velocity.y) > 0.1f)
        {
            if(_rigidbody2D.velocity.x < 0.1f)
            {
                transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
                faceDirection = 0f;
            
            }
            else if(_rigidbody2D.velocity.x > 0.1f)
            {
                transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
                faceDirection = 0f;
            }
            else if(_rigidbody2D.velocity.y > 0.1f)
                faceDirection = 1f;
            else if(_rigidbody2D.velocity.y < -0.1f)
            {
                faceDirection = 0.5f;
            }
            animator.SetFloat("FaceDirection",faceDirection);
            animator.SetBool("IsMove",true);
        }
        else
        {
            animator.SetBool("IsMove", false);
        }
    }

    private void move()
    {
        _rigidbody2D.velocity = moveDirection;
    }
}
