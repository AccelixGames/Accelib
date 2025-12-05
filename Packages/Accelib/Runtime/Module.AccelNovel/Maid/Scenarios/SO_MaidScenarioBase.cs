using System;
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
        [field:SerializeField] public bool continuous{get; private set;}
        
        [field:Header("시나리오 정보")]
        [field:SerializeField] public string ScnKey { get; private set; }
        [field:SerializeField] public string ScnName { get; private set; }
        
        [field:Header("스크립트")] 
        [field:SerializeField] private List<SO_MaidDialogue> dialogues;
        public List<SO_MaidDialogue> Dialogues => dialogues;

        [field:Header("커플씬")] 
        [field: SerializeField] public bool limitToday{ get; private set; }
        [field: SerializeField, ShowIf("limitToday"), ReadOnly] public bool todaySeen{ get; private set; }
        [field: SerializeField] public bool limitStart { get; private set; }
        [field: SerializeField, ShowIf("limitStart"), ReadOnly] public bool startSeen{ get; private set; }
        public bool canCouple => limitToday || limitStart;
        [field: SerializeField,ShowIf("canCouple")] public SO_MaidScenarioBase coupleScenario{ get; private set; }

        public void ResetHasSeen()
        {
            hasSeen = false;
        }
        
        public void TodaySeen()
        {
            if(continuous) return;
            
            todaySeen = true;
        }

        public void ResetTodaySeen()
        {
            if(continuous) return;
            
            todaySeen = false;
            
            foreach (var d in dialogues)
            {
                //if (d.choiceButtonLists?.Count > 0)
                if (d.hasChoice)
                {
                    d.ResetToday();
                }
            }
        }

        public void StartSeen()
        {
            if(continuous) return;
            
            startSeen = true;
        }

        public void ResetStartSeen()
        {
            if(continuous) return;
            
            startSeen = false;
        }

        public SO_MaidScenarioBase GetScenario()
        {
            if ((todaySeen || startSeen) && coupleScenario != null)
            {
                var scenario = coupleScenario.GetScenario();
                
                return scenario;
            }
            
            return this;
        }

        private void OnValidate()
        {
            if (dialogues.Count > 0)
            {
                foreach (var d in dialogues)
                {
                   d.SetParentScenario(this); 
                }
            }
        }
    }
}
