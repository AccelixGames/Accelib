using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Accelib.Module.AccelNovel.Model.SO
{
    
    [CreateAssetMenu(fileName = "char_", menuName = "Accelib/AccelNovel/Character", order = 11)]
    public class SO_Character : ScriptableObject
    {
        public string key;
        public string displayName;
        public Sprite thumbnail;
        public SerializedDictionary<string, string> resourceKeyDict;
    }
}