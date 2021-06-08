using System;
using CodeMonkey;
using UnityEngine;

namespace TaskSystem.Character
{
    public class AnimationObjectController : MonoBehaviour
    {
        public int LoopTimes;
        public event Action OnObjectAnimationEnd;
        

        public void LoopOneTimeEnd()
        {
            LoopTimes--;
            if (LoopTimes <= 0)
            {
                OnObjectAnimationEnd?.Invoke();
                GameObject.Destroy(gameObject.transform.parent.gameObject);
            }
        }

    }
}