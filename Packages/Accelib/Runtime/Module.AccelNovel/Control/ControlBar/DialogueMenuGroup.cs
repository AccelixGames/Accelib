using Accelib.Effect;
using Accelib.Module.Audio.Data._Base;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Module.AccelNovel.Control.ControlBar
{
    public class DialogueMenuGroup : MonoBehaviour
    {
        [SerializeField] private SimpleFadeEffect fadeEff;
        [SerializeField] private SimpleMoveEffect moveEff;

        [Header("SFX")]
        [SerializeField] private AudioRefBase toggleSfx;
        
        private Sequence _sequence;

        public void Toggle()
        {
            if (_sequence != null)
                if(_sequence.IsActive() && _sequence.IsPlaying())
                    return;
            
            if (gameObject.activeSelf)
                Hide();
            else 
                gameObject.SetActive(true);
            
            toggleSfx?.PlayOneShot();
        }
        
        [Button]
        public void Hide()
        {
            if(!gameObject.activeSelf) return;
            
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            _sequence.Join(fadeEff.FadeOut(false));
            _sequence.Join(moveEff.EffectOut());
            _sequence.onComplete += () => gameObject.SetActive(false);
        }
    }
}