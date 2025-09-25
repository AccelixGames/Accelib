using System.Collections.Generic;
using Accelib.Module.AccelNovel.Maid;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib
{
    [CreateAssetMenu(fileName = "SO_MaidScenarioBase", menuName = "Scriptable Objects/SO_MaidScenarioBase")]
    public class SO_MaidScenarioBase : ScriptableObject
    {
        [Header("인게임 관련")] 
        [SerializeField, ReadOnly, Tooltip("이미 봤던 시나리오인지 체크")] public bool hasSeen;
        
        [Header("시나리오 정보")]
        [SerializeField] public string ScnKey { get; private set; }
        [SerializeField] public string ScnName { get; private set; }
        
        [Header("스크립트")] 
        [SerializeField] private List<SO_MaidDialogue> dialogues;
        public List<SO_MaidDialogue> Dialogues => dialogues;
        
        [Header("대화 오픈 조건")]
        [field:SerializeField, ShowIf("IsBase")] public SBaseFilter BFilter;
        
        private bool IsBase => GetType() == typeof(SO_MaidScenarioBase);
    }
}
