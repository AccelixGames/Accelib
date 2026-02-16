using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Module.UI.Popup.Data
{
    [System.Serializable]
    public class ModalOpenOption
    {
        public string title;
        [TextArea] public string desc;
        [ShowInInspector] public object[] descParams;
        public string ok;
        public string ng;
    }
}
