using System;
using System.Collections.Generic;
using Bonwerk.Injection;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Bonwerk.InputUtils
{
    public class InputMonitor : MonoBehaviour
    {
        public bool IsDragging { get; private set; }
        public bool InputFieldActive { get; private set; }
        public bool OverUi { get; private set; }
        public bool Shift { get; private set; }
        public bool Control { get; private set; }
        public bool Alt { get; private set; }
        public bool LockMouse { get; private set; }

        [SerializeField] private KeyCode _lockMouse = KeyCode.F1;
        
        private bool IsDragOriginUi;
        private Vector3 ClickOrigin;
        private bool IsMouseDown0;
        private bool IsMouseDown1;
        private float DragSensitivity = 5;
        private bool EndDragging;
        private List<InputListener> Listeners { get; } = new List<InputListener>();
        private bool MouseUp0;
        private bool MouseUp1;

        private void Awake()
        {
            Presto.Bind(this);
        }

        public void AddListener(IListenInput listener, int priority = int.MaxValue)
        {
            var newItem = new InputListener(listener, priority);
            for (int i = 0; i < Listeners.Count; i++)
            {
                var item = Listeners[i];
                if (priority > item.Priority) continue;
                Listeners.Insert(i, newItem);
                return;
            }
            Listeners.Add(newItem);
        }

        public bool ConsumeMouseUp(int mouseCode)
        {
            switch (mouseCode)
            {
                case 0 when !MouseUp0:
                    return false;
                case 0:
                    MouseUp0 = false;
                    return true;
                case 1 when !MouseUp1:
                    return false;
                case 1:
                    MouseUp1 = false;
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void Update()
        {
            if (!EventSystem.current)
                throw new Exception("InputMonitor expecting an EventSystem in the scene");

            ResetConsumedKeys();
            ControlMouseLock();
            
            CheckDragging();
            InputFieldActive = CheckInput();
            OverUi = CheckOverUi();

            MonitorModifiers();
            MonitorClick();

            UpdateListeners();
        }

        private void ResetConsumedKeys()
        {
            MouseUp0 = Input.GetMouseButtonUp(0);
            MouseUp1 = Input.GetMouseButtonUp(1);
        }

        private void MonitorModifiers()
        {
            Shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            Control = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            Alt = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
        }

        private void CheckDragging()
        {
            if (IsMouseDown0 || IsMouseDown1)
            {
                if (!IsDragging && Vector3.Distance(ClickOrigin, Input.mousePosition) > DragSensitivity)
                {
                    IsDragging = true;
                }
            }
            else
            {
                if (EndDragging)
                {
                    // ensures it will end on the next frame after release
                    EndDragging = false;
                    IsDragging = false;
                }

                if (IsDragging)
                {
                    EndDragging = true;
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
                return IsDragOriginUi;
            }

            return EventSystem.current.IsPointerOverGameObject();
        }

        private void MonitorClick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                ClickOrigin = Input.mousePosition;
                IsMouseDown0 = true;
                IsDragOriginUi = EventSystem.current.IsPointerOverGameObject();
            }

            if (Input.GetMouseButtonUp(0))
            {
                IsMouseDown0 = false;
            }

            if (Input.GetMouseButtonDown(1))
            {
                ClickOrigin = Input.mousePosition;
                IsMouseDown1 = true;
                IsDragOriginUi = EventSystem.current.IsPointerOverGameObject();
            }

            if (Input.GetMouseButtonUp(1))
            {
                IsMouseDown1 = false;
            }
        }

        private void ControlMouseLock()
        {
            if (InputFieldActive || !Input.GetKeyDown(_lockMouse)) return;
            
            LockMouse = !LockMouse;
            Cursor.lockState = LockMouse ? CursorLockMode.Locked : CursorLockMode.None;
        }

        private void UpdateListeners()
        {
            foreach (var item in Listeners)
            {
                item.Listener.UpdateInput(this); 
            }
        }

        #region Classes

        private class InputListener
        {
            public InputListener(IListenInput listener, int priority)
            {
                Listener = listener;
                Priority = priority;
            }

            public IListenInput Listener { get; }
            public int Priority { get; }
        }

        #endregion
    }
}