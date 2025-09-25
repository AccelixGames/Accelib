using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Accelib
{
    [CreateAssetMenu(fileName = "SO_StartConversationList", menuName = "Scriptable Objects/SO_StartConversationList")]
    public class SO_StartConversationList : ScriptableObject
    {
        public List<SO_MaidScenarioBase> startConversations;
        
        public SO_MaidScenarioBase GetStartConversation(string scnKey) => startConversations.FirstOrDefault(x=>x.ScnKey == scnKey);
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (startConversations == null || startConversations.Count == 0)
            {
                Debug.LogError($"{this.name}의 시나리오가 없습니다!!!");
            }
        }
#endif
    }
}
