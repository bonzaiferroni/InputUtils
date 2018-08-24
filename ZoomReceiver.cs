using UnityEngine;

namespace InputUtils
{
    public class ZoomReceiver : MonoBehaviour
    {
        public float Speed { get; private set; } = 10;
        public Transform Target;
        
        public void Zoom(float delta)
        {
            Target.localScale *= (1 + delta * Speed);
        }
    }
}