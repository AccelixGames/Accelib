using Accelib.Module.SaveLoad.SaveData;
using Newtonsoft.Json;

namespace Accelix.Singleton.SaveLoad.Statistics
{
    [System.Serializable]
    public class SaveData_Stats : SaveDataBase
    {
        public int replayCount;
        
        public override void New()
        {
            replayCount = 0;
        }

        public override void FromJson(string json)
        {
            var des = JsonConvert.DeserializeObject<SaveData_Stats>(json);
            replayCount = des.replayCount;
        }
    }
}