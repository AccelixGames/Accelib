#if ACCELIB_AIT
using Accelib.AccelixWeb.Module.Advertisement.Model;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.AccelixWeb.Module.Advertisement.Control.Emulator.Screen
{
    public abstract class AdsEmulatorBase : MonoBehaviour
    {
        [SerializeField] protected AdsEmulator emulator;
        [SerializeField, ReadOnly] protected GameObject unityCaller;
        [SerializeField, ReadOnly] protected string unitId;

        protected abstract string Type { get; }
        
        public void Open(GameObject unityCaller, string unitId)
        {
            this.unityCaller = unityCaller;
            this.unitId = unitId;   
            
            gameObject.SetActive(true);
        }

        public void OnClickAds()
        {
            if(emulator.EventToInvoke.HasFlag(AdsEmulator.EventFlag.Clicked))
                AdsEmulator.SendMsg(unityCaller, AdsCallback.OnEvent, new AdsResponse(Type, unitId, AdsCode.Clicked, "Clicked called by emulator logic"));
        }

        public void OnClickClose()
        {
            if(emulator.EventToInvoke.HasFlag(AdsEmulator.EventFlag.Dismissed))
                AdsEmulator.SendMsg(unityCaller, AdsCallback.OnEvent, new AdsResponse(Type, unitId, AdsCode.Dismissed, "Dismissed called by emulator logic"));
            if(emulator.EventToInvoke.HasFlag(AdsEmulator.EventFlag.Requested))
                AdsEmulator.SendMsg(unityCaller, AdsCallback.OnShow, new AdsResponse(Type, unitId, AdsCode.Requested, "Show request called by emulator logic"));
            
            gameObject.SetActive(false);
        }
    }
}
#endif