using System.Collections.Generic;
using Accelib.Module.AccelNovel.Model.Collective.Enum;
using Accelib.Module.AccelNovel.Model.Collective.SO.Base;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Accelib.Module.AccelNovel.Model.Collective.Achievement
{
    
    [CreateAssetMenu(fileName = "(CollectiveAchievement) ", menuName = "Terri/CollectiveAchievement", order = 10)]
    public class CollectiveAchievement : ScriptableObject
    {
        [SerializeField] private List<CollectiveSO_Base> collectives;
        [SerializeField] private SO_Achievement achievement;

        public void CheckAndAchieve(in SerializedDictionary<string, CollectiveState> save)
        {
            if (collectives is not { Count: > 0 }) 
                return;
            
            foreach (var collective in collectives)
            {
                if (!save.TryGetValue(collective.AssetKey, out var state))
                    return;

                if (state is CollectiveState.Locked)
                    return;
            }

            achievement.CheckAndAchieve();
        }
    }
}