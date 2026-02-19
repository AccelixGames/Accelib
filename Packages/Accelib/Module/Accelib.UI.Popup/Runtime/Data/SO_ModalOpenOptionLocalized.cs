using Accelib.Module.Localization.Model;
using Accelib.UI.Popup.Runtime.Layer.Base;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.UI.Popup.Runtime.Data
{
    /// <summary>
    /// 모달 설정 데이터를 에셋으로 관리하는 ScriptableObject.
    /// </summary>
    [CreateAssetMenu(menuName = "Accelib.UI/Popup/ModalOpenOptionLocalized", fileName = "(ModalOption) Name")]
    public class SO_ModalOpenOptionLocalized : ScriptableObject, IModalOptionProvider
    {
        [Title("정보")]
        [SerializeField] private LocaleKey title;
        [SerializeField] private LocaleKey desc;
        [SerializeField] private LocaleKey ok;
        [SerializeField] private LocaleKey ng;

        public string Title => title.key;
        public string Desc => desc.key;
        public string Ok => ok.key;
        public string Ng => ng.key;

        [Title("파라매터")]
        [ShowInInspector, ReadOnly]
        public object[] DescParams { get; private set; }

        public void SetParams(params object[] param)
        {
            DescParams = param;
        }

        [Button]
        public async UniTask<LayerPopup_Modal.Result> OpenAsync(params object[] param)
        {
            SetParams(param);

            var instance = PopupSingleton.Instance;
            if (instance == null)
            {
                Debug.LogError("PopupSingleton instance is null", this);
                return LayerPopup_Modal.Result.Exception;
            }
            return await PopupSingleton.Instance.OpenModalAsync(this);
        }
    }
}
