using Accelib.Module.AccelNovel.Model;
using Accelib.Module.SaveLoad;
using Accelix.Accelib.AccelNovel.Model;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Accelib.Module.AccelNovel.Control.Parameter
{
    public class ParameterController : MonoBehaviour
    {
        [SerializeField] private NovelController novel;
        
        [field:Header("Parameter")]
        [field: SerializeField] public IntVariable Affection { get; private set; }
        [field: SerializeField] public StringVariable PlayerName { get; private set; }

        public NovelController Novel => novel;
        
        public void Initialize(SaveData saveData)
        {
            Affection.Value = saveData?.affection ?? 0;
            Affection.Changed.UnregisterAll();
            
            PlayerName.Value = saveData?.playerName;
            if(string.IsNullOrEmpty(PlayerName.Value))
                PlayerName.Value = "올리";
            PlayerName.Changed.UnregisterAll();
        }

        public int? GetValue(string key)
        {
            if (key == "affection") return Affection.Value;
            if (key == "replay") return novel.ReplayCount;

            return null;
        }
    }
}