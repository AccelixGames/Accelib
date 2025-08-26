using Accelib.Data;
using Accelib.Extensions;
using Accelib.Module.AccelNovel.Control.Action.Sprite.Unit;
using DG.Tweening;
using NaughtyAttributes;
using Newtonsoft.Json;
using UnityEngine;

namespace Accelib.Module.AccelNovel.Control.Action.Character
{
    public class CharacterUnit : MonoBehaviour
    {
        [System.Serializable]
        public class Data
        {
            public string charKey;
            public string spriteKey;
            public float x;
            public float y;
            public float scl;
            public bool fade;
            [JsonConverter(typeof(ColorConverter))] public Color color;
        }
        [field: SerializeField] public Data CurrData { get; private set; }

        [Header("Character")]
        [SerializeField] private SpriteRenderUnit renderUnit;
        [SerializeField] private EasePairTweenConfig defaultEase;
        
        [Header("Emoji")]
        [SerializeField] private SpriteRenderer emojiRender;
        [SerializeField] private EasePairTweenConfig emojiEase;

        [Header("Color")]
        [SerializeField, Range(0f, 1f)] private float colorMixAlpha = 0.3f;
        [SerializeField] private Color fadeColor = new(0f, 0f, 0f, 1f);
        [SerializeField, ReadOnly] private Color finalColor = new(1f, 1f, 1f, 1f);

        private Sequence _seq;
        private Sequence _emojiSeq;
        
        public void Initialize()
        {
            gameObject.SetActive(false);
            
            CurrData.charKey = string.Empty;
            CurrData.spriteKey = string.Empty;
            
            // 색상
            CurrData.fade = false;
            CurrData.color = Color.white;
            CalculateFinalColor();
            
            // 렌더 초기화
            renderUnit.Initialize();
            emojiRender.sprite = null;
            emojiRender.color = Color.white;
            emojiRender.gameObject.SetActive(false);
            
            // 시퀀스
            _seq?.Kill(true);
            _seq = null;
            _emojiSeq?.Kill(true);
            _emojiSeq = null;
        }

        public void Skip()
        {
            _seq?.Complete(true);
            _seq = null;
            _emojiSeq?.Complete(true);
            _emojiSeq = null;
        }

        private void OnDisable()
        {
            CurrData.spriteKey = string.Empty;
            CurrData.charKey = string.Empty;
            CurrData.x = 0.5f;
            CurrData.y = 0.5f;
            CurrData.scl = 1f;
        }

        public Sequence DOSwap(string key, string spriteKey, UnityEngine.Sprite sprite, EasePairTweenConfig tConfig = null, bool anim = true)
        {
            if (CurrData.charKey == key && CurrData.spriteKey == spriteKey && sprite != null) return null;
            
            gameObject.SetActive(true);
            CurrData.charKey = key; 
            CurrData.spriteKey = spriteKey;
            
            tConfig ??= defaultEase;

            _seq = renderUnit.DoSwap(sprite, tConfig, anim);
            if (!sprite) _seq.OnComplete(Initialize);

            return _seq;
        }

        public Sequence DOMove(Vector2 pos, EasePairTweenConfig tConfig = null, bool anim = true)
        {
            tConfig ??= defaultEase;
            
            CurrData.x = pos.x;
            CurrData.y = pos.y;
            var duration = anim ? tConfig.duration : 0f;
            return _seq = DOTween.Sequence()
                .Append(renderUnit.transform.DOLocalMove(pos.ToVec3(), duration).SetEase(tConfig.easeA))
                .SetLink(gameObject);
        }
        
        public Sequence DOScale(float scl, EasePairTweenConfig tConfig = null, bool anim = true)
        {
            tConfig ??= defaultEase;
            
            CurrData.scl = scl;
            var duration = anim ? tConfig.duration : 0f;
            return _seq = DOTween.Sequence()
                .Append(renderUnit.transform.DOScale(new Vector3(scl,scl,1f), duration).SetEase(tConfig.easeA))
                .SetLink(gameObject);
        }

        public Sequence DOEmoji(UnityEngine.Sprite emojiSprite, bool anim =true)
        {
            if (emojiSprite == null) return null;

            var duration = anim ? emojiEase.duration : 0f;
            _emojiSeq?.Kill(true);
            _emojiSeq = DOTween.Sequence().SetLink(gameObject);
            _emojiSeq.OnStart(() =>
            {
                emojiRender.gameObject.SetActive(true);
                emojiRender.sprite = emojiSprite;
                emojiRender.color = Color.white;
                emojiRender.transform.localScale = Vector3.zero;
            });
            _emojiSeq.Append(emojiRender.transform.DOScale(1f, duration).SetEase(emojiEase.easeA, emojiEase.overshoot));
            _emojiSeq.AppendInterval(anim ? emojiEase.delayA : 0);
            _emojiSeq.Append(emojiRender.DOFade(0f, duration).SetEase(emojiEase.easeB));
            _emojiSeq.OnComplete(() => emojiRender.gameObject.SetActive(false));

            return _emojiSeq;
        }

        public Sequence DOColor(Color color, bool fade, EasePairTweenConfig tConfig = null, bool anim = true)
        {
            tConfig ??= defaultEase;

            CurrData.color = color;
            CurrData.fade = fade;
            CalculateFinalColor();
            
            return _seq = DOTween.Sequence()
                .Append(renderUnit.DOColor(finalColor, tConfig, anim))
                .SetLink(gameObject);
        }

        private void CalculateFinalColor()
        {
            finalColor = Color.Lerp(CurrData.color, fadeColor, CurrData.fade ? colorMixAlpha : 0f);
            finalColor.a = 1f;
        }
    }
}