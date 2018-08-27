using System;
using UnityEngine;

namespace InputUtils
{
    public class LongPressReceiver : MonoBehaviour
    {
        public event Action OnLongPress;
        
        public bool Interactable { get; set; }
        
        public void SendEvent()
        {
            OnLongPress?.Invoke();
        }
    }
}