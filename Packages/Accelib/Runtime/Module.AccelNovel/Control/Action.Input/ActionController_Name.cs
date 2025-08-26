using System.Collections.Generic;
using System.Text.RegularExpressions;
using Accelib.Logging;
using Accelib.Module.AccelNovel.Model;
using Accelib.Tween;
using Accelib.Utility;
using Accelix.Accelib.AccelNovel.Model;
using Accelix.Accelib.AccelNovel.Model.Enum;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Events;

namespace Accelib.Module.AccelNovel.Control.Action.Input
{
    public class ActionController_Name : ActionController_Default
    {
        public override string Keyword => "name";
        public override bool IsPlaying => isPlaying;
        public override bool CanSkip => false;
        protected override ENextActionMode DefaultNextActionMode => ENextActionMode.Auto;
        private const string RegexPattern = @"^[가-힣a-zA-Z0-9-_]{1,6}$";

        [Header("연결")]
        [SerializeField] private BoolVariable isPaused;
        [SerializeField] private StringVariable playerNameVar;
        
        [Header("UI")]
        [SerializeField] private GameObject canvas;
        [SerializeField] private TMP_InputField input;
        [SerializeField] private SimpleTmpObject warningTMP;
        [SerializeField] private CanvasGroup inputGroup;
        [SerializeField] private ChoiceDirectiveUnit directive;
        [SerializeField] private TMP_FontAsset fontAsset;

        [Header("Warning")]
        [SerializeField] private List<string> warningTexts;
        
        [Header("트윈")]
        [SerializeField] private DefaultTweenValue tweenValue;
        [SerializeField, Range(0.1f, 1f)] private float delayAfterSelection = 1f;
        [SerializeField, ReadOnly] private string directiveText;
        [SerializeField, ReadOnly] private bool isPlaying;
        
        private UnityAction<string> _playNextAction;
        
        public override void Initialize()
        {
            canvas.SetActive(false);
            
            directiveText = string.Empty;
            inputGroup.interactable = true;
            inputGroup.alpha = 0f;
            inputGroup.gameObject.SetActive(false);
            isPlaying = false;
            _playNextAction = null;
        }

        public override void PlayAction(ActionLine action, UnityAction<string> playNext)
        {
            // 블러처리
            isPaused.Value = true;
            isPlaying = true;
            _playNextAction = playNext;
            canvas.SetActive(true);
            
            // 지시문 열기
            directiveText = action.value;
            directive.Open(directiveText, tweenValue);
            
            // 입력 그룹 열기
            inputGroup.interactable = true;
            input.text = playerNameVar.Value;
            inputGroup.gameObject.SetActive(true);
            inputGroup.alpha = 0f;
            inputGroup.DOFade(1f, tweenValue.duration)
                .SetEase(tweenValue.ease)
                .SetDelay(tweenValue.delay)
                .SetLink(gameObject);
        }

        public void OnClickSubmit()
        {
            var text = input.text;

            if (string.IsNullOrEmpty(text))
            {
                warningTMP.SetActive(warningTexts[0], true);
                return;
            }

            if (text.Length > 6)
            {
                warningTMP.SetActive(warningTexts[1], true);
                return;
            }

            if (!Regex.IsMatch(text, RegexPattern))
            {
                warningTMP.SetActive(warningTexts[2], true);
                return;
            }

            if (!fontAsset.HasCharacters(text))
            {
                warningTMP.SetActive(warningTexts[3], true);
                return;
            }

            inputGroup.interactable = false;
            playerNameVar.Value = text;

            DOTween.Sequence().SetLink(gameObject)
                .AppendInterval(delayAfterSelection)
                .AppendCallback(() =>
                {
                    // 일시정지 해제 (블러 해제)
                    isPaused.Value = false;
                    isPlaying = false;

                    // 캔버스 비활성화
                    canvas.SetActive(false);

                    // 다음 재생
                    _playNextAction?.Invoke(null);
                });
        }

        public override void SkipAction(bool isFastForward = false) => Deb.LogError("이름 입력은 스킵할 수 없습니다.", this);
    }
}