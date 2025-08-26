using Accelib.Module.AccelNovel.Implements.SaveLoad.Collective;
using Accelib.Module.AccelNovel.Model.Collective.Enum;
using Accelib.Module.SaveLoad;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Module.AccelNovel.Model.Collective.SO.Base
{
    public abstract class CollectiveSO_Base : ScriptableObject
    {
        [field: SerializeField] public string AssetKey { get; set; }

        public CollectiveState CurrState
        {
            get 
            {
                var storage = SaveLoadSingleton.Get<SaveDataHolder_Collective>();
                return storage.GetState(this);
            }
        }

        [Button]
        [ContextMenu(nameof(OnCollect))]
        public bool OnCollect()
        {
            var storage = SaveLoadSingleton.Get<SaveDataHolder_Collective>();
            var state = storage.GetState(this);

            if (state is CollectiveState.Locked)
            {
                storage.SetState(this, CollectiveState.UnlockedAsNew);
                return true;
            }

            return false;
        }
        
        [Button]
        [ContextMenu(nameof(OnViewed))]
        public void OnViewed()
        {
            var storage = SaveLoadSingleton.Get<SaveDataHolder_Collective>();
            storage.SetState(this, CollectiveState.Unlocked);
        }
    }
}