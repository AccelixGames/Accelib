using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Module.AccelNovel.Maid
{
    [CreateAssetMenu(fileName = "SO_ChoiceButton", menuName = "Maid_Scenario/Dialogue/SO_ChoiceButton")]
    public class SO_ChoiceButton : ScriptableObject
    {
        [field:Header("우선도, 높을수록 우선시")]
        [field: SerializeField] public int priority {get; private set;}
        
        [field:Header("버튼 비활성화")]
        [field: SerializeField] public bool neverInteract {get; private set;}
        [field: SerializeField] public bool cantInteract {get; private set;}
        [field: SerializeField] public bool noShowJustOffInteractive {get; private set;}
        
        [field: Header("경고 텍스트")]
        [field: SerializeField] public string warningText {get; private set;}
        [field: SerializeField] public string warningTextLocalKey {get; private set;}
        
        [SerializeField, Tooltip("이미 선택 됐던 선택지인지")] public bool chose;
        
        [field: Header("버튼 텍스트")]
        [field: SerializeField] public string localKey { get; private set; }
        [field: SerializeField,  TextArea] public string text { get; private set; }
        
        [Header("시나리오 이동")] 
        [field:SerializeField, HideIf("isExit")] public bool hasAfterScn;
        [field:SerializeField, ShowIf("hasAfterScn")] public SO_MaidScenarioBase nextScenario { get; private set; }
        //[field:SerializeField, ShowIf("hasAfterScn")] public List<SO_MaidScenarioBase> nextScenarioList { get; private set; }
        [field:SerializeField, ReadOnly]private SO_MaidScenarioBase pickedScenario;

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
            if (dailyFilter)
            {
                var dF = MaidScenarioDirector.DailyFilter(DFilter, MaidScenarioDirector.CurrentFilter);
                
                //inActiveButton = !dF;

                if (!dF)
                {
                    if (noShowJustOffInteractive) cantInteract = true;
                    return noShowJustOffInteractive;
                }
            }

            if (favorFilter)
            {
                var fF = MaidScenarioDirector.FavorFilter(FFilter, MaidScenarioDirector.CurrentFilter);
                
                //inActiveButton = !fF;

                if (!fF)
                {
                    if (noShowJustOffInteractive) cantInteract = true;
                    return noShowJustOffInteractive;
                }
            }

            if (giftFilter)
            {
                var gF = MaidScenarioDirector.GiftFilter(GFilter, MaidScenarioDirector.CurrentFilter);
                
                //inActiveButton = !gF;

                if (!gF)
                {
                    if (noShowJustOffInteractive) cantInteract = true;
                    return noShowJustOffInteractive;
                }
            }

            if (toyFilter)
            {
                var tF = MaidScenarioDirector.ToyFilter(TFilter, MaidScenarioDirector.CurrentFilter);
                
                //inActiveButton = !tF;

                if (!tF)
                {
                    if (noShowJustOffInteractive) cantInteract = true;
                    return noShowJustOffInteractive;
                }
            }

            if (noShowJustOffInteractive) cantInteract = false;
            
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
            if (hasAfterScn)
            {
                PickedScenario();

                if (pickedScenario != null)
                {
                    //pickedScenario.hasSeen = true;
                    MaidScenarioDirector.PublishAddScenario(pickedScenario);
                }
                else
                {
                    Debug.LogError("버튼에서 이동할 씬이 없습니다");
                }
            }
            
            // todo 그냥 떠나기 라면 캐릭터 간단한 감정표현
            if (isExit)
            {
                //Debug.Log($"{name} Exit, {MaidScenarioDirector.mainActor} 표정 + W감정 텍스트!");
            }
        }

        private void PickedScenario()
        {
            pickedScenario = nextScenario;
            
            // pickedScenario = null;
            //
            // foreach (var scn in nextScenarioList)
            // {
            //     if (!scn.hasSeen)
            //     {
            //         pickedScenario = scn;
            //         break;
            //     }
            // }
            
            //return pickedScenario;
        }
    }
}
