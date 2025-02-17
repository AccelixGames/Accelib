using Accelib.Module.UI.InfoBox.Base.View;
using Accelib.Module.UI.InfoBox.Default.Model;
using TMPro;
using UnityEngine;

namespace Accelib.Module.UI.InfoBox.Default.View
{
    public class InfoBoxView_Default : InfoBoxViewBaseT<InfoData_Default>
    {
        [SerializeField] private TMP_Text descTMP;
        
        protected override void OnDrawInfo(InfoData_Default info)
        {
            descTMP.text = info.description;
        }
    }
}