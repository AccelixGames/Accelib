using DG.Tweening;
using UnityEngine;

namespace Accelib.Data
{
    [CreateAssetMenu(fileName = "ButtonTweenConfig", menuName = "Accelix/Tween/Button", order = 0)]
    public class ButtonTweenConfig : ScriptableObject
    {
        [field: Header("Down")]
        [field: SerializeField] public Vector3 DownAmount { get; private set; } = new(1.2f, 0.8f, 1f);
        [field: SerializeField, Range(0.001f, 1f)] public float DownDuration { get; private set; } = 0.15f;
        [field: SerializeField] public Ease DownEase { get; private set; } = Ease.Linear;

        [field: Header("Up")]
        [field: SerializeField, Range(0.001f, 1f)] public float UpDuration { get; private set; } = 0.1f;
        [field: SerializeField] public Ease UpEase { get; private set; } = Ease.OutBack;
    }
}