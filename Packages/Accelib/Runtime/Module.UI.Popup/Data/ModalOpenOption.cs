using UnityEngine;

namespace Accelib.Module.UI.Popup.Data
{
    [System.Serializable]
    public class ModalOpenOption
    {
        public bool useLocale;
        public string title;
        [TextArea] public string desc;
        public string ok;
        public string ng;
    }
}