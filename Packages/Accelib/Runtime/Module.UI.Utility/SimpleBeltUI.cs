using TMPro;
using UnityEngine;

namespace Accelib.Module.UI.Utility
{
    public class SimpleBeltUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text tmp;
        
        public void Activate(string msg)
        {
            tmp.text = msg;
            gameObject.SetActive(true);
        }
    }
}