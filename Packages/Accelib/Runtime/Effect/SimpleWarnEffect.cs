using System;
using Accelib.Tween;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Effect
{
    [RequireComponent(typeof(CanvasGroup))]
    public class SimpleWarnEffect : MonoBehaviour
    {
        [SerializeField] private int vibrateCount = 5;
        [SerializeField] private float duration = 1f;
        [SerializeField] private float strength = 1f;
        [SerializeField] private bool disableOnEnd = true;
        [SerializeField] private DefaultTweenValue fadeOutTween;

        private CanvasGroup _group;
        private Sequence _seq;

        private void Awake()
        {
            _group = GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            DoWarn();
        }

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void DoWarn()
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
                return;
            }
            
            _seq?.Kill();
            _seq = null;
            
            _group.alpha = 1f;
            
            var str = strength * 0.5f;
            var dur = duration / vibrateCount;
            
            var p = transform.localPosition;
            p.x = -str;
            transform.localPosition = p;
            
            _seq = DOTween.Sequence();
            for (var i = 0; i < vibrateCount; i++)
            {
                var x = i % 2 == 0 ? str : -str;
                _seq.Append(transform.DOLocalMoveX(x, dur).SetEase(Ease.Linear));
            }

            if (disableOnEnd)
            {
                _seq.Append(_group.DOFade(0f, fadeOutTween.duration)
                    .SetEase(fadeOutTween.ease).SetDelay(fadeOutTween.delay));
                _seq.onComplete += () => gameObject.SetActive(false);
            }
        }
    }
}