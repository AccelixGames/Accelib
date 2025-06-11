#if ACCELIB_AIT
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.AccelixWeb.Module.Utility
{
    public class AITUtil_Share : MonoBehaviour
    {
        [SerializeField, TextArea] private string shareMsg = "게임을 플레이 하세요! {link}";
        [SerializeField] private string deepLink = "intoss://";
        
        [Button]
        public void HandleShare() => AppInTossNative.HandleShare(shareMsg, deepLink);
    }
}
#endif