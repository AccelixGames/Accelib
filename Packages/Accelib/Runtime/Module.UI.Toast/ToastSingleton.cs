using Accelib.Core;
using Accelib.Data;
using Accelib.Helper;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

namespace Accelib.Module.UI.Toast
{
    public class ToastSingleton : MonoSingleton<ToastSingleton>
    {
        [Header("Rect")]
        [SerializeField] private TMP_Text msgTMP; 
        [SerializeField] private RectTransform target;
        [SerializeField] private RectTransform showPos;
        [SerializeField] private RectTransform hidePos;

        [Header("Tween")]
        [SerializeField] private Timer timer;
        [SerializeField] private EasePairTweenConfig config;
        
        private Tweener _tween;
        
        public static void Open(string msg) => Instance?.Internal_Open(msg);

        private void Start()
        {
            _tween?.Kill();
            target.gameObject.SetActive(false);
        }

        private void Internal_Open(string msg)
        {
            _tween?.Kill();
            
            msgTMP.text = msg;
            target.gameObject.SetActive(true);
            target.anchoredPosition = hidePos.anchoredPosition;
            
            _tween = target.DOAnchorPosX(showPos.anchoredPosition.x, config.duration).SetEase(config.easeA);
            timer.Clear();
        }

        private void Update()
        {
            if (timer.OnTime())
            {
                _tween?.Kill();
                _tween = target.DOAnchorPosX(hidePos.anchoredPosition.x, config.duration).SetEase(config.easeB);
                _tween.onComplete += () => target.gameObject.SetActive(false);
            }
        }

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        private void TestOpen()
        {
            Internal_Open("토스트 메세지입니다!");
        }
    }
}