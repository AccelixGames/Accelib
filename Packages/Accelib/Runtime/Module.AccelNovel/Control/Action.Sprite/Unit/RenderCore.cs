using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

namespace Accelib.Module.AccelNovel.Control.Action.Sprite.Unit
{
    [System.Serializable]
    public class RenderCore
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Image image;

        public bool IsNull => spriteRenderer == null && image == null;
        
        public int sortingOrder
        {
            get => spriteRenderer != null ? spriteRenderer.sortingOrder : 0;
            set
            {
                if(spriteRenderer != null)
                    spriteRenderer.sortingOrder = value;
            }
            
        }
        
        public Color color
        {
            get => spriteRenderer != null ? spriteRenderer.color : image.color;
            set
            {
                if(spriteRenderer != null)
                    spriteRenderer.color = value;
                else
                    image.color = value;
            }
        }

        public UnityEngine.Sprite sprite
        {
            get => spriteRenderer != null ? spriteRenderer.sprite : image.sprite;
            set
            {
                if(spriteRenderer != null)
                    spriteRenderer.sprite = value;
                else
                    image.sprite = value;
            }
        }

        public TweenerCore<Color, Color, ColorOptions> DOFade(float endValue, float duration)
        {
            return spriteRenderer != null ? 
                spriteRenderer.DOFade(endValue, duration) : 
                image.DOFade(endValue, duration);
        }
        
        // public TweenerCore<Color, Color, ColorOptions> DOColor(Color endValue, float duration)
        // {
        //     return spriteRenderer != null ? 
        //         spriteRenderer.DOColor(endValue, duration) : 
        //         image.DOColor(endValue, duration);
        // }
    }
}