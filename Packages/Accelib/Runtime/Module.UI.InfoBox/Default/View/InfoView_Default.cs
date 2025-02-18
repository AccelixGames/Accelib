using Accelib.Module.UI.InfoBox.Base.Control.Receiver.Interface;
using Accelib.Module.UI.InfoBox.Default.Model;
using TMPro;
using UnityEngine;

namespace Accelib.Module.UI.InfoBox.Default.View
{
    public class InfoView_Default : MonoBehaviour, IInfoReceiverT<InfoData_Default>
    {
        [SerializeField] private TMP_Text descTMP;

        public void ReceiveInfo(InfoData_Default info)
        {
            if(info == null) 
                gameObject.SetActive(false);
            else
            {
                gameObject.SetActive(true);
                descTMP.text = info.description;
            }
        }
    }
}