using System;
using Accelib.Data;
using Accelib.Module.AccelNovel.Control.Utility;
using Accelib.Module.AccelNovel.Model;
using Accelix.Accelib.AccelNovel.Model;
using Accelix.Accelib.AccelNovel.Model.Enum;
using DG.Tweening;
using Febucci.UI.Core;
using NaughtyAttributes;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Events;

namespace Accelib.Module.AccelNovel.Control.Action.Transition
{
    public class ActionController_Interlude : ActionControllerT<ActionController_Interlude.Data>
    {
        [Serializable]
        public class Data
        {
            public bool isPlaying;
        }

        public override string Keyword => "interlude";
        public override bool IsPlaying => isPlaying;
        public override bool CanSkip => false;
        protected override ENextActionMode DefaultNextActionMode => ENextActionMode.Auto;

        [Header("Atom")]
        [SerializeField] private BoolVariable isUILocked;
        
        [Header("컴포넌트")]
        [SerializeField] private GameObject canvas;
        [SerializeField] private DefaultTweenConfig frameTween;
        [SerializeField] private RectTransform upperFrame;
        [SerializeField] private RectTransform lowerFrame;
        [SerializeField] private CanvasGroup[] groups;
        [SerializeField] private TypewriterCore typewriter;

        [Header("시스템")]
        [SerializeField, Range(0f, 1f)] private float canvasAlpha = 0.95f;
        [SerializeField] private string defaultText = "일주일 후..";
        [SerializeField, Range(0f, 10f)] private float defaultDelay = 1f;
        [SerializeField, Range(0f, 10f)] private float defaultDuration = 1f;
        [SerializeField, Range(0f, 10f)] private float typewriterDisappearPostDelay = 0.5f;
        [SerializeField,ReadOnly] private bool isPlaying;
        
        private Sequence _seq; 
        
        protected override void Internal_FromJson(Data data) => isPlaying = data?.isPlaying ?? false;
        protected override Data Internal_GetData() => new() { isPlaying = isPlaying };

        [Button]
        public override void Initialize()
        {
            _seq?.Kill(false);
            _seq = null;
            
            upperFrame.anchoredPosition = new Vector2(0f, upperFrame.sizeDelta.y);
            lowerFrame.anchoredPosition = new Vector2(0f, -lowerFrame.sizeDelta.y);
            
            foreach (var group in groups) 
                group.alpha = 0f;
            
            typewriter.TextAnimator.SetText("");

            canvas.SetActive(false);
        }

        public override void PlayAction(ActionLine action, UnityAction<string> playNext)
        {
            var text = action.value;
            if (string.IsNullOrEmpty(text)) text = defaultText;
            var duration = action.arguments.GetFloat("duration", defaultDuration);
            var delay = action.arguments.GetFloat("delay", defaultDelay);

            // 시작
            isUILocked.SetValue(true);
            isPlaying = true;
            canvas.SetActive(true);
            
            _seq?.Kill();
            _seq = DOTween.Sequence().SetLink(gameObject);
            
            // 초기화
            foreach (var group in groups)
                group.alpha = 1f;
            
            // 보이기
            // _seq.AppendInterval(delay);
            _seq.Append(CanvasGroupSeq(canvasAlpha));
            _seq.AppendInterval(delay);
            _seq.Join(FrameSeq(true));
            _seq.AppendCallback(() =>
            {
                typewriter.TextAnimator.SetText(text);
                typewriter.StartShowingText(true);
            });
            
            // 대기
            _seq.AppendInterval(duration);

            // 숨기기
            _seq.AppendCallback(() => typewriter.StartDisappearingText());
            _seq.AppendInterval(typewriterDisappearPostDelay);
            _seq.Append(FrameSeq(false));
            _seq.Join(CanvasGroupSeq(0f));
            _seq.AppendInterval(0.1f);
            _seq.AppendCallback(() =>
            {
                isUILocked.SetValue(false);
                isPlaying = false;
                canvas.SetActive(false);
                playNext?.Invoke(null);
            });
        }

        [Button]
        private void Test()
        {
            var text = defaultText;
            var delay = defaultDelay;
            var duration = defaultDuration;
            
            _seq?.Kill();
            _seq = DOTween.Sequence().SetLink(gameObject);
            
            // 초기화
            foreach (var group in groups)
                group.alpha = 1f;
            
            // 보이기
            _seq.AppendInterval(delay);
            _seq.Append(CanvasGroupSeq(canvasAlpha));
            _seq.Join(FrameSeq(true));
            _seq.AppendCallback(() =>
            {
                typewriter.TextAnimator.SetText(text);
                typewriter.StartShowingText(true);
            });
            
            // 대기
            _seq.AppendInterval(duration);

            // 숨기기
            _seq.AppendCallback(() => typewriter.StartDisappearingText());
            _seq.AppendInterval(typewriterDisappearPostDelay);
            _seq.Append(FrameSeq(false));
            _seq.Join(CanvasGroupSeq(0f));
            //_seq.AppendCallback(() => playNext?.Invoke(null));
        }

        private Sequence FrameSeq(bool enable)
        {
            var seq = DOTween.Sequence();
            var upper = enable ? 0f : upperFrame.sizeDelta.y;
            var lower = enable ? 0f : -lowerFrame.sizeDelta.y;
            seq.Join(upperFrame.DOAnchorPosY(upper, frameTween.duration).SetEase(frameTween.ease));
            seq.Join(lowerFrame.DOAnchorPosY(lower, frameTween.duration).SetEase(frameTween.ease));
            return seq;
        }

        private Sequence CanvasGroupSeq(float alpha)
        {
            var seq = DOTween.Sequence();
            
            foreach (var group in groups) 
                seq.Join(group.DOFade(alpha, frameTween.duration).SetEase(frameTween.ease));

            return seq;
        }
    }
}