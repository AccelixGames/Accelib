#if ACCELIB_AIT
using Accelib.AccelixWeb.Module.Advertisement.Model;
using UnityEngine;

namespace Accelib.AccelixWeb.Module.Advertisement.Control.Emulator.Screen
{
    public class AdsEmulator_Interstitial : AdsEmulatorBase
    {
        protected override string Type => AdsType.Interstitial.ToString();
    }
}
#endif