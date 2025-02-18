using Accelib.Module.UI.InfoBox.Base.Control.Provider;
using Accelib.Module.UI.InfoBox.Base.Control.Provider.Interface;
using Accelib.Module.UI.InfoBox.Default.Model;
using UnityEngine;

namespace Accelib.Module.UI.InfoBox.Default.Control.Provider
{
    [RequireComponent(typeof(InfoBroadcaster))]
    public class InfoProvider_Default : MonoBehaviour, IInfoProviderT<InfoData_Default>
    {
        [SerializeField, TextArea] private string msg;
        
        public InfoData_Default ProvideInfo()
        {
            return new InfoData_Default(msg);
        }
    }
}