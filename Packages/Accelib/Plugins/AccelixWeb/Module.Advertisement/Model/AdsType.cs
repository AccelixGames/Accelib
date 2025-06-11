#if ACCELIB_AIT
using UnityEngine;

namespace Accelib.AccelixWeb.Module.Advertisement.Model
{
    public enum AdsType
    {
        [InspectorName("전면 광고")]
        Interstitial = 0,
        [InspectorName("리워드 광고")]
        Rewarded = 1
    }
}
#endif