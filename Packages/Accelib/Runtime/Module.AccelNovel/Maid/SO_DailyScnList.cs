using System;
using System.Collections.Generic;
using System.Linq;
using Accelib.Module.AccelNovel.Maid;
using UnityEngine;

namespace Accelib
{
    [CreateAssetMenu(fileName = "SO_DailyScnList", menuName = "Scriptable Objects/SO_DailyScnList")]
    public class SO_DailyScnList : ScriptableObject
    {
        public List<SO_MaidDailyScenario> dailyScenarios;
        
        public SO_MaidDailyScenario GetScenario(string scnKey) => dailyScenarios.FirstOrDefault(x=>x.ScnKey == scnKey);


#if UNITY_EDITOR
        private void OnValidate()
        {
            if (dailyScenarios == null || dailyScenarios.Count == 0)
            {
                Debug.LogError($"{this.name}의 시나리오가 없습니다!!!");
            }
        }
#endif
    }
}
