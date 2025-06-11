using Accelib.AccelixWeb.Module.Advertisement.Model;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.AccelixWeb.Module.Advertisement.Control.Emulator.Screen
{
    public abstract class AdsEmulatorBase : MonoBehaviour
    {
        [SerializeField, ReadOnly] protected GameObject unityCaller;
        [SerializeField, ReadOnly] protected string unitId;

        protected abstract string Type { get; }
        
        public void Open(GameObject unityCaller, string unitId)
        {
            this.unityCaller = unityCaller;
            this.unitId = unitId;   
            
            gameObject.SetActive(true);
        }

        public void OnClickAds() => AdsEmulator.SendMsg(unityCaller, AdsCallback.OnEvent, new AdsResponse(Type, unitId, AdsCode.Clicked, "Clicked called by emulator logic"));
        

        public void OnClickClose()
        {
            AdsEmulator.SendMsg(unityCaller, AdsCallback.OnEvent, new AdsResponse(Type, unitId, AdsCode.Dismissed, "Dismissed called by emulator logic"));
            AdsEmulator.SendMsg(unityCaller, AdsCallback.OnShow, new AdsResponse(Type, unitId, AdsCode.Requested, "Show request called by emulator logic"));
            
            gameObject.SetActive(false);
        }
    }
}