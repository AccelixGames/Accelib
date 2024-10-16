using UnityEngine;
using UnityEngine.Events;

namespace Accelib.Extension.Atom
{
    public class BoolEventListener : MonoBehaviour
    {
        [SerializeField] private UnityEvent onTrue;
        [SerializeField] private UnityEvent onFalse;

        public void OnEventRaised(bool value)
        {
            if(value) onTrue?.Invoke();
            else onFalse?.Invoke();
        }
    }
}