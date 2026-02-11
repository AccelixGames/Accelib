using Accelib.Module.UI.InfoBox.Base.Model;

namespace Accelib.Module.UI.InfoBox.Default.Model
{
    [System.Serializable]
    public class InfoData_Locale : InfoDataBase
    {
        public string localeKey;
        public bool ignoreLocale = false;
        //public object[] parameters;
        
        public InfoData_Locale(string key = null, bool ignore = false)
        {
            localeKey = key;
            ignoreLocale = ignore;
        }

        public override bool Equals(InfoDataBase other)
        {
            var o = (InfoData_Locale)other;
            if (o == null) return false;
            
            //return localeKey.Equals(o.localeKey) && parameters.Equals(o.parameters);
            return localeKey.Equals(o.localeKey) &&  ignoreLocale.Equals(o.ignoreLocale);
        }
    }
}