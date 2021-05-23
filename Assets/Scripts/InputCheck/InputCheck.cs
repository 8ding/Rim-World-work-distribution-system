using System;
using System.Collections.Generic;
using StateMachine;
using UnityEngine;

namespace InputCheck
{
    public class InputCheck : MonoBehaviour
    {
        public event Action<Vector3> OnMouseClick;
        private Camera mainCamera;
        private Vector3 startPosition;
        private Vector3 endPosition;
        private List<FSM> selectedFSMList;
        [SerializeField] private Transform selectedAreaTransform;

        private void Awake()
        {
            selectedFSMList = new List<FSM>();
            selectedAreaTransform.gameObject.SetActive(false);
        }

        private void Start()
        {
            mainCamera = GetComponent<Camera>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                 startPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0 - mainCamera.transform.position.z));
                 selectedAreaTransform.gameObject.SetActive(true);
            }

            if (Input.GetMouseButton(0))
            {
                Vector3 currentMousePosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0 - mainCamera.transform.position.z));
                Vector3 lowerLeft = new Vector3(Mathf.Min(startPosition.x, currentMousePosition.x),
                    Mathf.Min(startPosition.y, currentMousePosition.y));
                Vector3 rightUp = new Vector3(Mathf.Max(startPosition.x, currentMousePosition.x),
                    Mathf.Max(startPosition.y, currentMousePosition.y));
                selectedAreaTransform.position = lowerLeft;
                selectedAreaTransform.localScale = rightUp - lowerLeft;
            }
            if (Input.GetMouseButtonUp(0))
            {
                selectedAreaTransform.gameObject.SetActive(false);
                endPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0 - mainCamera.transform.position.z));
                Collider2D[] collider2Ds = Physics2D.OverlapAreaAll(startPosition, endPosition);

                foreach (var fsm in selectedFSMList)
                {
                    fsm.SetSelectedVisible(false);
                }
                selectedFSMList.Clear();
                
                foreach (var VARIABLE in collider2Ds)
                {
                    FSM unitFSM = VARIABLE.GetComponent<FSM>();
                    if (unitFSM != null)
                    {
                        selectedFSMList.Add(unitFSM);
                        unitFSM.SetSelectedVisible(true);
                    }
                }
            }
            if(Input.GetMouseButtonDown(1))
            {
                Vector3 mouseWorldPosition =
                    mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0 - mainCamera.transform.position.z));
                OnMouseClick?.Invoke(mouseWorldPosition);
            }
        }
    }
}