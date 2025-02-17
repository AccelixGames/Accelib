using Accelib.Module.UI.InfoBox.Base.Control;
using Accelib.Module.UI.InfoBox.Default.Model;
using UnityEngine;

namespace Accelix.Accelib.Module.UI.InfoBox.Default.Control
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