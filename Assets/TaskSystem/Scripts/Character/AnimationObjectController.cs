using System;
using UnityEngine;

namespace TaskSystem.Character
{
    public class AnimationObjectController : MonoBehaviour
    {
        public event Action OnAnimationEnd;

        public void AnimationEnd()
        {
            OnAnimationEnd?.Invoke();
            Animator animator;
            Destroy(transform.parent.gameObject);
        }
    }
}