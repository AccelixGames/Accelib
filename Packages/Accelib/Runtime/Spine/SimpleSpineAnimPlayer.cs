#if ACCELIX_SPINE
using Accelib.Extensions;
using NaughtyAttributes;
using Spine.Unity;
using UnityEngine;
using AnimationState = Spine.AnimationState;

namespace Accelib.Spine
{
    //[RequireComponent(typeof(SkeletonAnimation))]
    public class SimpleSpineAnimPlayer : MonoBehaviour
    {
        [Header("Play")]
        [SerializeField] private bool playOnEnable = true;
        
        [Header("Option")]
        [SerializeField] private int trackId = 0;
        [SerializeField, SpineAnimation] private string animName;
        [SerializeField] private bool loop = true;
        [SerializeField, MinMaxSlider(0f, 10f)] private Vector2 delayRange = Vector2.zero;
        [SerializeField, MinMaxSlider(0f, 1f)] private Vector2 offsetRange = Vector2.zero;

        private SkeletonAnimation _anim;
        private SkeletonGraphic _graphic;

        private AnimationState _animState;
        
        private void Awake()
        {
            _anim = GetComponent<SkeletonAnimation>();
            _graphic = GetComponent<SkeletonGraphic>();
            
            _animState = _anim?.AnimationState ?? _graphic?.AnimationState;
            
            if (_anim == null && _graphic == null)
            {
                Debug.LogError("SkeletonAnimation이 없습니다.", this);
                Destroy(this);
            }
        }
        
        protected virtual void OnEnable()
        {
            if (playOnEnable)
                SetAnimation();
        }

        public void SetAnimation()
        {
            if(_animState == null) return;

            _animState.SetEmptyAnimation(trackId, 0f);
            var tr =_animState.AddAnimation(trackId, animName, loop, delayRange.Random());
            var offset = offsetRange.Random();
            tr.TrackTime = tr.Animation.Duration * offset;
        }
    }
}
#endif