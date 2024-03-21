using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Accelib.Transition.Effect
{
    internal class TransitionEffect_Door : TransitionEffect
    {
        [Header("Effect")]
        [SerializeField] private Image left;
        [SerializeField] private Image right;

        public override async UniTask StartTransition()
        {
            canvas.gameObject.SetActive(true);

            await DOTween.Sequence()
                .Join(left.rectTransform.DOLocalMoveX(0, duration).SetEase(easeStart))
                .Join(right.rectTransform.DOLocalMoveX(0, duration).SetEase(easeStart));
        }

        public override async UniTask EndTransition()
        {
            var w = canvas.renderingDisplaySize.x * 0.5f;
            
            await DOTween.Sequence()
                .Join(left.rectTransform.DOLocalMoveX(-w, duration).SetEase(easeEnd))
                .Join(right.rectTransform.DOLocalMoveX(w, duration).SetEase(easeEnd))
                .OnComplete(() => canvas.gameObject.SetActive(false));
        }
    }
}