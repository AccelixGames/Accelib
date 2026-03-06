using Sirenix.OdinInspector;
using Sonity;
using UnityEngine;

namespace Accelib.SonityExtension.Runtime
{
    /// <summary>
    /// Intensity 기반 루프 사운드 재생기.
    /// Play(duration)으로 재생하면, duration 동안 Intensity가 startIntensity→1로 자동 보간된다.
    /// SoundContainer의 Intensity→Pitch/Volume 커브를 활용하여 인스펙터에서 사운드 변조를 설정한다.
    /// </summary>
    public class LoopSoundPlayer : MonoBehaviour
    {
        [SerializeField] private SoundEvent loopSound;
        [SerializeField, Range(0f, 1f)] private float startIntensity;

        private SoundParameterIntensity _intensityParam;
        private bool _isPlaying;
        private float _duration;
        private float _elapsed;

        /// <summary>
        /// 현재 재생 중인지 여부.
        /// </summary>
        [TitleGroup("Debug"), ShowInInspector, ReadOnly]
        public bool IsPlaying => _isPlaying;

        /// <summary>
        /// 현재 Intensity 값 (0~1).
        /// </summary>
        [TitleGroup("Debug"), ShowInInspector, ReadOnly, ProgressBar(0f, 1f)]
        public float Intensity => _intensityParam?.Intensity ?? 0f;

        private void Awake()
        {
            _intensityParam = new SoundParameterIntensity(startIntensity, UpdateMode.Continuous);
        }

        private void Update()
        {
            if (!_isPlaying) return;
            if (_duration <= 0f) return;

            _elapsed += Time.deltaTime;

            // startIntensity → 1 보간
            var t = Mathf.Clamp01(_elapsed / _duration);
            _intensityParam.Intensity = Mathf.Lerp(startIntensity, 1f, t);
        }

        private void OnDisable()
        {
            if (_isPlaying)
                Stop(false);
        }

        /// <summary>
        /// 루프 사운드를 재생한다. duration 동안 Intensity가 startIntensity→1로 자동 상승한다.
        /// </summary>
        /// <param name="duration">Intensity가 1에 도달하는 시간 (초).</param>
        [TitleGroup("Debug"), Button, EnableIf("@UnityEngine.Application.isPlaying && !_isPlaying")]
        public void Play(float duration)
        {
            if (_isPlaying) return;

            _isPlaying = true;
            _duration = Mathf.Max(0f, duration);
            _elapsed = 0f;
            _intensityParam.Intensity = startIntensity;
            loopSound?.Play(transform, _intensityParam);
        }

        /// <summary>
        /// 루프 사운드를 정지한다.
        /// </summary>
        /// <param name="allowFadeOut">페이드아웃 허용 여부. 기본값 true.</param>
        [TitleGroup("Debug"), Button, EnableIf("@UnityEngine.Application.isPlaying && _isPlaying")]
        public void Stop(bool allowFadeOut = true)
        {
            if (!_isPlaying) return;

            _isPlaying = false;
            _duration = 0f;
            _elapsed = 0f;
            loopSound?.Stop(transform, allowFadeOut);
        }
    }
}
