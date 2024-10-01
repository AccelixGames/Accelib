#if ACCELIX_SPINE
using Spine.Unity;
using UnityEngine;

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

        private SkeletonAnimation _anim;
        private SkeletonGraphic _graphic;

        private void Awake()
        {
            _anim = GetComponent<SkeletonAnimation>();
            _graphic = GetComponent<SkeletonGraphic>();
            if (_anim == null && _graphic == null)
            {
                Debug.LogError("SkeletonAnimation이 없습니다.", this);
                Destroy(this);
            }
        }

        public void SetAnimation()
        {
            _anim?.AnimationState.SetAnimation(trackId, animName, loop);
            _graphic?.AnimationState.SetAnimation(trackId, animName, loop);
        }

        private void OnEnable()
        {
            if (playOnEnable)
                SetAnimation();
        }
    }
}
#endif