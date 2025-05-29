using Accelib.Module.Audio.Data._Base;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Accelib.Module.UI.Utility
{
    public class UIEventAudio : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler, ISubmitHandler
    {
        [Header("Mode")]
        [SerializeField] private bool playOneShot = true;
        [SerializeField, HideIf(nameof(playOneShot))] private bool fadeOnPlay;
        
        [Header("")]
        [SerializeField] private AudioRefBase onPointerDown;
        [SerializeField] private AudioRefBase onPointerUp;
        [SerializeField] private AudioRefBase onPointerClick;
        [SerializeField] private AudioRefBase onSelect;
        [SerializeField] private AudioRefBase onDeselect;
        [SerializeField] private AudioRefBase onSubmit;

        private void PlaySfx(AudioRefBase audioRef)
        {
            if(playOneShot)
                audioRef?.PlayOneShot();
            else
                audioRef?.Play(fadeOnPlay);
        }
        
        public void OnPointerDown(PointerEventData eventData) => PlaySfx(onPointerDown);

        public void OnPointerUp(PointerEventData eventData) => PlaySfx(onPointerUp);

        public void OnPointerClick(PointerEventData eventData) => PlaySfx(onPointerClick);

        public void OnSelect(BaseEventData eventData) => PlaySfx(onSelect);

        public void OnDeselect(BaseEventData eventData) => PlaySfx(onDeselect);
        public void OnSubmit(BaseEventData eventData) => PlaySfx(onSubmit);
    }
}