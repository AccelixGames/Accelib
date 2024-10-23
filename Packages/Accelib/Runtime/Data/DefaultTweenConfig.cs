using DG.Tweening;
using UnityEngine;

namespace Accelib.Data
{
    [CreateAssetMenu(fileName = "tween-", menuName = "Accelix/Tween/Default", order = 0)]
    public class DefaultTweenConfig : ScriptableObject
    {
        [Range(0f, 10f)] public float delay = 0f;
        [Range(0.01f, 10f)] public float duration = 1f;
        public Ease ease = Ease.Linear;
        public float overshoot = 1.78f;
    }
}