#if ACCELIX_SPINE
using Spine.Unity;
using UnityEngine;

namespace Accelib.Spine
{
    [RequireComponent(typeof(SkeletonAnimation))]
    public class SimpleSpineAnimPlayer : MonoBehaviour
    {
        [SerializeField] private int trackId = 0;
        [SerializeField, SpineAnimation] private string animName;
        [SerializeField] private bool loop = true;

        private SkeletonAnimation _anim;

        private void Awake()
        {
            _anim = GetComponent<SkeletonAnimation>();
            if (_anim == null)
            {
                Debug.LogError("SkeletonAnimation이 없습니다.", this);
                Destroy(this);
            }
        }

        private void OnEnable()
        {
            _anim.AnimationState.SetAnimation(trackId, animName, loop);
        }
    }
}
#endif