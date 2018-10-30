using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InputUtils
{
    public class LongPressMonitor : MonoBehaviour
    {
        [SerializeField] private float _chargeLength = 1f;
        [SerializeField] private bool _interceptRightClick;
        
        private Image _indicator;
        private LongPressReceiver _charging;
        private List<RaycastResult> _results = new List<RaycastResult>();
        private float _beginTime;
        private float _chargedTime;
        private float _delay = .5f;

        public static LongPressState State { get; private set; }

        private void Start()
        {
            _indicator = GetComponent<Image>();
            BeginInactive();
        }

        private void Update()
        {
            ManageState();
            RightClick();
        }

        private void RightClick()
        {
            if (!_interceptRightClick || !Input.GetMouseButtonUp(1)) return;
            _charging = FindLongPress();
            if (_charging) BeginCharged();
        }

        private void ManageState()
        {
            switch (State)
            {
                case LongPressState.Inactive:
                    ManageInactive();
                    break;
                case LongPressState.Charging:
                    ManageCharging();
                    break;
                case LongPressState.Charged:
                    ManageCharged();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ManageInactive()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _charging = FindLongPress();
                if (_charging) BeginCharging();
            }
        }

        private void ManageCharging()
        {
            if (!Input.GetMouseButton(0) || InputMonitor.IsDragging)
            {
                BeginInactive();
                return;
            }
            
            var progress = Mathf.Clamp((Time.time - _beginTime - _delay) / _chargeLength, 0, 1);
            
            if (progress >= 1)
            {
                BeginCharged();
                return;
            }
            
            _indicator.color = new Color(1, 1, 1, progress);
            _indicator.fillAmount = progress;
        }

        private void ManageCharged()
        {
            if (!Input.GetMouseButton(0) || InputMonitor.IsDragging)
            {
                BeginInactive();
                return;
            }

            var progress = Mathf.Clamp(1 - (Time.time - _chargedTime) * 4, 0, 1);
            
            _indicator.color = new Color(1, 1, 1, progress);
        }

        private void BeginInactive()
        {
            State = LongPressState.Inactive;
            _charging = null;
            _indicator.color = new Color(1, 1, 1, 0);
            _indicator.fillAmount = 0;
        }

        private void BeginCharging()
        {
            State = LongPressState.Charging;
            _beginTime = Time.time;
            transform.position = Input.mousePosition;
        }

        private void BeginCharged()
        {
            State = LongPressState.Charged;
            _charging.SendEvent();
            _chargedTime = Time.time;
        }

        private LongPressReceiver FindLongPress()
        {
            PointerEventData ped = new PointerEventData(null);
            ped.position = Input.mousePosition;
            _results.Clear();
            EventSystem.current.RaycastAll(ped, _results);
            foreach (var result in _results)
            {
                var receiver = result.gameObject.GetComponent<LongPressReceiver>();
                if (receiver && receiver.Interactable) return receiver;
            }

            return null;
        }
    }

    public enum LongPressState
    {
        Inactive,
        Charging,
        Charged,
    }
}