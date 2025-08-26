using System.Collections.Generic;
using Accelib.Extensions;
using Accelib.Logging;
using Accelib.Module.AccelNovel.Control.Utility;
using Accelib.Module.AccelNovel.Implements.SaveLoad.Collective;
using Accelib.Module.AccelNovel.Model;
using Accelib.Module.AccelNovel.Model.Collective.Enum;
using Accelib.Module.Audio.Data;
using Accelib.Module.SaveLoad;
using Accelib.Tween;
using Accelix.Accelib.AccelNovel.Model;
using DG.Tweening;
using NaughtyAttributes;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Events;

namespace Accelib.Module.AccelNovel.Control.Action.Input
{
    public class ActionController_Choice : ActionController_Default
    {
        public const string CollectivePrefix = "choice_collective";
        
        public override string Keyword => "choice";
        public override bool IsPlaying => isPlaying;
        public override bool CanSkip => false;
        public override bool AutoSaveBeforeAction => true;

        [Header("연결")]
        [SerializeField] private BoolVariable isPaused;
        [SerializeField] private IntVariable affectionVar;
        
        [Header("UI")]
        [SerializeField] private GameObject dialogueCanvas;
        [SerializeField] private ChoiceDirectiveUnit directive;
        [SerializeField] private CanvasGroup optionGroup;
        [SerializeField] private List<ChoiceOptionBtnUnit> optionBtns = new();
        
        [Header("트윈")]
        [SerializeField] private DefaultTweenValue tweenValue;
        [SerializeField, Range(0.1f, 1f)] private float delayAfterSelection = 0.5f;
        
        [Header("상태")]
        [SerializeField, ReadOnly] private string directiveText;
        [SerializeField, ReadOnly] private List<ChoiceOption> options;
        [SerializeField, ReadOnly] private bool isPlaying;

        [Header("SFX")] 
        [SerializeField] private AudioRefSO directiveSfx;
        [SerializeField] private AudioRefSO optionSfx;
        [SerializeField] private AudioRefSO loveriseSfx;
        [SerializeField] private AudioRefSO lovedropSfx;
        
        private UnityAction<string> _playNextAction;

        public override void Initialize()
        {
            dialogueCanvas.SetActive(false);
            
            directive.gameObject.SetActive(false);
            directiveText = string.Empty;
            options = new List<ChoiceOption>();
            isPlaying = false;
            _playNextAction = null;
        }

        public override void PlayAction(ActionLine action, UnityAction<string> playNext)
        {
            var msg = action.arguments.GetString("msg", "");
            var jump = action.arguments.GetString("jump", null);
            var affection = action.arguments.GetInt("affection", 0);

            if (action.value == "start")
            {
                directiveText = msg;
                options = new List<ChoiceOption>();
                
                isPlaying = true;
                playNext?.Invoke(null);
            }
            else if (action.value == "option")
            {
                options.Add(new ChoiceOption { msg = msg, jump = jump, affection = affection});
                playNext?.Invoke(null);
            }
            else if(action.value == "end")
            {
                _playNextAction = playNext;
                OpenChoiceDialog();
            }
            else
            {
                Deb.LogError($"{action.value} 는 올바른 선택지 키가 아닙니다.", this);
            }
        }

        private void OpenChoiceDialog()
        {
            // 지시문 적용
            directive.Open(directiveText, tweenValue);
            
            // 옵션 적용
            for (var i = 0; i < optionBtns.Count; i++)
            {
                // 옵션 개수 이상부터 처리하지 않음
                if (i >= options.Count)
                {
                    optionBtns[i].gameObject.SetActive(false);
                    continue;
                }
                
                // 버튼에 파싱된 데이터 넘겨주기
                optionBtns[i].Open(i, options[i].msg, this);
            }
            
            // 옵션 트윈
            optionGroup.SetInteractable(false);
            optionGroup.alpha = 0f;
            optionGroup.DOFade(1f, tweenValue.duration)
                .SetEase(tweenValue.ease)
                .SetDelay(tweenValue.delay * 2f)
                .SetLink(gameObject)
                .OnStart(() => optionSfx?.PlayOneShot())
                .OnComplete(() => optionGroup.SetInteractable(true));
            
            // 일시정지 (블러처리)
            isPaused.Value = true;
            
            // 전체 활성화
            dialogueCanvas.SetActive(true);
            
            // sfx 재생
            directiveSfx?.PlayOneShot();
            
            // 자동저장
            // AutoSave().Forget();
        }

        public void OnClickOption(int id)
        {
            // 비활성
            optionGroup.SetInteractable(false);
            
            // 버튼 비활성
            for (var i = 0; i < optionBtns.Count; i++)
                if(i != id)
                    optionBtns[i].Hide();
            
            // 호감도 sfx
            var affection = options[id].affection;
            if(affection > 0)
                loveriseSfx?.PlayOneShot();
            else
                lovedropSfx?.PlayOneShot();
            
            // 컬렉티브
            var storage = SaveLoadSingleton.Get<SaveDataHolder_Collective>();
            var msg = options[id].msg;
            var collectiveKey = $"{CollectivePrefix}({msg})";
            storage.SetState(collectiveKey, CollectiveState.Unlocked);

            // 딜레이 대기 후,
            DOTween.Sequence().SetLink(gameObject)
                .AppendInterval(delayAfterSelection)
                .AppendCallback(() =>
                {
                    // 일시정지 해제 (블러 해제)
                    isPaused.Value = false;

                    // 캔버스 비활성화
                    dialogueCanvas.SetActive(false);

                    // 점프
                    var jump = options[id].jump;

                    // 호감도
                    if (affection != 0)
                        affectionVar.Add(affection);
                    
                    // 다음 재생
                    _playNextAction?.Invoke(jump);

                    // 자동저장
                    // AutoSave().Forget();
                });
        }
        
        public override void SkipAction(bool isFastForward = false) { }

        // private async UniTaskVoid AutoSave()
        // {
        //     await UniTask.DelayFrame(1);
        //     
        //     // 자동저장
        //     Novel.AutoSave("자동");
        // }
    }
}