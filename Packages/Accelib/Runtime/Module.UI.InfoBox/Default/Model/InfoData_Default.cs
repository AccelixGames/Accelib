using Accelib.Module.UI.InfoBox.Base.Model;

namespace Accelib.Module.UI.InfoBox.Default.Model
{
    [System.Serializable]
    public class InfoData_Default : InfoDataBase
    {
        public string description;
        
        public InfoData_Default(string desc) => description = desc;
    }
}