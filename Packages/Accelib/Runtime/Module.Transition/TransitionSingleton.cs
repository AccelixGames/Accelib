using System.Linq;
using Accelib.Core;
using Accelib.Module.Transition.Effect;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Module.Transition
{
    public class TransitionSingleton : MonoSingleton<TransitionSingleton>
    {
        [Header("상태")]
        [SerializeField] private Camera cam;
        [SerializeField] private TransitionEffect[] targetEffects;
        [SerializeField, ReadOnly] private bool isMoving;
        [SerializeField, ReadOnly] private int currIndex;

        // [Header("오디오")]
        // [SerializeField, Range(0f, 1f)] private float transitionControlVolume = 1f;

        private Sequence _seq;
        public static bool IsActive => Instance?.targetEffects.Any(x => x.IsActive) ?? false;

        private void Start()
        {
            currIndex = 0;
            cam.gameObject.SetActive(false);
        }
    
        [HorizontalGroup("# 테스트")]
        [Button("트렌지션 시작", DrawResult = false, DisplayParameters = true, Style = ButtonStyle.FoldoutButton)]
        public static Sequence StartTransition(int index = 0)
        {
            if (!Instance) 
                return null;

            Instance.currIndex = index;
            return Instance.Transition(true);
        }

        [HorizontalGroup("# 테스트")]
        [Button("트렌지션 종료", DrawResult = false, DisplayParameters = true, Style = ButtonStyle.FoldoutButton)]
        public static Sequence EndTransition()
        {
            if (!Instance) 
                return null;

            return Instance.Transition(false);
        }

        private Sequence Transition(bool start)
        {
            _seq?.Kill();
            _seq = DOTween.Sequence();
            
            cam.gameObject.SetActive(start);
            
            // 끝
            if(!start)
                _seq.AppendCallback(() =>
                {
                    Application.backgroundLoadingPriority = ThreadPriority.Low;
#if UNITY_SWITCH && !UNITY_EDITOR
                    UnityEngine.Switch.Performance.SetCpuBoostMode(UnityEngine.Switch.Performance.CpuBoostMode.Normal);
#endif
                }).AppendInterval(0.2f);

            var eff = targetEffects[currIndex];
            _seq.Append(start ? eff.StartTransition() : eff.EndTransition());
            
            // 콜백
            // var targetVolume = start ? transitionControlVolume : 1f;
            // _seq.AppendCallback(() => AudioSingleton.SetControlVolume(AudioChannel.Bgm, targetVolume));
            
            // 시작
            if (start)
            {
                _seq.AppendCallback(() =>
                {
                    Application.backgroundLoadingPriority = ThreadPriority.High;
#if UNITY_SWITCH && !UNITY_EDITOR
                    UnityEngine.Switch.Performance.SetCpuBoostMode(UnityEngine.Switch.Performance.CpuBoostMode.FastLoad);
#endif
                });
            }
            
            return _seq;
        }
    }
}