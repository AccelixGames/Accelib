using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Accelib.Module.AccelNovel.Model
{
    [System.Serializable]
    public class ActionLine
    {
        [Header("Core")]
        public string value;
        public string key;
        public SerializedDictionary<string,string> arguments;
        
        [Header("Args")]
        public string label;
        public string characterKey;
        public string voiceKey;

        [Header("ETC")]
        public bool isAutoSavePoint = false;
    }
}