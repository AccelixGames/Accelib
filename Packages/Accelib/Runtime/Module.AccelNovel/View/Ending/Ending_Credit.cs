using System;
using Accelib.Data;
using Accelib.Logging;
using Accelib.Module.AccelNovel.Control.Resource;
using Accelib.Module.AccelNovel.Model;
using Accelib.Module.Audio.Data;
using Accelix.Accelib.AccelNovel.Model;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Accelib.Module.AccelNovel.View.Ending
{
    public class Ending_Credit : MonoBehaviour
    {
        [Header("Logo")]
        [SerializeField] private CanvasGroup logo;
        [SerializeField] private CanvasGroup screen;
        [SerializeField] private EasePairTweenConfig fadeTween;
        [SerializeField] private float creditDelayTime = 3f;

        [Header("Ending Info")]
        [SerializeField] private GameObject endingInfoGroup;
        [SerializeField] private Image endingImage;
        [SerializeField] private TMP_Text endingTitle;
        
        [Header("BGM")]
        [SerializeField] private string defaultBgmKey;
        [SerializeField] private AudioRefSO audioRef;

        [Header("Scroll")]
        [SerializeField] private RectTransform creditRect;
        [SerializeField] private Transform endPos;
        [SerializeField] private Transform endTarget;
        [SerializeField] private float scrollSpeed = 5f;
        [SerializeField] private GameObject endObj;
        
        [Header("Event")]
        [SerializeField] private float endDelayTime = 5f;
        [SerializeField] private UnityEvent onEndScroll;

        [SerializeField, ReadOnly] private bool isScrolling = false;
        private Sequence _fadeSeq;
        private Vector2 _targetPos;

        private void Awake()
        {
            _targetPos = creditRect.anchoredPosition;
        }

        private void OnDisable()
        {
            _fadeSeq?.Kill();
        }

        public void Open(EndingData endingData, bool skipLogo)
        {
            // 초기화
            Init();
            
            // 엔딩 정보 로드
            LoadInfo(endingData);
            
            // 크래딧 활성화
            gameObject.SetActive(true);

            // bgm 재생
            if(endingData != null)
                PlayBgm(endingData.bgmKey);
            
            // 로고 비활성화
            if (skipLogo)
            {
                logo.gameObject.SetActive(false);
                StartScroll();
            }
            else
            {
                // 로고 페이드
                _fadeSeq?.Kill();
                _fadeSeq = DOTween.Sequence()
                    .Append(logo.DOFade(1, fadeTween.duration).SetEase(fadeTween.easeA))
                    .AppendInterval(creditDelayTime)
                    .AppendCallback(StartScroll);
            }
        }

        private void StartScroll()
        {
            // 크레딧 페이드
            FadeScreen(true);
            // 스크롤 시작
            isScrolling = true;
        }
        
        private async UniTaskVoid Close()
        {
            isScrolling = false;

            endObj.SetActive(true);

            await UniTask.WaitForSeconds(endDelayTime);

            onEndScroll?.Invoke();
        }

        private void Init()
        {
            logo.alpha = 0;
            screen.alpha = 0;
        }
        
        private void Update()
        {
            UpdateScroll();
        }

        private void UpdateScroll()
        {
            if (!isScrolling) return;

            // 스크롤
            _targetPos += Vector2.up * (scrollSpeed * Time.deltaTime);
            creditRect.anchoredPosition = Vector2.Lerp(creditRect.anchoredPosition, _targetPos, 10f * Time.deltaTime);
            
            // 종료 조건
            if (endPos.position.y < endTarget.position.y)
            {
                Close().Forget();
            }
        }
        
        public void LoadInfo(EndingData data)
        {
            var isValid = data != null && data.collective?.Image;
            endingInfoGroup.SetActive(isValid);
            
            if (isValid)
            {
                endingImage.sprite = data.collective.Image;
                endingTitle.text = data.collective.Msg;
            }
        }
        
        private DG.Tweening.Tween FadeScreen(bool isFadeIn)
        {
            var endValue = isFadeIn ? 1f : 0f;
            var easeValue = isFadeIn ? fadeTween.easeA : fadeTween.easeB;
            
            return screen.DOFade(endValue, fadeTween.duration).SetEase(easeValue);
        }

        private void PlayBgm(string bgmKey)
        {
            try
            {
                var clip = ResourceProvider.Instance.GetClip(bgmKey);
                if (clip != null)
                {
                    audioRef.SetClip(clip);
                    audioRef.Play(true);
                }
            }
            catch (Exception e)
            {
                Deb.LogWarning($"{bgmKey}에 해당하는 오디오 클립을 가져올 수 없습니다.");
            }
        }
    }
}