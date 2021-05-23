using UnityEngine;

public interface IMoveVelocity
{
    void SetVelocity(Vector3 velocityVector);
    void Disable();
    void Enable();
}
