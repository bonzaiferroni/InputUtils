using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InputUtils
{
    public class ZoomMonitor : MonoBehaviour
    {
        private List<RaycastResult> _results = new List<RaycastResult>();

        private void Update()
        {
            CheckInput();
        }

        private void CheckInput()
        {
            var delta = Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime;
            if (delta == 0) return;
            var reciever = FindReceiver();
            if (reciever == null) return;
            reciever.Zoom(delta);
        }

        private ZoomReceiver FindReceiver()
        {
            PointerEventData ped = new PointerEventData(null);
            ped.position = Input.mousePosition;
            _results.Clear();
            EventSystem.current.RaycastAll(ped, _results);
            foreach (var result in _results)
            {
                var receiver = result.gameObject.GetComponent<ZoomReceiver>();
                if (receiver) return receiver;
            }

            return null;
        }
    }
}