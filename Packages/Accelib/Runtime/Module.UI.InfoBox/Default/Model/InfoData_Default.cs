using Accelib.Module.UI.InfoBox.Base.Model;

namespace Accelib.Module.UI.InfoBox.Default.Model
{
    [System.Serializable]
    public class InfoData_Default : InfoDataBase
    {
        public string description;
        
        public InfoData_Default(string desc = null) => description = desc;
        
        public override bool Equals(InfoDataBase other)
        {
            var o = (InfoData_Default)other;
            if (o == null) return false;
            
            return description.Equals(o.description);
        }
    }
}