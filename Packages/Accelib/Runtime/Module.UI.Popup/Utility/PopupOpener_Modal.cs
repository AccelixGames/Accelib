using Accelib.Module.UI.Popup.Data;
using Accelib.Module.UI.Popup.Layer;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Accelib.Module.UI.Popup.Utility
{
    public class PopupOpener_Modal : MonoBehaviour
    {
        [Header("Text(Or Key)")]
        [SerializeField] private ModalOpenOption option;
        
        [Header("Events")]
        public UnityEvent onOK;
        public UnityEvent onNG;
        
        [Button]
        public async void Open()
        {
            var result = await PopupSingleton.Instance.OpenModal(option);
            
            if (result == LayerPopup_Modal.Result.OK) 
                onOK?.Invoke();
            else if(result == LayerPopup_Modal.Result.NG)
                onNG?.Invoke();
        }
    }
}