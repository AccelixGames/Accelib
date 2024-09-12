using Accelib.UI.Data;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Accelib.UI.Popup
{
    public class ModalPopup : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleTMP;
        [SerializeField] private TMP_Text descTMP;
        [SerializeField] private TMP_Text okButton;
        [SerializeField] private TMP_Text ngButton;

        public enum Result {OK = 0, NG = 1, Exception = -1}
        
        private UniTaskCompletionSource<Result> _ucs;
        
        public UniTask<Result> Open(string title, string desc, string okText = "네", string ngText = "아니요")
        {
            titleTMP.text = title;
            descTMP.text = desc;
            okButton.text = okText;
            ngButton.text = ngText;
            
            OnOpen();
            
            _ucs?.TrySetResult(Result.Exception);
            _ucs = new UniTaskCompletionSource<Result>();
            _ucs.Task.AttachExternalCancellation(this.GetCancellationTokenOnDestroy());
            return _ucs.Task;
        }

        public virtual bool IsEnabled() => gameObject.activeSelf;
        protected virtual void OnOpen() => gameObject.SetActive(true);
        protected virtual void OnClose() => gameObject.SetActive(false);
        
        public void OnClickResult(int result)
        {
            _ucs?.TrySetResult((Result)result);
            
            OnClose();
        }
        
        private void OnDisable() => _ucs?.TrySetResult(Result.Exception);
    }
}