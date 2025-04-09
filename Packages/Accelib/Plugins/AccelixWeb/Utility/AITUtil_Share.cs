#if ACCELIB_AIT
using Accelix.Plugins.AccelixWeb;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.AccelixWeb.Utility
{
    public class AITUtil_Share : MonoBehaviour
    {
        [SerializeField] private string shareMsg;
        
        [Button]
        public void HandleShare() => AppInTossNative.HandleShare(shareMsg);
    }
}
#endif