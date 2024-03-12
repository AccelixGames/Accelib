using DG.Tweening;
using UnityEngine;

namespace Accelib.Tween
{
    [System.Serializable]
    public class DefaultTweenValue
    {
        [Range(0.01f, 10f)] public float delay = 0f;
        [Range(0.01f, 10f)] public float duration = 1f;
        public Ease ease = Ease.Linear;
    }
}