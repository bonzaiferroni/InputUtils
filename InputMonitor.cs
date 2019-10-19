using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Bonwerk.InputUtils
{
    public class InputMonitor : MonoBehaviour
    {
        public static bool IsDragging { get; private set; }
        public static bool InputFieldActive { get; private set; }
        public static bool OverUi { get; private set; }
        public static bool Shift { get; private set; }
        public static bool Control { get; private set; }
        public static bool Alt { get; private set; }
        public static bool LockMouse { get; private set; }

        private bool _isDragOriginUi;
        private Vector3 _clickOrigin;
        private bool _isMouseDown0;
        private bool _isMouseDown1;
        private float _dragSensitivity = 5;
        private bool _endDragging;

        private void Update()
        {
            if (!EventSystem.current)
                throw new Exception("InputMonitor expecting an EventSystem in the scene");
            
            ControlMouseLock();
            
            CheckDragging();
            InputFieldActive = CheckInput();
            OverUi = CheckOverUi();

            MonitorModifiers();
            MonitorClick();
        }

        private void MonitorModifiers()
        {
            Shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            Control = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            Alt = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
        }

        private void CheckDragging()
        {
            if (_isMouseDown0 || _isMouseDown1)
            {
                if (!IsDragging && Vector3.Distance(_clickOrigin, Input.mousePosition) > _dragSensitivity)
                {
                    IsDragging = true;
                }
            }
            else
            {
                if (_endDragging)
                {
                    // ensures it will end on the next frame after release
                    _endDragging = false;
                    IsDragging = false;
                }

                if (IsDragging)
                {
                    _endDragging = true;
                }
            }
        }

        private bool CheckInput()
        {
            if (EventSystem.current.currentSelectedGameObject)
            {
                var input = EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>();
                if (input && input.isFocused)
                    return true;
            }

            return false;
        }

        private bool CheckOverUi()
        {
            if (IsDragging)
            {
                return _isDragOriginUi;
            }

            return EventSystem.current.IsPointerOverGameObject();
        }

        private void MonitorClick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _clickOrigin = Input.mousePosition;
                _isMouseDown0 = true;
                _isDragOriginUi = EventSystem.current.IsPointerOverGameObject();
            }

            if (Input.GetMouseButtonUp(0))
            {
                _isMouseDown0 = false;
            }

            if (Input.GetMouseButtonDown(1))
            {
                _clickOrigin = Input.mousePosition;
                _isMouseDown1 = true;
                _isDragOriginUi = EventSystem.current.IsPointerOverGameObject();
            }

            if (Input.GetMouseButtonUp(1))
            {
                _isMouseDown1 = false;
            }
        }

        private void ControlMouseLock()
        {
            if (InputFieldActive || !Input.GetKeyDown(KeyCode.Y)) return;
            
            LockMouse = !LockMouse;
            Cursor.lockState = LockMouse ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
}