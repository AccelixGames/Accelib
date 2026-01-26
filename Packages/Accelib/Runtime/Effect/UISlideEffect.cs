using System;
using Accelib.Data;
using DG.Tweening;
using NaughtyAttributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Effect
{
    public class UISlideEffect : MonoBehaviour
    {
        private enum ShowMode { In, Out, None }
        
        [Title("연결")]
        [SerializeField] private EasePairTweenConfig config;
        [SerializeField] private RectTransform showPos;
        [SerializeField] private RectTransform hidePos;

        [Title("옵션")]
        [SerializeField] private ShowMode enableMode = ShowMode.In;

        private RectTransform _rt;
        private DG.Tweening.Tween _tween;
        
        private void Awake()
        {
            _rt = GetComponent<RectTransform>();
            _tween = null;
        }

        private void OnEnable()
        {
            if (enableMode == ShowMode.In)
                Show();
            else if (enableMode == ShowMode.Out)
                Hide();
        }

        private void OnDestroy() => _tween?.Kill();

        [NaughtyAttributes.Button(enabledMode:EButtonEnableMode.Playmode)] 
        public DG.Tweening.Tween Show() => DoSlide(false, showPos, hidePos);
        
        [NaughtyAttributes.Button(enabledMode:EButtonEnableMode.Playmode)]
        public DG.Tweening.Tween Hide() => DoSlide(true, hidePos, showPos);

        private DG.Tweening.Tween DoSlide(bool show, RectTransform initPos, RectTransform endPos)
        {
            _tween?.Kill(true);

            gameObject.SetActive(true);
            _rt.anchoredPosition = initPos.anchoredPosition;

            _tween = _rt
                .DOAnchorPos(endPos.anchoredPosition, config.duration);

            _tween.SetEase(show ? config.easeA : config.easeB);

            if (config.delayA > 0f)
                _tween.SetDelay(config.delayA);
            // if (!show)
            //     _tween.onComplete += () => gameObject.SetActive(false);

            return _tween;
        }
    }
}