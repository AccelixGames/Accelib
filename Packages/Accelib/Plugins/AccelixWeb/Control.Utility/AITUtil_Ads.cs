#if ACCELIB_AIT
using Accelib.AccelixWeb.Control.Manager;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.AccelixWeb.Control.Utility
{
    public class AITUtil_Ads : MonoBehaviour
    {
        [Button]
        public void LoadRewarded()
        {
            AITManager_Ads.Instance.LoadRewarded();
        }

        [Button]
        public void ShowRewarded()
        {
            AITManager_Ads.Instance.ShowRewarded();
        }
    }
}
#endif