using System.Collections.Generic;
using Accelib.Module.SaveLoad.SaveData;
using Newtonsoft.Json;

namespace Accelix.Singleton.SaveLoad.Tip
{
    [System.Serializable]
    public class SaveData_Tip : SaveDataBase
    {
        public List<string> tipSaves;
        public int miniGameLevelMax;
        
        public override void New()
        {
            tipSaves = new List<string>();
            miniGameLevelMax = 0;
        }

        public override void FromJson(string json)
        {
            var des = JsonConvert.DeserializeObject<SaveData_Tip>(json);
            tipSaves = des.tipSaves;
            miniGameLevelMax = des.miniGameLevelMax;
        }
    }
}