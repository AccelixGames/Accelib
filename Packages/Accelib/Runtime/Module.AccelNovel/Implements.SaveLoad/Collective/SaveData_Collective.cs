using Accelib.Module.AccelNovel.Model.Collective.Enum;
using Accelib.Module.SaveLoad.SaveData;
using AYellowpaper.SerializedCollections;
using Newtonsoft.Json;

namespace Accelib.Module.AccelNovel.Implements.SaveLoad.Collective
{
    [System.Serializable]
    public class SaveData_Collective : SaveDataBase
    {
        public SerializedDictionary<string, CollectiveState> saveData;
        
        public override void New()
        {
            saveData = new SerializedDictionary<string, CollectiveState>();
        }

        public override void FromJson(string json)
        {
            var des = JsonConvert.DeserializeObject<SaveData_Collective>(json);
            saveData = des.saveData;
        }
    }
}