using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Module.AccelNovel.Maid
{
    //[CreateAssetMenu(fileName = "SO_MaidScenarioBase", menuName = "Maid_Scenario/Scenario/SO_MaidScenarioBase")]
    public abstract class SO_MaidScenarioBase : ScriptableObject, IScenarioWithKey
    {
        [Header("인게임 관련")] 
        [SerializeField, ReadOnly, Tooltip("이전에도 봤던 시나리오인지 체크")] public bool hasSeen;
        
        
        [Header("시나리오 정보")]
        [field:SerializeField] public string ScnKey { get; private set; }
        [field:SerializeField] public string ScnName { get; private set; }
        
        [Header("스크립트")] 
        [SerializeField] private List<SO_MaidDialogue> dialogues;
        public List<SO_MaidDialogue> Dialogues => dialogues;
        
    }
}
