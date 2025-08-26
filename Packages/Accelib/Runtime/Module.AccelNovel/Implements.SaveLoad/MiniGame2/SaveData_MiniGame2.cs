using Accelib.Module.SaveLoad.SaveData;
using AYellowpaper.SerializedCollections;
using Newtonsoft.Json;

namespace Accelib.Module.AccelNovel.Implements.SaveLoad.MiniGame2
{
    [System.Serializable]
    public class SaveData_MiniGame2 : SaveDataBase
    {
        public SerializedDictionary<string, MaxScoreData> saveData;
        
        public override void New()
        {
            saveData = new SerializedDictionary<string, MaxScoreData>();
        }

        public override void FromJson(string json)
        {
            var des = JsonConvert.DeserializeObject<SaveData_MiniGame2>(json);
            saveData = des.saveData;
        }
    }
}