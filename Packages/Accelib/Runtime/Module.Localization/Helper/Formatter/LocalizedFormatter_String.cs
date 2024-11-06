using UnityEngine;

namespace Accelib.Module.Localization.Helper.Formatter
{
    public class LocalizedFormatter_String : MonoBehaviour, ILocalizedFormatter
    {
        [SerializeField] private string[] args;
        
        public object[] GetArgs() => args;
    }
}