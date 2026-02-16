using Accelib.UI.Popup.Runtime.Data;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.UI.Popup.Runtime.Layer.Base
{
    /// <summary>
    /// 비동기 모달 다이얼로그의 추상 베이스 클래스.
    /// </summary>
    public abstract class LayerPopup_Modal : LayerPopupBase
    {
        [TitleGroup("버튼")]
        [SerializeField] protected GameObject okButtonObj;
        [TitleGroup("버튼")]
        [SerializeField] protected GameObject ngButtonObj;

        public enum Result {OK = 0, NG = 1, Exception = -1}

        private UniTaskCompletionSource<Result> _ucs;

        private void OnDisable() => _ucs?.TrySetResult(Result.Exception);

        public UniTask<Result> Open(IModalOptionProvider option)
        {
            gameObject.SetActive(true);

            // 버튼 오브젝트 활성화/비활성화
            if (okButtonObj) okButtonObj.SetActive(!string.IsNullOrEmpty(option.Ok));
            if (ngButtonObj) ngButtonObj.SetActive(!string.IsNullOrEmpty(option.Ng));

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
        protected abstract void ApplyOption(IModalOptionProvider option);

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
