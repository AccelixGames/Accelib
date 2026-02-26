using System.Linq;
using Accelib.Core;
using Accelib.Flag;
using Accelib.Module.Transition.Effect;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Module.Transition
{
    public class TransitionSingleton : MonoSingleton<TransitionSingleton>
    {
        [Title("플래그")]
        [SerializeField] private SO_TokenFlag showCursor;

        [Title("설정")]
        [SerializeField] private AudioListener audioListener;
        [SerializeField] private Camera cam;
        
        [Title("상태")]
        [SerializeField] private TransitionEffect[] targetEffects;
        [SerializeField, ReadOnly] private bool isMoving;
        [SerializeField, ReadOnly] private int currIndex;

        public void ToggleAudioListener(bool enable) => audioListener.gameObject.SetActive(enable);

        private Sequence _seq;
        public static bool IsActive => Instance?.targetEffects.Any(x => x.IsActive) ?? false;

        private void Start()
        {
            currIndex = 0;
            cam.gameObject.SetActive(false);
            ToggleAudioListener(false);
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

            // 커서 플래그: 트랜지션 시작 시 Lock, 종료 시 Unlock
            if (start) showCursor?.Lock(this);
            else showCursor?.Unlock(this);
            
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
                    cam.gameObject.SetActive(true);
                    Application.backgroundLoadingPriority = ThreadPriority.High;
#if UNITY_SWITCH && !UNITY_EDITOR
                    UnityEngine.Switch.Performance.SetCpuBoostMode(UnityEngine.Switch.Performance.CpuBoostMode.FastLoad);
#endif
                });
            }
            else
            {
                _seq.AppendCallback(() => cam.gameObject.SetActive(false));
            }
            
            return _seq;
        }
    }
}