using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Module.AccelNovel.Maid
{
    public enum EWhenMove
    {
        None = 0,
        BeforeTalk = 1,
        AfterTalk = 2,
    }

    public enum EFromWhere
    {
        None = 0,
        Left = 1,
        Right = 2,
    }

    [System.Serializable]
    public struct SAddActorFrame
    {
        public EMaidName actorName;
        public EFromWhere fromWhere;
    }
    
    [CreateAssetMenu(fileName = "SO_MaidDialogue", menuName = "Maid_Scenario/Dialogue/SO_MaidDialogue")]
    public class SO_MaidDialogue : ScriptableObject
    {
        [field:Header("누가 말하지?")]
        [field: SerializeField] public bool isPlayer { get; private set; }
        [field: SerializeField, HideIf("isPlayer")] public string whoLocalKey { get; private set; }
        [field: SerializeField, HideIf("isPlayer")] public string who { get; private set; }
        [field: SerializeField] public SO_MaidScenarioBase parentScenario { get;private set; }

        [Header("애니메이션 재생 필요 정보")] 
        [SerializeField, ReadOnly] public int uNumber;

        [field: Header("대화 후 시스템 값 변화(ex: 인연도)")] 
        [field: SerializeField] public bool changeSystemValue {get; private set;}
        [field: SerializeField, ShowIf("changeSystemValue")] public int changeValue {get; private set;}
        
        [field:Header("감정변화")]
        [field: SerializeField] public bool stateChange { get; private set; }
        [field: SerializeField, ShowIf("stateChange")] public string stateKey { get; private set; }

        [field: Header("선택지")] 
        [field: SerializeField] public bool hasChoice { get; private set; }
        //[field:SerializeField, ShowIf("hasChoice")] public List<SO_ChoiceButton> choiceButtons;
        [field:SerializeField, ShowIf("hasChoice")] public List<SO_ChoiceButtonList> choiceButtonLists;

        private bool move => moveActor || addActor;
        [field: Header("씬 이동")] 
        [field: SerializeField, ShowIf("move")] public EWhenMove whenMove { get; private set; }
        [field: SerializeField] public bool moveActor { get; private set; }
        [field: SerializeField, ShowIf("moveActor"),Tooltip("새 씬을 열어야 하는지?")] public bool useNewScn{ get; private set;}
        private bool scnField => moveActor && useNewScn;
        [field: SerializeField,ShowIf("scnField"),Scene] public string nextScn { get; private set; }
        [field: SerializeField,ShowIf("scnField")] public bool usePlayerCam { get; private set; }
        
        [field:Header("Actor 추가")]
        [field: SerializeField] public bool addActor { get; private set; }
        [field: SerializeField, ShowIf("addActor")] public List<SAddActorFrame> addMaids;
        
        [field: Header("사운드")] //todo key들은 나중에 static ID 형태로 변경할지도
        [field: SerializeField] public bool useBGM { get; private set; }
        [field: SerializeField, ShowIf("useBGM")] public string bgmKey { get; private set; }
        [field: SerializeField] public bool useVoice { get; private set; }
        [field: SerializeField, ShowIf("useVoice")] public string voiceKey { get; private set; }

        [field:Header("텍스트")] 
        [field:SerializeField] public string localKey{ get; private set; }
        [field:SerializeField, ResizableTextArea] public string text { get; private set; }


        public void ResetToday()
        {
            foreach (var btn in choiceButtonLists)
            {
                btn.ResetTodaySeen();
            }
        }

        public void SetParentScenario(SO_MaidScenarioBase scenario)
        {
            parentScenario = scenario;
        }
    }
}
