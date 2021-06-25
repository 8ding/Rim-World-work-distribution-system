using System;
using CodeMonkey;
using UnityEngine;

namespace TaskSystem.Character
{
    public class AnimationObjectController : MonoBehaviour
    {
    
        public Action OnObjectAnimationEnd;
        public void LoopOneTimeEnd()
        {
            OnObjectAnimationEnd?.Invoke();
        }
    }
}