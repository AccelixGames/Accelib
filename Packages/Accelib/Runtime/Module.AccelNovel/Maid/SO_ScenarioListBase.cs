using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Module.AccelNovel.Maid
{
    public abstract class SO_ScenarioListBase<TScenario> : ScriptableObject where TScenario : SO_MaidScenarioBase
    {
        [SerializeField] private List<TScenario> scenarios = new();
        
        private Dictionary<string, TScenario> _cache;

        public IReadOnlyList<TScenario> Scenarios => scenarios;

        public TScenario GetScenario(string scnKey)
        {
            if (string.IsNullOrEmpty(scnKey)) return null;
            
            EnsureCache();

            return _cache.TryGetValue(scnKey, out var scn) ? scn : scenarios.FirstOrDefault(x=>x.ScnKey == scnKey);
        }

        public bool TryGetScenario(string scnKey, out TScenario scenario)
        {
            scenario = GetScenario(scnKey);
            
            return scenario != null;
        }

        private void EnsureCache()
        {
            if (_cache != null) return;
            
            _cache = new Dictionary<string, TScenario>();

            foreach (var scn in scenarios)
            {
                if(scn == null || string.IsNullOrEmpty(scn.ScnKey)) continue;

                _cache.TryAdd(scn.ScnKey, scn);
            }
            
        }

        private void OnEnable()
        {
            _cache = null;
            EnsureCache();
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (scenarios == null || scenarios.Count == 0)
            {
                Debug.LogError($"{name}의 시나리오가 없습니다!!!", this);
                return;
            }
            
            var seen = new HashSet<string>();
            for (int i = 0; i < scenarios.Count; ++i)
            {
                // var scn = scenarios[i];
                //
                // if (scn == null)
                // {
                //     Debug.LogError($"{name}의 시나리오 [{i}]가 null 입니다!!!", this);
                //     continue;
                // }
                //
                // if (string.IsNullOrEmpty(scn.ScnKey))
                // {
                //     Debug.LogError($"{name}의 시나리오 [{i}]의 ScnKey가 비어있습니다!!!", this);
                //     continue;
                // }
                //
                // if (!seen.Add(scn.ScnKey))
                // {
                //     Debug.LogError($"{name}에 중복된 ScnKey가 있습니다!!!! {scn.ScnKey}", this);
                // }
            }
            
            _cache = null;
            EnsureCache();
        }

        // [ContextMenu(nameof(ResetSeen))][Button]
        // public void ResetSeen()
        // {
        //     foreach (var s in scenarios)
        //     {
        //         s.ResetHasSeen();
        //         s.ResetTodaySeen();
        //         s.ResetStartSeen();
        //     }
        // }
#endif
    }
}
