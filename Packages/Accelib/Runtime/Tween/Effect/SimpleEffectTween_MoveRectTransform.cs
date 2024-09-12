using Accelib.Tween.Effect.Base;
using DG.Tweening;
using UnityEngine;

namespace Accelib.Tween.Effect
{
    [RequireComponent(typeof(RectTransform))]
    public class SimpleEffectTween_MoveRectTransform : SimpleEffectTween
    {
        [Header("Rect Option")]
        [SerializeField] private RectTransform startPos;
        [SerializeField] private RectTransform endPos;

        private RectTransform _rt;
        
        protected override Tweener Internal_EnableEffect(bool resetOnStart = true)
        {
            _rt ??= GetComponent<RectTransform>();
            return _rt.DOAnchorPos(endPos.anchoredPosition, Config.duration);
        }

        protected  override Tweener Internal_DisableEffect(bool resetOnStart = true)
        {
            _rt ??= GetComponent<RectTransform>();
            return _rt.DOAnchorPos(startPos.anchoredPosition, Config.duration);
        }
    }
}