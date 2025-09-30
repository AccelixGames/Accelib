using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Accelib.Module.AccelNovel.Maid
{
    [CreateAssetMenu(fileName = "SO_ChoiceButton", menuName = "Maid_Scenario/Dialogue/SO_ChoiceButton")]
    public class SO_ChoiceButton : ScriptableObject
    {
        [SerializeField, Tooltip("이미 선택 됐던 선택지인지")] public bool chose;
        [field: SerializeField,  TextArea] public string text { get; private set; }
        
        [Header("씬 이동")] 
        [field:SerializeField, HideIf("isExit")] public bool hasAfterScn;
        [field:SerializeField, ShowIf("hasAfterScn")] public SO_MaidScenarioBase nextScenario { get; private set; }

        [Header("나가기 버튼")] 
        [field: SerializeField, HideIf("hasAfterScn")] public bool isExit;
        
        [Header("버튼 보이는 조건 설정")]
        [field:SerializeField] public bool dailyFilter;
        [field:SerializeField, ShowIf("dailyFilter")] public SDailyFilter DFilter{ get; private set; }
        
        [field:SerializeField] public bool favorFilter;
        [field:SerializeField, ShowIf("favorFilter")] public SFavorFilter FFilter{ get; private set; }
        
        [field:SerializeField] public bool giftFilter;
        [field:SerializeField, ShowIf("giftFilter")] public SGiftFilter GFilter{ get; private set; }
        
        [field:SerializeField] public bool toyFilter;
        [field:SerializeField, ShowIf("toyFilter")] public SToyFilter TFilter{ get; private set; }
        
        public bool anyFilter => dailyFilter || favorFilter || giftFilter || toyFilter;
        
        [field:Header("아이콘 사용")]
        [field:SerializeField, ShowIf("anyFilter")] public bool hasIcon{ get; private set; }
        [field:SerializeField, ShowIf("hasIcon")] public Sprite sideIcon{ get; private set; }
        
        public bool isShow => IsShow();
        

        private bool IsShow()
        {
            if (dailyFilter) return MaidScenarioDirector.DailyFilter(DFilter, MaidScenarioDirector.CurrentFilter);
            
            if (favorFilter) return MaidScenarioDirector.FavorFilter(FFilter, MaidScenarioDirector.CurrentFilter);
            
            if (giftFilter) return MaidScenarioDirector.GiftFilter(GFilter, MaidScenarioDirector.CurrentFilter);
            
            if (toyFilter) return MaidScenarioDirector.ToyFilter(TFilter, MaidScenarioDirector.CurrentFilter);

            return true;
        }

        // public void ConnectDialogue(SO_MaidDialogue dlg)
        // {
        //     dialogue = dlg;
        // }

        public void Selected()
        {
            //Debug.Log($"{name} Selected!!!!!");
            
            //chose = true;
            
            // 이후 씬 연결이 있다면
            if(hasAfterScn)
                MaidScenarioDirector.PublishAddScenario(nextScenario);
            
            // todo 그냥 떠나기 라면 캐릭터 간단한 감정표현
            if (isExit)
            {
                Debug.Log($"{name} Exit, {MaidScenarioDirector.mainActor} 표정 + 감정 텍스트!");
            }
        }
    }
}
