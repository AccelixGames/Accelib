using Newtonsoft.Json;

namespace Accelib.Module.SaveLoad.SaveData
{
    [System.Serializable]
    public abstract class SaveDataBase
    {
        public abstract void New();
        public abstract void FromJson(string json);
        public virtual string ToJson() => JsonConvert.SerializeObject(this);
    }
}