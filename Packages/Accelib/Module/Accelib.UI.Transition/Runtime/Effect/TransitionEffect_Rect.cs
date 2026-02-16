using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Module.Transition.Effect
{
    internal class TransitionEffect_Rect : TransitionEffect
    {
        [Header("Effect")]
        [SerializeField] private RectTransform rt;
        [SerializeField] private RectTransform leftStart;
        [SerializeField] private RectTransform leftEnd;
        [SerializeField] private RectTransform rightStart;
        [SerializeField] private RectTransform rightEnd;
        
        [Header("Loading Group")]
        [SerializeField] private CanvasGroup loadingGroup;
        [SerializeField, Range(0.01f, 5f)] private float canvasGroupDuration = 0.2f;
        
        private Sequence _seq;
        
        public override Sequence StartTransition()
        {
            canvas.gameObject.SetActive(true);
            
            if (loadingGroup)
            {
                loadingGroup.gameObject.SetActive(true);
                loadingGroup.alpha = 0f;
            }
            
            _seq?.Kill();
            _seq = DoRect(rt, leftStart, leftEnd, true);
            
            if (loadingGroup)
                _seq.Append(loadingGroup.DOFade(1f, canvasGroupDuration));
            
            return _seq;
        }

        public override Sequence EndTransition()
        {
            _seq?.Kill();
            _seq = DOTween.Sequence();
            
            if (loadingGroup)
                _seq.Join(loadingGroup.DOFade(0f, canvasGroupDuration));
            
            _seq.Join(DoRect(rt, rightStart, rightEnd, false));
            _seq.onComplete += () =>
            {
                canvas.gameObject.SetActive(false);
                if (loadingGroup)
                    loadingGroup.gameObject.SetActive(false);
            };
            return _seq;
        }

        private Sequence DoRect(RectTransform target, RectTransform start, RectTransform end, bool isStart)
        {
            target.anchorMin = start.anchorMin;
            target.anchorMax = start.anchorMax;
            target.pivot = start.pivot;
            target.anchoredPosition = start.anchoredPosition;
            target.sizeDelta = start.sizeDelta;

            var sizeTween = target.DOSizeDelta(end.sizeDelta, duration);
            if (isStart)
            {
                if(easeStart == Ease.Unset)
                    sizeTween.SetEase(easeStartCurve);
                else
                    sizeTween.SetEase(easeStart);
            }
            else
            {
                if(easeEnd == Ease.Unset)
                    sizeTween.SetEase(easeEndCurve);
                else
                    sizeTween.SetEase(easeEnd);
            }
            return DOTween.Sequence().SetLink(gameObject).Join(sizeTween);
        }
    }
}