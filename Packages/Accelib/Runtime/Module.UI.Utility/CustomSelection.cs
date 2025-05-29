using UnityEngine;
using UnityEngine.EventSystems;

namespace Accelib.Module.UI.Utility
{
    public class CustomSelection : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        [SerializeField] private GameObject selectTarget;
        
        public void OnSelect(BaseEventData eventData) => selectTarget.SetActive(true);

        public void OnDeselect(BaseEventData eventData) => selectTarget.SetActive(false);
    }
}