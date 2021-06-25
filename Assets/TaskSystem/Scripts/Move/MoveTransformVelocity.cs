using System;
using UnityEngine;


public class MoveTransformVelocity : MonoBehaviour,IMoveVelocity
{
    private Vector3 vectorVelocity;
    private UnitData unitData;

    private void Awake()
    {
        unitData = GetComponent<UnitData>();
    }

    public void SetVelocity(Vector3 velocityVector)
    {
        this.vectorVelocity = velocityVector;
    }

    private void Update()
    {
        transform.position += vectorVelocity * unitData.Speed * Time.deltaTime;
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
