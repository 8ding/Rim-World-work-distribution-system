using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : BaseManager<InputManager>
{
    private bool isStart = false;
    public InputManager()
    {
        MonoMgr.Instance.AddUpdateListener(Update);
    }

    public void StartOrEnd(bool isOpen)
    {
        isStart = isOpen;
    }
    private void checkKeyCode(KeyCode keyCode)
    {
        if(Input.GetKeyDown(keyCode))
        {
            EventCenter.Instance.EventTrigger<IArgs>("GetSomeKeyDown", new EventParameter<KeyCode>(keyCode));
        }
        if(Input.GetKeyUp(keyCode))
        {
            EventCenter.Instance.EventTrigger<IArgs>("GetSomeKeyUp", new EventParameter<KeyCode>(keyCode));
        }
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
    }
}
