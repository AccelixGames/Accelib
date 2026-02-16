using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Conditional
{
    /// <summary> 조건식 ScriptableObject 래퍼 </summary>
    [CreateAssetMenu(fileName = "(Conditional) Name", menuName = "Accelib/Conditional/Conditional", order = 0)]
    public class SO_Conditional : ScriptableObject
    {
        [SerializeField, InlineProperty, HideLabel] private Conditional conditional;
        public Conditional Conditional => conditional;
    }
}
