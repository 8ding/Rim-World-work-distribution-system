
using System;
using UnityEngine;

public class MoveVelocity : MonoBehaviour,IMoveVelocity
{
    [SerializeField] private float moveSpeed;
    private Rigidbody2D rb;
    private Vector3 velocityVector;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetVelocity(Vector3 _velocityVector)
    {
        this.velocityVector = _velocityVector;
    }

    private void FixedUpdate()
    {
        rb.velocity = velocityVector * moveSpeed;
    }

    public void Disable()
    {
        rb.velocity = Vector2.zero;
        this.enabled = false;
    }

    public void Enable()
    {
        this.enabled = true;
    }
}
