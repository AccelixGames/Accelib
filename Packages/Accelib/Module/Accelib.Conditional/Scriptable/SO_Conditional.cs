using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Conditional.Scriptable
{
    [CreateAssetMenu(fileName = "(Conditional) Name", menuName = "Accelib/Conditional/Conditional", order = 0)]
    public class SO_Conditional : ScriptableObject
    {
        [SerializeField, InlineProperty, HideLabel] private Model.Conditional conditional;
        public  Model.Conditional Conditional => conditional;
    }
}