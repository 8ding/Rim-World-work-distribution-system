using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : BaseManager<InputManager>
{
    private bool isStart = false;
    private Vector3 AxisMove;
    private Vector3 AxisMoveH;
    private Vector3 AxisMoveV;
    public InputManager()
    {
        MonoMgr.Instance.AddUpdateListener(Update);
        AxisMove = Vector3.zero;
        AxisMoveH = Vector3.zero;
        AxisMoveV = Vector3.zero;
    }

    public void StartOrEnd(bool isOpen)
    {
        isStart = isOpen;
    }
    private void checkKeyCode(KeyCode keyCode)
    {
        if(Input.GetKeyDown(keyCode))
        {
            EventCenter.Instance.EventTrigger<IArgs>(EventType.GetSomeKeyDown, new EventParameter<KeyCode>(keyCode));
        }
        if(Input.GetKeyUp(keyCode))
        {
            EventCenter.Instance.EventTrigger<IArgs>(EventType.GetSomeKeyUp, new EventParameter<KeyCode>(keyCode));
        }
    }

    private void checkAxis()
    {
        AxisMoveH = Input.GetAxisRaw("Horizontal") * Vector3.right;
        AxisMoveV = Input.GetAxisRaw("Vertical") * Vector3.up;
        AxisMove = AxisMoveH + AxisMoveV;
        EventCenter.Instance.EventTrigger<IArgs>(EventType.GetAxis, new EventParameter<Vector3>(AxisMove));
    }
    private void Update()
    {
        if(!isStart)
        {
            return;
        }
        checkKeyCode(KeyCode.W);
        checkKeyCode(KeyCode.A);
        checkKeyCode(KeyCode.S);
        checkKeyCode(KeyCode.D);
        checkKeyCode(KeyCode.Space);
        checkKeyCode(KeyCode.Tab);
        checkKeyCode(KeyCode.Q);
        checkAxis();
        
    }
}
