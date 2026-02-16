using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.UI.Popup.Runtime.Data
{
    /// <summary>
    /// 모달 설정 데이터를 에셋으로 관리하는 ScriptableObject.
    /// </summary>
    [CreateAssetMenu(menuName = "Accelib.UI/Popup/ModalOpenOption", fileName = "SO_ModalOpenOption")]
    public class SO_ModalOpenOption : ScriptableObject, IModalOptionProvider
    {
        [field: Title("정보")]
        [field: SerializeField] public string Title { get; private set; }
        [field: SerializeField] public string Desc { get; private set; }
        [field: SerializeField] public string Ok { get; private set; }
        [field: SerializeField] public string Ng { get; private set; }
        
        [Title("파라매터")]
        [ShowInInspector, ReadOnly] public object[] DescParams { get; private set; }

        public void SetParams(object[] param)
        {
            DescParams = param;
        }
    }
}
