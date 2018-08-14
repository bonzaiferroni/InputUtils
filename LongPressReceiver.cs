using System;
using UnityEngine;

namespace InputUtils
{
    public class LongPressReceiver : MonoBehaviour
    {
        public event Action OnLongPress;
        
        public void SendEvent()
        {
            OnLongPress?.Invoke();
        }
    }
}