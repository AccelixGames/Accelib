using Accelib.AccelixWeb.Module.Advertisement.Model;

namespace Accelib.AccelixWeb.Module.Advertisement.Control.Emulator.Screen
{
    public class AdsEmulator_Rewarded : AdsEmulatorBase
    {
        protected override string Type => AdsType.Rewarded.ToString();

        public void OnClickReward()
        {
            AdsEmulator.SendMsg(unityCaller, AdsCallback.OnEvent,
                new AdsResponse(Type, unitId, AdsCode.UserEarnedReward, "Rewarded called by emulator logic"));

            OnClickClose();
        }
    }
}