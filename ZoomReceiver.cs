using UnityEngine;

namespace Bonwerk.InputUtils
{
    public class ZoomReceiver : MonoBehaviour
    {
        public float Speed { get; private set; } = 20;
        public Transform Target;
        
        public void Zoom(float delta)
        {
            Target.localScale *= (1 + delta * Speed);
        }
    }
}