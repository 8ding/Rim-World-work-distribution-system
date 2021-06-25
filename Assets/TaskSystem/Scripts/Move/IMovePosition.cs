
using System;
using UnityEngine;

public interface IMovePosition
{
     void SetMovePosition(Vector3 movePosition);
     void BindOnPostMoveEnd(Action postMoveEnd);
     void Enable();
     void Disable();
     
}
