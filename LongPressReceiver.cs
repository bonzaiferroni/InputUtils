using System;
using UnityEngine;

namespace Bonwerk.InputUtils
{
    public class LongPressReceiver : MonoBehaviour
    {
        public event Action OnLongPress;

        public bool Interactable { get; set; } = true;
        
        public void SendEvent()
        {
            OnLongPress?.Invoke();
        }
    }
}