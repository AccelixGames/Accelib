using Accelib.Data;
using Accelib.Effect;
using Accelib.Module.AccelNovel.Control.Utility;
using DG.Tweening;
using Febucci.UI;
using Febucci.UI.Core;
using NaughtyAttributes;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Accelib.Module.AccelNovel.Control.Action.Dialogue
{
    /// <summary>
    /// [다이어로그 시스템]
    /// 스크립트 재생/스킵 등을 컨트롤한다.
    /// </summary>
    public class DialogueModule : MonoBehaviour
    {
        [Header("Holder")]
        [SerializeField] protected Canvas canvas;
        [SerializeField] protected RectTransform scriptHolder;
        [SerializeField] protected CanvasGroup scriptCanvasGroup;

        [Header("Detailed")]
        [SerializeField] protected SimpleFadeEffect nameTagFader;
        [SerializeField] protected TypewriterCore nameTmp;
        [SerializeField] protected TypewriterByCharacter textWriter;
        [SerializeField] protected GameObject indicator;

        [Header("Tween")]
        [SerializeField] protected RectTransform[] showHideTr;
        [SerializeField] protected EasePairTweenConfig showHideTween;

        [Header("State")]
        [SerializeField] protected IntVariable textOutputMode;
        [SerializeField] protected FloatVariable textSpd;
        [SerializeField, ReadOnly] protected float waitForNormalChars;
        [SerializeField, ReadOnly] protected float waitMiddle;
        [SerializeField, ReadOnly] protected float waitLong;
        [SerializeField, ReadOnly] protected bool isEnabled;

        protected Sequence _seq;
        
        [ShowNativeProperty] public bool IsShowingText => textWriter != null && textWriter.isShowingText;

        // 이벤트 등록
        protected void Awake()
        {
            // 타입라이터 초기화
            textWriter.onTypewriterStart.AddListener(OnTypewriteStart);
            textWriter.onTextShowed.AddListener(OnTextShowed);
            
            // 텍스트 스피드 변경 이벤트 구독
            waitForNormalChars = textWriter.waitForNormalChars;
            waitMiddle = textWriter.waitMiddle;
            waitLong = textWriter.waitLong;
            textSpd.Changed.Register(OnTextSpdChanged);
        }

        // 종료시 이벤트 구독 다 해제
        protected void OnDestroy()
        {
            // 타입라이터 이벤트 해제
            textWriter.onTypewriterStart.RemoveListener(OnTypewriteStart);
            textWriter.onTextShowed.RemoveListener(OnTextShowed);
            
            // 텍스트 스피드 변경 이벤트 구독 해제
            textSpd.Changed.Unregister(OnTextSpdChanged);
        }
        
        /// <summary>
        /// 다이어로그를 초기화한다. 
        /// </summary>
        public virtual void Initialize()
        {
            // 포지션, 알파값 초기화
            scriptHolder.anchoredPosition = showHideTr[1].anchoredPosition;
            scriptCanvasGroup.alpha = 0f;
            canvas.gameObject.SetActive(false);
            isEnabled = false;
            
            // 인디케이서 비활성화
            indicator.SetActive(false);
            
            // 타입라이터 초기화
            textWriter.TextAnimator.SetText("");
            OnTextSpdChanged(textSpd.Value);
            
            // 이름 숨기기
            nameTmp.TextAnimator.SetText("", true);
            nameTagFader.gameObject.SetActive(false);
        }
        
        protected void OnTextSpdChanged(float spd)
        {
            // 타입라이터 스피드 변경
            var multiplier = NovelUtility.MultiplierValue(spd);
            
            textWriter.waitForNormalChars = waitForNormalChars / multiplier;
            textWriter.waitMiddle = waitMiddle / multiplier;
            textWriter.waitLong = waitLong / multiplier;
            textWriter.SetTypewriterSpeed(multiplier);
        }
        
        // 타입라이터 시작 이벤트
        protected void OnTypewriteStart() => indicator.SetActive(false);
        // 타입라이트 종료 후 텍스트가 모두 보여졌을 때 실행되는 이벤트
        protected void OnTextShowed() => indicator.SetActive(true);

        /// <summary>
        /// 다이어로그 창을 띄운다.
        /// </summary>
        public virtual Sequence Show(bool show)
        {
            if (isEnabled == show) return null;
            
            // 신규 시퀀싱
            _seq?.Kill();
            _seq = DOTween.Sequence().SetLink(gameObject);

            // 목표값
            var duration = showHideTween.duration;
            var ease = show ? showHideTween.easeA : showHideTween.easeB;
            var targetPos = (show ? showHideTr[0] : showHideTr[1]).anchoredPosition;
            var targetAlpha = show ? 1f : 0f;
            
            // 위치 트윈
            _seq.Join(scriptHolder.DOAnchorPosY(targetPos.y, duration).SetEase(ease));
            // 알파 트윈
            _seq.Join(scriptCanvasGroup.DOFade(targetAlpha, duration).SetEase(ease));

            _seq.OnPlay(() =>
            {
                // 상태 설정
                isEnabled = show;
                
                // 캔버스 활성화
                canvas.gameObject.SetActive(true);
                
                // 인디케이터 비활성화
                if(show) indicator.SetActive(false);
             
                // 네임태그 초기화
                if (!show) SetNameTag(null);
                else nameTagFader.FadeIn(false);
                
                // 텍스트 초기화
                textWriter.TextAnimator.SetText("");
            });
            
            // 비활성화시,
            if (!show)
            {
                // 종료시 캔버스 비활성화
                _seq.OnComplete(() =>
                {
                    Debug.Log("캔버스 비활성화!");
                    canvas.gameObject.SetActive(false);
                });
            }
            
            return _seq;
        }

        public virtual void SetNameTag(string characterName)
        {
            if(nameTagFader == null || nameTmp == null) return;
            
            if (string.IsNullOrEmpty(characterName))
            {
                if (nameTagFader.gameObject.activeSelf)
                    nameTagFader.FadeOut(false);
            }
            else
            {
                if (!nameTagFader.gameObject.activeSelf)
                {
                    nameTagFader.gameObject.SetActive(true);
                    nameTagFader.FadeIn(false);
                }

                if (nameTmp.TextAnimator.textFull != characterName)
                {
                    nameTmp.TextAnimator.SetText(characterName, true);
                    nameTmp.StartShowingText();   
                }
            }
        }
        
        /// <summary>
        /// 텍스트 재생을 시작한다.
        /// </summary>
        public virtual void ShowText(string text)
        {
            text = text.TrimStart().TrimEnd();
            
            _seq?.Complete();
            if (textWriter.isActiveAndEnabled && textWriter.gameObject.activeSelf)
            {
                if(!string.IsNullOrEmpty(text))
                    textWriter.ShowText(text);
                if(textOutputMode.Value != 0)
                    textWriter.SkipTypewriter();   
            }
        }
        
        // 텍스트 재생을 스킵한다.
        public virtual void SkipText() => textWriter.SkipTypewriter();

        // 디버그 함수들
#if UNITY_EDITOR
        [Button] private void Deb_Init() => Initialize();
        [Button] private void Deb_StartText() => ShowText("테스트 텍스트입니다. 엄청 길다길다길다.\n 끝났습니다!!!");
        [Button] private void Deb_SkipText() => SkipText();
        [Button] private void Deb_Show() => Show(true);
        [Button] private void Deb_Hide() => Show(false);
        [Button] private void Deb_TestName() => SetNameTag("이름" + Random.Range(0, 999));
        [Button] private void Deb_HideName() => SetNameTag(null);
#endif
    }
}