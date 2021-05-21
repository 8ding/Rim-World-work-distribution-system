using System;
using UnityEngine;

namespace InputCheck
{
    public class InputCheck : MonoBehaviour
    {
        public event Action<Vector3> OnMouseClick;
        private Camera mainCamera;
        private void Start()
        {
            mainCamera = GetComponent<Camera>();
        }

        private void Update()
        {
            if(Input.GetMouseButtonDown(1))
            {
                Vector3 mouseWorldPosition =
                    mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0 - mainCamera.transform.position.z));
                OnMouseClick?.Invoke(mouseWorldPosition);
            }
        }
    }
}