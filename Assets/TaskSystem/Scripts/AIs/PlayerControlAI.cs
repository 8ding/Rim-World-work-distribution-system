using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlAI : AIBase
{

    private Vector3 direction = Vector3.zero;
    private Vector3 moveH = Vector3.zero;
    private Vector3 moveV = Vector3.zero;


    
    
    // Update is called once per frame
    void Update()
    {
        CheckIput();
        MoveBehave();
    }

    private void MoveBehave()
    {

    }

    private void CheckIput()
    {
        moveH = Vector3.zero;
        moveV = Vector3.zero;
        if(Input.GetAxisRaw("Horizontal") < -0.1f)
        {
            moveH = Vector3.left;
        }
        else if(Input.GetAxisRaw("Horizontal") > 0.1f)
        {
            moveH = Vector3.right;
        }
        if(Input.GetAxisRaw("Vertical") > 0.1f)
        {
            moveV = Vector3.up;
        }
        else if(Input.GetAxisRaw("Vertical") < -0.1f)
        {
            moveV = Vector3.down;
        }
        
        direction = (moveH + moveV).normalized;
        if(direction != Vector3.zero)
            Debug.Log(direction);
    }
    
}
