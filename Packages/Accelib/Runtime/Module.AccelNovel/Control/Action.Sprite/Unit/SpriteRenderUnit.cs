using Accelib.Data;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Module.AccelNovel.Control.Action.Sprite.Unit
{
    public class SpriteRenderUnit : MonoBehaviour
    {
        [Header("컴포넌트")]
        [SerializeField] private RenderCore dim;
        [SerializeField] private RenderCore[] renders;
        [SerializeField] private EasePairTweenConfig defaultEase;
     
        [Header("현 상태")]
        [SerializeField, ReadOnly] private int currId;
        [SerializeField, ReadOnly] private Color currColor;
        
        private Sequence _seq;

        private void OnDisable()
        {
            _seq?.Kill();
        }

        public void Initialize()
        {
            currColor = Color.white;
            foreach (var render in renders)
            {
                render.color = new Color(1, 1, 1, 0);
                render.sprite = null;
            }
            
            if (!dim.IsNull)
                dim.color = new Color(dim.color.r, dim.color.g, dim.color.b, 0);
        }

        public bool IsSameSprite(UnityEngine.Sprite sprite)
        {
            return (renders[currId].sprite == sprite);
        }

        public void SetColor(Color color)
        {
            currColor = new Color(color.r, color.g, color.b, currColor.a);
            foreach (var render in renders)
            {
                currColor.a = render.color.a;
                render.color = currColor;
            }
        }

        public Sequence DOColor(Color color, EasePairTweenConfig tConfig = null, bool anim = true)
        {
            // 렌더러가 부족하면, 종료
            if (renders is not { Length: > 1 }) return null;
            
            // 킬
            _seq?.Kill();
            _seq = DOTween.Sequence().SetLink(gameObject);
            
            // 옵션 캐싱
            tConfig ??= defaultEase;
            var duration = anim ?  tConfig.duration * 0.5f : 0f;

            var t = DOTween.To(() => currColor, x => currColor = x, color, duration).SetEase(tConfig.easeA);
            t.OnUpdate(() =>
            {
                foreach (var render in renders)
                {
                    var c = currColor;
                    c.a = render.color.a;
                    render.color = c;
                }
            });
            
            _seq.Append(t);
            return _seq;
        }

        public Sequence DoSwap(UnityEngine.Sprite sprite, EasePairTweenConfig tConfig = null, bool anim = true)
        {
            // 렌더러가 부족하면, 종료
            if (renders is not { Length: > 1 }) return null;
            
            // 같은 이미지로 변경하려고 한다면, 종료
            if(renders[currId].sprite == sprite) return _seq;
            
            // 킬
            _seq?.Kill();
            _seq = DOTween.Sequence().SetLink(gameObject);
            
            // 옵션 캐싱
            tConfig ??= defaultEase;
            var duration = anim ?  tConfig.duration * 0.5f : 0f;
            
            // ID 구하기
            var prevId = currId;
            currId = (currId + 1) % renders.Length;
            
            _seq.OnStart(() =>
            {
                renders[currId].sprite = sprite;
                renders[currId].sortingOrder = 1;
                renders[prevId].sortingOrder = 0;
            });

            var endValue = sprite == null ? 0f : 1f;

            if (!dim.IsNull)
                _seq.Append(dim.DOFade(endValue, duration).SetEase(tConfig.easeA));
            _seq.Join(renders[currId].DOFade(endValue, duration).SetEase(tConfig.easeA));
            _seq.Append(renders[prevId].DOFade(0f, duration).SetEase(tConfig.easeB));

            return _seq;
        }
    }
}