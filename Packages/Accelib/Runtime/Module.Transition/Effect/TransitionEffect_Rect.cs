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
            _seq = DoRect(rt, leftStart, leftEnd);
            
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
            
            _seq.Join(DoRect(rt, rightStart, rightEnd));
            _seq.onComplete += () =>
            {
                canvas.gameObject.SetActive(false);
                if (loadingGroup)
                    loadingGroup.gameObject.SetActive(false);
            };
            return _seq;
        }

        private Sequence DoRect(RectTransform taret, RectTransform start, RectTransform end)
        {
            taret.anchorMin = start.anchorMin;
            taret.anchorMax = start.anchorMax;
            taret.pivot = start.pivot;
            taret.anchoredPosition = start.anchoredPosition;
            taret.sizeDelta = start.sizeDelta;
            
            return DOTween.Sequence().SetLink(gameObject)
                .Join(taret.DOSizeDelta(end.sizeDelta, duration).SetEase(easeStart));
        }
    }
}