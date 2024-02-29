using DG.Tweening;
using UnityEngine;

namespace Accelib.Data
{
    [CreateAssetMenu(fileName = "config-tween-", menuName = "Accelix/Tween/EasePair", order = 0)]
    public class EasePairTweenConfig : ScriptableObject
    {
        [Range(0f, 10f)] public float delay = 0f;
        [Range(0.01f, 10f)] public float duration = 1f;
        public Ease easeA = Ease.Linear;
        public Ease easeB = Ease.Linear;
    }
}