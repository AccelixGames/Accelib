using Accelib.Core;
using Accelib.Transition.Effect;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Transition
{
    public class TransitionHandler : MonoSingleton<TransitionHandler>
    {
        [Header("상태")]
        [SerializeField] private TransitionEffect targetEffect;
        [SerializeField, ReadOnly] private bool isMoving;

        public static UniTask StartTransition() => Instance.targetEffect.StartTransition();
        public static UniTask EndTransition() => Instance.targetEffect.EndTransition();

#if UNITY_EDITOR
        [Button]
        public void TrStart() => targetEffect.StartTransition().Forget();
        
        [Button]
        public void TrEnd() => targetEffect.EndTransition().Forget();
#endif
    }
}