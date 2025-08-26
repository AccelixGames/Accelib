using Accelib.Data;
using Accelib.Helper;
using Accelib.Module.AccelNovel.Control.Utility;
using Accelib.Module.Audio.Data._Base;
using DG.Tweening;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Events;

namespace Accelib.Module.AccelNovel.Control.ControlBar
{
    public class ControlBarHandler : MonoBehaviour
    {
        [Header("Canvas")]
        [SerializeField] private EasePairTweenConfig config;
        [SerializeField] private CanvasGroup[] canvasGroups;
        [SerializeField] private DialogueMenuGroup menuGroup;
        
        [Header("일시정지/숨기기")]
        [SerializeField] private BoolVariable isUILocked;
        [SerializeField] private BoolVariable isPaused;
        [SerializeField] private BoolVariable isHideUI;
        
        [Header("Autoplay")]
        [SerializeField] private BoolVariable isAutoPlaying;
        [SerializeField] private GameObject onAutoplayObj;
        [SerializeField] private GameObject offAutoplayObj;
        
        [Header("Fast Forward")]
        [SerializeField] private NovelController novelController;
        [SerializeField] private FloatVariable fastForwardSpeed;
        [SerializeField] private Timer fastForwardTimer;

        [Header("Log")]
        [SerializeField] private AudioRefBase openLogSfx;
        
        [Header("Event")]
        [SerializeField] private UnityEvent onOpenLog;
        
        private bool IsLocked => isPaused.Value || isUILocked.Value;
        private Sequence _seq;

        private void Start() => isHideUI.Value = false;
        private void OnEnable()
        {
            isPaused.Changed.Register(ForceShow);
            isAutoPlaying.Changed.Register(OnAutoplayStateChanged);

            OnAutoplayStateChanged(isAutoPlaying.Value);
        }

        private void OnDisable()
        {
            isPaused.Changed.Unregister(ForceShow);
            isAutoPlaying.Changed.Unregister(OnAutoplayStateChanged);

            // isAutoPlaying.Value = false;
        }

        private void Update()
        {
            if (IsLocked) return;

            if (isHideUI.Value ? Input.anyKeyDown : Input.GetMouseButtonDown(1))
            {
                isHideUI.Value = !isHideUI.Value;
                Hide();
            }
            
            // 설정
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                menuGroup.Toggle();
                isAutoPlaying.Value = false;
            }
            
            // 자동 재생 토글
            if(Input.GetKeyDown(KeyCode.A))
                isAutoPlaying.Value = !isAutoPlaying.Value;
            
            // 빠른 저장
            if (Input.GetKeyDown(KeyCode.S)) 
                novelController.AutoSave("빠른");
            
            // 로그
            if (Input.GetKeyDown(KeyCode.H) || Input.GetKeyDown(KeyCode.Backspace) || Input.GetMouseButtonDown(2))
            {
                openLogSfx?.PlayOneShot();
                onOpenLog?.Invoke();
            }

            // 빨리감기
            if (!isHideUI.Value)
            {
                if (Input.GetKeyDown(KeyCode.LeftControl)) 
                    fastForwardTimer.Clear();
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    if (fastForwardTimer.OnTime(false, NovelUtility.MultiplierValue(fastForwardSpeed.Value)))
                    {
                        novelController.OnClick(true);
                        fastForwardTimer.Clear();
                    }
                }
            }
        }

        private void OnAutoplayStateChanged(bool value)
        {
            onAutoplayObj.SetActive(!value);
            offAutoplayObj.SetActive(value);
        }

        private void ForceShow(bool paused)
        {
            if (paused && isHideUI.Value)
            {
                isHideUI.Value = false;
                Hide();
            }
        }
        
        private DG.Tweening.Tween Hide()
        {
            _seq?.Kill();
            _seq = DOTween.Sequence();
            _seq.SetLink(gameObject);

            var target = isHideUI.Value ? 0f : 1f;
            var duration = config.duration;
            var ease = isHideUI.Value ? config.easeA : config.easeB;
            
            foreach (var canvasGroup in canvasGroups)
            {
                _seq.Join(canvasGroup.DOFade(target, duration).SetEase(ease));
                _seq.JoinCallback(() =>
                {
                    canvasGroup.interactable = !isHideUI.Value;
                    canvasGroup.blocksRaycasts = !isHideUI.Value;
                });    
            }
            
            return _seq;
        }
    }
}