#if ACCELIB_AIT
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.AccelixWeb.Control.Utility
{
    public class AITUtil_Share : MonoBehaviour
    {
        [SerializeField] private string shareMsg;
        
        [Button]
        public void HandleShare() => AppInTossNative.HandleShare(shareMsg);
    }
}
#endif