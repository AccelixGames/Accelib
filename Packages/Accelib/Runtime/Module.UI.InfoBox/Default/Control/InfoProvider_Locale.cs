using Accelib.Module.Localization;
using Accelib.Module.Localization.Model;
using Accelib.Module.UI.InfoBox.Base.Control.Provider;
using Accelib.Module.UI.InfoBox.Base.Control.Provider.Interface;
using Accelib.Module.UI.InfoBox.Default.Model;
using UnityEngine;

namespace Accelib.Module.UI.InfoBox.Default.Control
{
    [RequireComponent(typeof(InfoBroadcaster))]
    public class InfoProvider_Locale : MonoBehaviour, IInfoProviderT<InfoData_Default>
    {
        [SerializeField] private LocaleKey key;

        private InfoData_Default _infoData;
        
        public InfoData_Default ProvideInfo()
        {
            var localizedString = LocalizationSingleton.GetLocalizedString(key.key, this);

            _infoData ??= new InfoData_Default(string.Empty);
            _infoData.description = localizedString;
            return _infoData;
        }
    }
}