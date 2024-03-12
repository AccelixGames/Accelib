using Accelib.Data;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Effect
{
    public class UISlideEffect : MonoBehaviour
    {
        [Header("연결")]
        [SerializeField] private EasePairTweenConfig config;
        [SerializeField] private RectTransform showPos;
        [SerializeField] private RectTransform hidePos;

        [Header("옵션")]
        [SerializeField] private bool showOnStart = true;

        private RectTransform rt;
        private DG.Tweening.Tween tween;
        
        private void Awake()
        {
            rt = GetComponent<RectTransform>();
        }

        private void Start()
        {
            rt.anchoredPosition = showOnStart ? showPos.anchoredPosition : hidePos.anchoredPosition;
            gameObject.SetActive(showOnStart);
        }

        private void OnDestroy() => tween?.Kill();

        [Button] public DG.Tweening.Tween Show() => DoSlide(true, hidePos, showPos);
        [Button] public DG.Tweening.Tween Hide() => DoSlide(false, showPos, hidePos);

        private DG.Tweening.Tween DoSlide(bool show, RectTransform initPos, RectTransform endPos)
        {
            tween?.Kill(true);
            
            gameObject.SetActive(true);
            rt.anchoredPosition = initPos.anchoredPosition;

            tween = rt
                .DOAnchorPos(endPos.anchoredPosition, config.duration);

            tween.SetEase(show ? config.easeA : config.easeB);

            if (config.delay > 0f)
                tween.SetDelay(config.delay);
            if (!show)
                tween.OnComplete(() => gameObject.SetActive(false));

            return tween;
        }
    }
}