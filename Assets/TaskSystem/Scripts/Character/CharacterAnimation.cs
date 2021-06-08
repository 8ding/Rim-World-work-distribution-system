using System;
using System.Collections;
using System.Collections.Generic;
using CodeMonkey;
using StateMachine;
using TaskSystem.Character;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    private Animator animator;
    private FaceDirectionType faceDirectionType;
    //加event就只能在这个类内被调用被赋值,封闭安全
    public  Action OnAnimationEnd;
    public int LoopTimes;

    void Start()
    {
        animator = GetComponent<Animator>();
        faceDirectionType = FaceDirectionType.Side;
        LoopTimes = 0;
    }
    private void CreateobjectAnimaiton(ObjectAnimationType objectAnimationType)
    {
        GameObject animationobject =
            GameAssets.Instance.createAnimationGameObject(objectAnimationType, null, transform.position);
        AnimationObjectController animationObjectController =
            animationobject.GetComponentInChildren<AnimationObjectController>();
        animationObjectController.LoopTimes = LoopTimes;
        animationObjectController.OnObjectAnimationEnd += Enable;
        animationObjectController.OnObjectAnimationEnd += handleObjectAnimationEnd;
        gameObject.SetActive(false);
    }

    public void PlayDirectMoveAnimation(Vector3 Position)
    {
        if (Position.x - transform.position.x > 0.1f)
        {
            animator.Play("walk_side");
            faceDirectionType = FaceDirectionType.Side;
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (Position.x - transform.position.x  < -0.1f)
        {
            animator.Play("walk_side");
            faceDirectionType = FaceDirectionType.Side;
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            if (Position.y - transform.position.y > 0f)
            {
                animator.Play("walk_up");
                faceDirectionType= FaceDirectionType.Up;
            }
            else if(Position.y - transform.position.y< 0f)
            {
                animator.Play("walk_down");
                faceDirectionType = FaceDirectionType.Down;
            }
        }
    }

    public void PlayIdleAnimation()
    {
        if(faceDirectionType == FaceDirectionType.Side)
        {
            animator.Play("idle_side");
        }
        else if(faceDirectionType == FaceDirectionType.Down)
        {
            animator.Play("idle_down");
        }
        else
        {
            animator.Play("idle_up");
        }
    }

    public void PlayVictoryAnimation()
    {
        if(faceDirectionType == FaceDirectionType.Side)
        {
            animator.Play("throw_side");
        }
        else if(faceDirectionType == FaceDirectionType.Down)
        {
            animator.Play("throw_down");
        }
        else
        {
            animator.Play("throw_up");
        }
    }

    public void PlayCleanAnimation(int looptimes)
    {
        this.LoopTimes = looptimes;
        CreateobjectAnimaiton(ObjectAnimationType.CleanUP);
    }


    public void PlayMineAnimation(int looptimes)
    {
        this.LoopTimes = looptimes;
        CreateobjectAnimaiton(ObjectAnimationType.Mine);
    }
    public void VictoryEnd()
    {
        OnAnimationEnd?.Invoke();
    }

    private void handleObjectAnimationEnd()
    {
        OnAnimationEnd?.Invoke();
    }
    public void Enable()
    {
        gameObject.SetActive(true);
    }
}
