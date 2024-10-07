using NaughtyAttributes;
using Spine.Unity;
using UnityEngine;

namespace Accelib.Spine.Architecture
{
    [System.Serializable]
    public class SpineAnimationRefGroup
    {
            
        public SpineAnimMethod method = SpineAnimMethod.Set;
        [Range(0, 5)] public int trackId = 0;
        public AnimationReferenceAsset animRef;
        public bool loop = false;
        [ShowIf(nameof(method), SpineAnimMethod.Add), Range(0f, 5f)]
        public float delay = 0f;
    }
}