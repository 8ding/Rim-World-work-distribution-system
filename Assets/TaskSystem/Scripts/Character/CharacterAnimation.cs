using System;
using System.Collections;
using System.Collections.Generic;
using CodeMonkey;
using StateMachine;
using TaskSystem.Character;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    private FaceDirectionType faceDirectionType;

    private GameObject animationobject;
    //加event就只能在这个类内被调用被赋值,封闭安全
    public  Action OnAnimationEnd;
    public Action OneTimeAction;
    public int LoopTimes;
    private Dictionary<FaceDirectionType, GameObject> faceDirectionGameObjectDictionary;

    private Dictionary<ObjectAnimationType, Dictionary<FaceDirectionType, GameObject>>
        animationTypDirectionDictionaryDictionary;
    void Awake()
    {
        animationTypDirectionDictionaryDictionary =
            new Dictionary<ObjectAnimationType, Dictionary<FaceDirectionType, GameObject>>();
        faceDirectionGameObjectDictionary = new Dictionary<FaceDirectionType, GameObject>();
        faceDirectionType = FaceDirectionType.Side;
        LoopTimes = 0;
    }
    private void CreateobjectAnimaiton(ObjectAnimationType objectAnimationType)
    {
        if(animationobject != null && animationobject.activeSelf)
            animationobject.SetActive(false);
        if (!animationTypDirectionDictionaryDictionary.TryGetValue(objectAnimationType,out faceDirectionGameObjectDictionary))
        {
            animationobject =
                GameAssets.Instance.createAnimationGameObject(objectAnimationType,faceDirectionType, gameObject.transform, transform.position);
            faceDirectionGameObjectDictionary = new Dictionary<FaceDirectionType, GameObject>
                {{faceDirectionType, animationobject}};
            animationTypDirectionDictionaryDictionary[objectAnimationType] = faceDirectionGameObjectDictionary;
        }
        else if(!faceDirectionGameObjectDictionary.TryGetValue(faceDirectionType, out animationobject))
        {
            animationobject =
                GameAssets.Instance.createAnimationGameObject(objectAnimationType,faceDirectionType, gameObject.transform, transform.position);
            animationTypDirectionDictionaryDictionary[objectAnimationType][faceDirectionType] = animationobject;
        }

        animationobject.SetActive(true);
        AnimationObjectController animationObjectController =
            animationobject.GetComponentInChildren<AnimationObjectController>();
        if (animationObjectController == null)
        {
            animationObjectController = animationobject.GetComponent<AnimationObjectController>();
        }
        animationObjectController.LoopTimes = LoopTimes;
        animationObjectController.OnLoopOneTime = OneTimeAction;
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


    public void PlayMineAnimation(int looptimes,Action onetimeAction)
    {
        this.LoopTimes = looptimes;
        this.OneTimeAction = onetimeAction;
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
