using System;
using CodeMonkey;
using UnityEngine;

namespace TaskSystem.Character
{
    public class AnimationObjectController : MonoBehaviour
    {
        public int LoopTimes;
        public Action OnObjectAnimationEnd;
        public Action OnLoopOneTime;

        public void LoopOneTimeEnd()
        {
            LoopTimes--;
            OnLoopOneTime?.Invoke();
            if (LoopTimes <= 0)
            {
                OnObjectAnimationEnd?.Invoke();
            }
        }
        
        private void OnBecameInvisible()
        {
            Debug.Log("看不见了");
        }

        private void OnBecameVisible()
        {
            Debug.Log("又看见了");
        }
    }
}