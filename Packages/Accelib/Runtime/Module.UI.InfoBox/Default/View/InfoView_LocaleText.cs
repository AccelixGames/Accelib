using Accelib.Module.Localization.Helper;
using Accelib.Module.UI.InfoBox.Base.Control.Receiver.Interface;
using Accelib.Module.UI.InfoBox.Default.Model;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Accelib.Module.UI.InfoBox.Default.View
{
    public class InfoView_LocaleText  : MonoBehaviour, IInfoReceiverT<InfoData_Locale>
    {
        [TitleGroup("Core")]
        [SerializeField] private LocalizedTMP descTMP;
        [TitleGroup("Core")]
        [SerializeField] private bool clearTextOnNullInfo;
        
        [TitleGroup("Event")]
        [SerializeField] private UnityEvent onInfoReceived;
        [TitleGroup("Event")]
        [SerializeField] private UnityEvent onNullReceived;

        public void ReceiveInfo(InfoData_Locale info)
        {
            var hasInfo = !string.IsNullOrEmpty(info?.localeKey);

            if (hasInfo)
            {
                descTMP.ChangeKey(info.localeKey);
                
                onInfoReceived?.Invoke();
            }
            else
            {
                if (clearTextOnNullInfo)
                    descTMP.ChangeKey(null);
                
                onNullReceived?.Invoke();
            }
        }
    }
}