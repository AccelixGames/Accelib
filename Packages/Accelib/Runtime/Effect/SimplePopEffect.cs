using System;
using Accelib.Data;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Effect
{
    public class SimplePopEffect : MonoBehaviour
    {
        [SerializeField] private EasePairTweenConfig config;

        private static readonly Vector3 Small = Vector3.one * 0.0001f;

        private DG.Tweening.Tween _tween;
        
        private void OnEnable()
        {
            _tween?.Kill();
            
            transform.localScale = Small;
            _tween = transform.DOScale(Vector3.one, config.duration)
                .SetEase(config.easeA)
                .SetDelay(config.delay);
        }

        [Button]
        public void SetDisable()
        {
            if(!gameObject.activeSelf) return;
            
            _tween?.Kill();
            
            _tween = transform.DOScale(Small, config.duration)
                .SetEase(config.easeB)
                .SetDelay(config.delay)
                .OnComplete(() => gameObject.SetActive(false));
        }
    }
}