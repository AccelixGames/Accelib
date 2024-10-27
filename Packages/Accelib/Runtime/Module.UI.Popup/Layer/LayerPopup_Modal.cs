using Accelib.Module.Localization.Helper;
using Accelib.Module.UI.Popup.Data;
using Accelib.Module.UI.Popup.Layer.Base;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Accelib.Module.UI.Popup.Layer
{
    public sealed class LayerPopup_Modal : LayerPopupBase
    {
        [SerializeField] private TMP_Text titleTMP;
        [SerializeField] private TMP_Text descTMP;
        [SerializeField] private TMP_Text okButton;
        [SerializeField] private TMP_Text ngButton;

        public enum Result {OK = 0, NG = 1, Exception = -1}
        
        private UniTaskCompletionSource<Result> _ucs;
        
        private void OnDisable() => _ucs?.TrySetResult(Result.Exception);
        
        internal UniTask<Result> Open(ModalOpenOption option)
        {
            gameObject.SetActive(true);
            
            SetText(titleTMP, option.title, option.useLocale);
            SetText(descTMP, option.desc, option.useLocale);
            SetText(okButton, option.ok, option.useLocale);
            SetText(ngButton, option.ng, option.useLocale);
            
            _ucs?.TrySetResult(Result.Exception);
            _ucs = new UniTaskCompletionSource<Result>();
            _ucs.Task.AttachExternalCancellation(this.GetCancellationTokenOnDestroy());
            return _ucs.Task;
        }

        private void SetText(TMP_Text target, string text, bool useLocale)
        {
            if(useLocale)
                target.GetComponent<LocalizedTMP>()?.ChangeKey(text);
            else
                target.text = text;
        }
          
        [VisibleEnum(typeof(Result))]
        public void OnClickResult(int result)
        {
            _ucs?.TrySetResult((Result)result);
            PopupSingleton.Instance.CloseModal();
        }
    }
}