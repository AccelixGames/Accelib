using Accelib.Module.Localization.Model;
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
    }
}
