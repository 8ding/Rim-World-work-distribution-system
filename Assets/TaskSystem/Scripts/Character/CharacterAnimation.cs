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

    private GameObject animationobject;
    //加event就只能在这个类内被调用被赋值,封闭安全
    public  Action OnAnimationEnd;
    public int LoopTimes;

    void Start()
    {
        animator = GetComponent<Animator>();
        faceDirectionType = FaceDirectionType.Side;
        LoopTimes = 1;
    }
    private void CreateobjectAnimaiton(ObjectAnimationType objectAnimationType)
    { 
        if(animationobject != null)
            Destroy(animationobject);
        animationobject =
            GameAssets.Instance.createAnimationGameObject(objectAnimationType,faceDirectionType, gameObject.transform, transform.position);
        AnimationObjectController animationObjectController =
            animationobject.GetComponentInChildren<AnimationObjectController>();
        if (animationObjectController == null)
        {
            animationObjectController = GetComponent<AnimationObjectController>();
        }
        animationObjectController.LoopTimes = LoopTimes;
        animationObjectController.OnObjectAnimationEnd += handleObjectAnimationEnd;
    }

    public void PlayDirectMoveAnimation(Vector3 Position)
    {
        if (Position.x - transform.position.x > 0.1f)
        {
            faceDirectionType = FaceDirectionType.Side;
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (Position.x - transform.position.x  < -0.1f)
        {
            faceDirectionType = FaceDirectionType.Side;
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            if (Position.y - transform.position.y > 0f)
            {
                faceDirectionType= FaceDirectionType.Up;
            }
            else if(Position.y - transform.position.y< 0f)
            {
                faceDirectionType = FaceDirectionType.Down;
            }
        }
        CreateobjectAnimaiton(ObjectAnimationType.Walk);
    }

    public void PlayIdleAnimation()
    {
        // if(faceDirectionType == FaceDirectionType.Side)
        // {
        //     animator.Play("idle_side");
        // }
        // else if(faceDirectionType == FaceDirectionType.Down)
        // {
        //     animator.Play("idle_down");
        // }
        // else
        // {
        //     animator.Play("idle_up");
        // }
        CreateobjectAnimaiton(ObjectAnimationType.Idle);
    }

    public void PlayVictoryAnimation()
    {
        // if(faceDirectionType == FaceDirectionType.Side)
        // {
        //     animator.Play("throw_side");
        // }
        // else if(faceDirectionType == FaceDirectionType.Down)
        // {
        //     animator.Play("throw_down");
        // }
        // else
        // {
        //     animator.Play("throw_up");
        // }
        CreateobjectAnimaiton(ObjectAnimationType.Throw);
    }

    public void PlayCleanAnimation(int looptimes)
    {
        this.LoopTimes = looptimes;
        CreateobjectAnimaiton(ObjectAnimationType.Clean);
    }


    public void PlayMineAnimation(int looptimes)
    {
        this.LoopTimes = looptimes;
        CreateobjectAnimaiton(ObjectAnimationType.Mine);
    }

    public void PlayCutAnimation(int looptimes)
    {
        this.LoopTimes = looptimes;
        CreateobjectAnimaiton(ObjectAnimationType.Cut);
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
