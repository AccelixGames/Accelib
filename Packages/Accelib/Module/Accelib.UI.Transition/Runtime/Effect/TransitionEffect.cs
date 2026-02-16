using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Module.Transition.Effect
{
    public abstract class TransitionEffect : MonoBehaviour
    {
        [Header("Option")]
        [SerializeField] protected Canvas canvas;
        [SerializeField, Range(0.01f, 5f)] protected float duration = 0.2f;
        
        [SerializeField] protected Ease easeStart = Ease.Linear;
        [ShowIf("easeStart",  Ease.Unset)]
        [SerializeField] protected AnimationCurve easeStartCurve;
        [SerializeField] protected Ease easeEnd = Ease.Linear;
        [ShowIf("easeEnd",  Ease.Unset)]
        [SerializeField] protected AnimationCurve easeEndCurve;

        public bool IsActive => canvas.gameObject.activeSelf;
        
        public abstract Sequence StartTransition();
        
        public abstract Sequence EndTransition(); 
    }
}