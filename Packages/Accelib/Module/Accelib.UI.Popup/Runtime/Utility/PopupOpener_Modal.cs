using Accelib.UI.Popup.Runtime.Data;
using Accelib.UI.Popup.Runtime.Layer.Base;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Accelib.UI.Popup.Runtime.Utility
{
    /// <summary>
    /// UnityEvent 기반 모달 열기 헬퍼.
    /// </summary>
    public class PopupOpener_Modal : MonoBehaviour
    {
        [Header("Text(Or Key)")]
        [SerializeReference] private SO_ModalOpenOption option;

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
