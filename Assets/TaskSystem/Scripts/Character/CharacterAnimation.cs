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
    public Action OnAnimationEnd;
    public Action OneTimeAction;
    
    private Dictionary<FaceDirectionType, GameObject> faceDirectionGameObjectDictionary;

    private Dictionary<ObjectAnimationType, Dictionary<FaceDirectionType, GameObject>>
        animationTypDirectionDictionaryDictionary;
    void Awake()
    {
        animationTypDirectionDictionaryDictionary =
            new Dictionary<ObjectAnimationType, Dictionary<FaceDirectionType, GameObject>>();
        faceDirectionGameObjectDictionary = new Dictionary<FaceDirectionType, GameObject>();
        faceDirectionType = FaceDirectionType.Side;
    
    }
    public void PlayobjectAnimaiton(int loopTimes,ObjectAnimationType objectAnimationType)
    {

        GameObject temp;
        if (!animationTypDirectionDictionaryDictionary.TryGetValue(objectAnimationType,out faceDirectionGameObjectDictionary))
        {
            temp =
                GameAssets.Instance.createAnimationGameObject(objectAnimationType,faceDirectionType, gameObject.transform, transform.position);
            faceDirectionGameObjectDictionary = new Dictionary<FaceDirectionType, GameObject>
                {{faceDirectionType, temp}};
            animationTypDirectionDictionaryDictionary[objectAnimationType] = faceDirectionGameObjectDictionary;
        }
        else if(!faceDirectionGameObjectDictionary.TryGetValue(faceDirectionType, out temp))
        {
            temp =
                GameAssets.Instance.createAnimationGameObject(objectAnimationType,faceDirectionType, gameObject.transform, transform.position);
            animationTypDirectionDictionaryDictionary[objectAnimationType][faceDirectionType] = temp;
        }

        if (animationobject != temp)
        {
            if(animationobject != null) 
                animationobject.SetActive(false);
            animationobject = temp;
            animationobject.SetActive(true);
        }
        AnimationObjectController animationObjectController =
            animationobject.GetComponentInChildren<AnimationObjectController>();
        if (animationObjectController == null)
        {
            animationObjectController = animationobject.GetComponent<AnimationObjectController>();
        }
        animationObjectController.LoopTimes = loopTimes;
        animationObjectController.OnLoopOneTime = OneTimeAction;
        animationObjectController.OnObjectAnimationEnd = handleObjectAnimationEnd;
    }

    public void PlayMoveAnimation(Vector3 position)
    {
        
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
        PlayobjectAnimaiton(0,ObjectAnimationType.Walk);
     }

    // public void PlayIdleAnimation()
    // {
    //     PlayobjectAnimaiton(ObjectAnimationType.Idle);
    // }
    //
    // public void PlayVictoryAnimation()
    // {
    //     PlayobjectAnimaiton(ObjectAnimationType.Throw);
    // }
    //
    // public void PlayCleanAnimation(int looptimes)
    // {
    //     this.LoopTimes = looptimes;
    //     PlayobjectAnimaiton(ObjectAnimationType.Clean);
    // }
    //
    //
    // public void PlayMineAnimation(int looptimes,Action onetimeAction)
    // {
    //     this.LoopTimes = looptimes;
    //     this.OneTimeAction = onetimeAction;
    //     PlayobjectAnimaiton(ObjectAnimationType.Mine);
    // }
    //
    // public void PlayCutAnimation(int looptimes)
    // {
    //     this.LoopTimes = looptimes;
    //     PlayobjectAnimaiton(ObjectAnimationType.Cut);
    // }
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
