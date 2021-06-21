using System;
using UnityEngine;


public class MoveTransformVelocity : MonoBehaviour,IMoveVelocity
{
    private Vector3 vectorVelocity;
    [SerializeField] private float moveSpeed;
    public void SetVelocity(Vector3 velocityVector)
    {
        this.vectorVelocity = velocityVector;
    }

    private void Update()
    {
        transform.position += vectorVelocity * moveSpeed * Time.deltaTime;
    }

    public void Disable()
    {
        this.vectorVelocity = Vector3.zero;
        this.enabled = false;
    }

    public void Enable()
    {
        this.enabled = true;
    }
}
