using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector2 moveInput;
    private Vector2 mousePosition;
    [SerializeField] private float panSpeed;
    [SerializeField] private float scrollSpeed;

    void Update()
    {
        hanldeInput();
    }

    private void hanldeInput()
    {
        Vector3 pos = transform.position;
        moveInput.Set(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        // mousePosition = Input.mousePosition;
        // if (mousePosition.x > Screen.width * 0.9f && mousePosition.x < Screen.width)
        // {
        //     moveInput.x = 1;
        // }
        //
        // if (mousePosition.x < Screen.width * 0.1f && mousePosition.x > 0f)
        // {
        //     moveInput.x = -1;
        // }
        //
        // if (mousePosition.y > Screen.height * 0.9f && mousePosition.y < Screen.height)
        // {
        //     moveInput.y = 1;
        // }
        //
        // if (mousePosition.y < Screen.height * 0.1f && mousePosition.y > 0f)
        // {
        //     moveInput.y = -1;
        // }
        pos.x += moveInput.normalized.x * panSpeed * Time.deltaTime;
        pos.y += moveInput.normalized.y * panSpeed * Time.deltaTime;
        pos.z += Input.GetAxisRaw("Mouse ScrollWheel") * scrollSpeed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, -5, 10);
        pos.y = Mathf.Clamp(pos.y, 0, 10);
        pos.z = Mathf.Clamp(pos.z, -13, -5);
        transform.position = pos;
    }
}
