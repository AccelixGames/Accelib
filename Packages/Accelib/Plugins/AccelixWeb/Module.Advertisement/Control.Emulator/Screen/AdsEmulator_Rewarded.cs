#if ACCELIB_AIT
using Accelib.AccelixWeb.Module.Advertisement.Model;

namespace Accelib.AccelixWeb.Module.Advertisement.Control.Emulator.Screen
{
    public class AdsEmulator_Rewarded : AdsEmulatorBase
    {
        protected override string Type => AdsType.Rewarded.ToString();

        public void OnClickReward()
        {
            if(emulator.EventToInvoke.HasFlag(AdsEmulator.EventFlag.UserEarnedReward))
                AdsEmulator.SendMsg(unityCaller, AdsCallback.OnEvent,
                    new AdsResponse(Type, unitId, AdsCode.UserEarnedReward, "Rewarded called by emulator logic"));

            OnClickClose();
        }
    }
}
#endif