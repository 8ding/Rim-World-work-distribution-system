
using System;
using UnityEngine;

public interface IMovePosition
{
     void SetMovePosition(Vector3 movePosition,Action onArrive = null);
}
