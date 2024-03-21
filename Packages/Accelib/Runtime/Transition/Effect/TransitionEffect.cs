using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Accelib.Transition.Effect
{
    internal abstract class TransitionEffect : MonoBehaviour
    {
        [Header("Option")]
        [SerializeField] protected Canvas canvas;
        [SerializeField] protected Ease easeStart = Ease.Linear;
        [SerializeField] protected Ease easeEnd = Ease.Linear;
        [SerializeField, Range(0.01f, 5f)] protected float duration = 0.2f;
        
        public abstract UniTask StartTransition();
        
        public abstract UniTask EndTransition(); 
    }
}