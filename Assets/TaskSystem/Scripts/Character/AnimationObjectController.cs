using System;
using UnityEngine;

namespace TaskSystem.Character
{
    public class AnimationObjectController : MonoBehaviour
    {
        public event Action OnObjectAnimationEnd;

        public void AnimationEnd()
        {
            OnObjectAnimationEnd?.Invoke();
            Destroy(transform.parent.gameObject);
        }
    }
}