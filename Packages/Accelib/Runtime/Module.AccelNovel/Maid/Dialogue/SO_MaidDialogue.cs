using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Sirenix.OdinInspector;
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
    public enum OdinTestTypeA
    {
        Aa,
        Ab,
        Ac
    }
    
    [CreateAssetMenu(fileName = "SO_MaidDialogue", menuName = "Maid_Scenario/Dialogue/SO_MaidDialogue")]
    public class SO_MaidDialogue : ScriptableObject
    {
        protected const string L_VERTICAL_GROUP        = "who/info";
        protected const string PROFILE_BOX_            = "who/info/profile";
        protected const string SETTINGS_VERTICAL_GROUP = "who/info/settings/who/sub";
        
        [field: Header("#### Odin")]
        [field: Header("누가 말하지?")] 
        [VerticalGroup(L_VERTICAL_GROUP)] 
        [HorizontalGroup(L_VERTICAL_GROUP + "settings/who")]
        [PreviewField]
        public Texture TalkerIcon;

        [Sirenix.OdinInspector.BoxGroup(L_VERTICAL_GROUP + "settings")]
        [VerticalGroup(SETTINGS_VERTICAL_GROUP)]
        public string name = "name";
        
        [Sirenix.OdinInspector.BoxGroup("who/sub/description")]
        [HideLabel, TextArea(4, 14)]
        public string who_description = "description";
        
        // [HorizontalGroup("who", 0.5f, MarginLeft = 5, LabelWidth = 130)]
        // [Sirenix.OdinInspector.BoxGroup("who/sub/notes")]
        // [HideLabel, TextArea(4, 9)]
        // public string who_Notes = "notes";
        
        
        [VerticalGroup(SETTINGS_VERTICAL_GROUP)]
        [ValueDropdown("OdinTestATypes")]
        [Sirenix.OdinInspector.ValidateInput("IsSupportedType")]
        public OdinTestTypeA Type;

        [AssetsOnly] [VerticalGroup(SETTINGS_VERTICAL_GROUP)]
        public GameObject PrefabTest;

        [Sirenix.OdinInspector.BoxGroup(PROFILE_BOX_)]
        public int testInt = 1;

        [Sirenix.OdinInspector.BoxGroup(PROFILE_BOX_)]
        public float testFloat = 0.5f;

        public OdinTestTypeA[] OdinTestATypes
        {
            get
            {
                return new OdinTestTypeA[]
                {
                    OdinTestTypeA.Aa,
                    OdinTestTypeA.Ab,
                    OdinTestTypeA.Ac
                };
            }
        }
        
        private bool IsSupportedType(OdinTestTypeA type)
        {
            return this.OdinTestATypes.Contains(type);
        }
        
        
        
        // protected const string LEFT_VERTICAL_GROUP             = "Split/Left";
        // protected const string STATS_BOX_GROUP                 = "Split/Left/Stats";
        // protected const string GENERAL_SETTINGS_VERTICAL_GROUP = "Split/Left/General Settings/Split/Right";
        //
        // [Space(100)]
        // [HideLabel, PreviewField(55)]
        // [VerticalGroup(LEFT_VERTICAL_GROUP)]
        // [HorizontalGroup(LEFT_VERTICAL_GROUP + "/General Settings/Split", 55, LabelWidth = 67)]
        // public Texture Icon;
        //
        // [Sirenix.OdinInspector.BoxGroup(LEFT_VERTICAL_GROUP + "/General Settings")]
        // [VerticalGroup(GENERAL_SETTINGS_VERTICAL_GROUP)]
        // public string Name;
        //
        // [Sirenix.OdinInspector.BoxGroup("Split/Right/Description")]
        // [HideLabel, TextArea(4, 14)]
        // public string Description;
        //
        // [HorizontalGroup("Split", 0.5f, MarginLeft = 5, LabelWidth = 130)]
        // [Sirenix.OdinInspector.BoxGroup("Split/Right/Notes")]
        // [HideLabel, TextArea(4, 9)]
        // public string Notes;
        //
        // // [VerticalGroup(GENERAL_SETTINGS_VERTICAL_GROUP)]
        // // [ValueDropdown("SupportedItemTypes")]
        // // [Sirenix.OdinInspector.ValidateInput("IsSupportedType")]
        // // public ItemTypes Type;
        // //
        // // [VerticalGroup("Split/Right")]
        // // public StatList Requirements;
        //
        // [AssetsOnly]
        // [VerticalGroup(GENERAL_SETTINGS_VERTICAL_GROUP)]
        // public GameObject Prefab;
        //
        // [Sirenix.OdinInspector.BoxGroup(STATS_BOX_GROUP)]
        // public int ItemStackSize = 1;
        //
        // [Sirenix.OdinInspector.BoxGroup(STATS_BOX_GROUP)]
        // public float ItemRarity;
        //
        // // public abstract ItemTypes[] SupportedItemTypes { get; }
        // //
        // // private bool IsSupportedType(ItemTypes type)
        // // {
        // //     return this.SupportedItemTypes.Contains(type);
        // // }
        
        
        
        [field:Space(50)]
        [field: Header("#### 기존")]
        [field: Header("누가 말하지?")] 
        [field: SerializeField] public bool isPlayer { get; private set; }
        [field: SerializeField, NaughtyAttributes.HideIf("isPlayer")] public string whoLocalKey { get; private set; }
        [field: SerializeField, NaughtyAttributes.HideIf("isPlayer")] public string who { get; private set; }
        [field: SerializeField] public SO_MaidScenarioBase parentScenario { get;private set; }

        [Header("애니메이션 재생 필요 정보")] 
        [SerializeField, NaughtyAttributes.ReadOnly] public int uNumber;

        [field: Header("대화 후 시스템 값 변화(ex: 인연도)")] 
        [field: SerializeField] public bool changeSystemValue {get; private set;}
        [field: SerializeField, NaughtyAttributes.ShowIf("changeSystemValue")] public int changeValue {get; private set;}
        
        [field:Header("감정변화")]
        [field: SerializeField] public bool stateChange { get; private set; }
        [field: SerializeField, NaughtyAttributes.ShowIf("stateChange")] public string stateKey { get; private set; }

        [field: Header("선택지")] 
        [field: SerializeField] public bool hasChoice { get; private set; }
        //[field:SerializeField, ShowIf("hasChoice")] public List<SO_ChoiceButton> choiceButtons;
        [field:SerializeField, NaughtyAttributes.ShowIf("hasChoice")] public List<SO_ChoiceButtonList> choiceButtonLists;

        private bool move => moveActor || addActor;
        [field: Header("씬 이동")] 
        [field: SerializeField, NaughtyAttributes.ShowIf("move")] public EWhenMove whenMove { get; private set; }
        [field: SerializeField] public bool moveActor { get; private set; }
        [field: SerializeField, NaughtyAttributes.ShowIf("moveActor"),Tooltip("새 씬을 열어야 하는지?")] public bool useNewScn{ get; private set;}
        private bool scnField => moveActor && useNewScn;
        [field: SerializeField,NaughtyAttributes.ShowIf("scnField"),Scene] public string nextScn { get; private set; }
        [field: SerializeField,NaughtyAttributes.ShowIf("scnField")] public bool usePlayerCam { get; private set; }
        
        [field:Header("Actor 추가")]
        [field: SerializeField] public bool addActor { get; private set; }
        [field: SerializeField, NaughtyAttributes.ShowIf("addActor")] public List<SAddActorFrame> addMaids;
        
        [field: Header("사운드")] //todo key들은 나중에 static ID 형태로 변경할지도
        [field: SerializeField] public bool useBGM { get; private set; }
        [field: SerializeField, NaughtyAttributes.ShowIf("useBGM")] public string bgmKey { get; private set; }
        [field: SerializeField] public bool useVoice { get; private set; }
        [field: SerializeField, NaughtyAttributes.ShowIf("useVoice")] public string voiceKey { get; private set; }

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
