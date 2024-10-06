using DG.Tweening;
using UnityEngine;

namespace Accelib.Module.Transition.Effect
{
    public abstract class TransitionEffect : MonoBehaviour
    {
        [Header("Option")]
        [SerializeField] protected Canvas canvas;
        [SerializeField] protected Ease easeStart = Ease.Linear;
        [SerializeField] protected Ease easeEnd = Ease.Linear;
        [SerializeField, Range(0.01f, 5f)] protected float duration = 0.2f;
        
        public abstract Sequence StartTransition();
        
        public abstract Sequence EndTransition(); 
    }
}