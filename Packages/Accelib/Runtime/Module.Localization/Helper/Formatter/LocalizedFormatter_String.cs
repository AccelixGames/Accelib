using UnityEngine;

namespace Accelib.Module.Localization.Helper.Formatter
{
    public class LocalizedFormatter_String : MonoBehaviour, ILocalizedFormatter
    {
        [SerializeField] private string[] args;
        
        public object[] GetArgs() => args;
        
        public void SetArgs(params object[] a)
        {
            if (a == null)
            {
                args = null;
                return;
            }
            if(args == null || args.Length != a.Length)
                args = new string[a.Length];
            
            for (var i = 0; i < a.Length; i++) 
                args[i] = a[i].ToString();
        }
    }
}