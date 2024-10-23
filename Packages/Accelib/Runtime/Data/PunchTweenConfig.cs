using UnityEngine;

namespace Accelib.Data
{
    [CreateAssetMenu(fileName = "tween-", menuName = "Accelix/Tween/Punch", order = 0)]
    public class PunchTweenConfig : ScriptableObject
    {
        [Range(0f, 10f)] public float delay = 0f;
        public Vector3 punch = Vector3.one;
        [Range(0, 10)] public int vibrato = 1;
        [Range(0.01f, 10f)] public float duration = 1f;
        [Range(0f, 1f)] public float elaticity = 0f;
    }
}