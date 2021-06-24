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
    public void PlayobjectAnimaiton(int id,int loopTimes,ObjectAnimationType objectAnimationType)
    {
        string path = "animation/" + id + "_" + objectAnimationType + "_" + faceDirectionType;
        if(animationobject == null || animationobject.name != path)
        {
            if(animationobject != null)
            {
                PoolMgr.Instance.PushObj(animationobject);
                animationobject.transform.localScale = Vector3.one;
            }
            PoolMgr.Instance.GetObj(path,(_o =>
            {
                animationobject = _o;
                animationobject.transform.SetParent(gameObject.transform);
                animationobject.transform.localPosition = Vector3.zero;
                animationobject.transform.localScale = Vector3.one;
                AnimationObjectController animationObjectController =
                    animationobject.GetComponentInChildren<AnimationObjectController>();
                if (animationObjectController == null)
                {
                    animationObjectController = animationobject.GetComponent<AnimationObjectController>();
                }
                animationObjectController.LoopTimes = loopTimes;
                animationObjectController.OnLoopOneTime = OneTimeAction;
                animationObjectController.OnObjectAnimationEnd = handleObjectAnimationEnd;
            }));
        }
    }


    public void PlayDirectMoveAnimation(int id,Vector3 vector3,bool isPosition = true)
    {
        if(isPosition)
        {
            if (vector3.x - transform.position.x > 0.1f)
            {
                faceDirectionType = FaceDirectionType.Side;
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (vector3.x - transform.position.x  < -0.1f)
            {
                faceDirectionType = FaceDirectionType.Side;
                transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                if (vector3.y - transform.position.y > 0f)
                {
                    faceDirectionType= FaceDirectionType.Up;
                }
                else if(vector3.y - transform.position.y< 0f)
                {
                    faceDirectionType = FaceDirectionType.Down;
                }
            }
        }
        else
        {
            if(vector3.x > 0)
            {
                faceDirectionType = FaceDirectionType.Side;
                transform.localScale = new Vector3(-1, 1, 1);

            }
            else if(vector3.x < 0)
            {
                faceDirectionType = FaceDirectionType.Side;
                transform.localScale = new Vector3(1, 1, 1);
            }
            else 
            {
                if(vector3.y > 0.1f)
                    faceDirectionType = FaceDirectionType.Up;
                else
                {
                    faceDirectionType = FaceDirectionType.Down;
                }
            }
        }
        PlayobjectAnimaiton(id,0,ObjectAnimationType.Walk);
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
