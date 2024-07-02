using UnityEngine;
using UnityEngine.Events;

namespace Accelib.Utility
{
    public class AnimationEventHelper : MonoBehaviour
    {
        public UnityEvent[] events;
        
        public void EventCall(int id)
        {
            if(id >= 0 && id < events.Length)
                events[id]?.Invoke();
        }
    }
}