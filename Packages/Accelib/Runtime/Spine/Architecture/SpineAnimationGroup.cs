using NaughtyAttributes;
using Spine.Unity;
using UnityEngine;

namespace Accelib.Spine.Architecture
{
    [System.Serializable]
    public class SpineAnimationGroup
    {
        public enum Method {Set, Add}
            
        public Method method = Method.Set;
        [Range(0, 5)] public int trackId = 0;
        [SpineAnimation(dataField = "skeletonDataAsset")] public string animName;
        public bool loop = false;
        [ShowIf(nameof(method), Method.Add), Range(0f, 5f)]
        public float delay = 0f;
    }
}