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

    
    void Awake()
    {
        faceDirectionType = FaceDirectionType.Side;
    }
    public void PlayobjectAnimaiton(int id,ObjectAnimationType objectAnimationType,  Action OnPlayOneTime = null)
    {
        string path = "animation/" + id + "/" + id + "_" + objectAnimationType + "_" + faceDirectionType;

        if(animationobject == null || animationobject.name != path)
        {

            PoolMgr.Instance.GetObj(path,(_o =>
            {
                if(animationobject != null)
                {
                    PoolMgr.Instance.PushObj(animationobject);
                }
                _o.transform.SetParent(gameObject.transform); 
                _o.transform.localPosition = Vector3.zero;
                _o.transform.localScale = Vector3.one;

                animationobject = _o;
                AnimationObjectController animationObjectController =
                    animationobject.GetComponentInChildren<AnimationObjectController>();
                if (animationObjectController == null)
                {
                    animationObjectController = animationobject.GetComponent<AnimationObjectController>();
                }
                
                animationObjectController.OnObjectAnimationEnd = OnPlayOneTime;
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
        PlayobjectAnimaiton(id,ObjectAnimationType.Walk);
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

    
    public void Enable()
    {
        gameObject.SetActive(true);
    }
}
