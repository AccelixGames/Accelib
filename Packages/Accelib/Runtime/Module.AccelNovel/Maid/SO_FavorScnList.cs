using System.Collections.Generic;
using System.Linq;
using Accelib.Module.AccelNovel.Maid;
using UnityEngine;

namespace Accelib
{
    [CreateAssetMenu(fileName = "SO_FavorScnList", menuName = "Scriptable Objects/SO_FavorScnList")]
    public class SO_FavorScnList : ScriptableObject
    {
        public List<SO_MaidFavorScenario> favorScenarios;
        
        public SO_MaidFavorScenario GetScenario(string scnKey) => favorScenarios.FirstOrDefault(x=>x.ScnKey == scnKey);
        
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (favorScenarios == null || favorScenarios.Count == 0)
            {
                Debug.LogError($"{this.name}의 시나리오가 없습니다!!!");
            }
        }
#endif
    }
}
