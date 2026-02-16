using Accelib.Module.UI.Popup.Data;
using Accelib.Module.UI.Popup.Layer.Base;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Accelib.Module.UI.Popup.Layer
{
    /// <summary>
    /// 비동기 모달 다이얼로그의 추상 베이스 클래스.
    /// </summary>
    public abstract class LayerPopup_Modal : LayerPopupBase
    {
        public enum Result {OK = 0, NG = 1, Exception = -1}

        private UniTaskCompletionSource<Result> _ucs;

        private void OnDisable() => _ucs?.TrySetResult(Result.Exception);

        public UniTask<Result> Open(ModalOpenOption option)
        {
            gameObject.SetActive(true);

            // 서브클래스에서 텍스트 설정
            ApplyOption(option);

            _ucs?.TrySetResult(Result.Exception);
            _ucs = new UniTaskCompletionSource<Result>();
            _ucs.Task.AttachExternalCancellation(this.GetCancellationTokenOnDestroy());
            return _ucs.Task;
        }

        /// <summary>
        /// 모달 옵션을 적용한다. 서브클래스에서 텍스트 설정 방식을 결정한다.
        /// </summary>
        protected abstract void ApplyOption(ModalOpenOption option);

        [VisibleEnum(typeof(Result))]
        public void OnClickResult(int result)
        {
            _ucs?.TrySetResult((Result)result);

            if(PopupSingleton.Instance)
                PopupSingleton.Instance.CloseModal();
            else
                gameObject.SetActive(false);
        }

        public override string GetId() => "_modal";
    }
}
