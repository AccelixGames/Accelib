using UnityEngine;

namespace Accelib.Conditional.Scriptable
{
    //[CreateAssetMenu(fileName = "(Conditional-Value) Name", menuName = "Accelib/Conditional/ValueProvider", order = 1)]
    public abstract class SO_ValueProviderBase : ScriptableObject
    {
        public abstract string Preview { get; }
        public abstract double Value { get; }
    }
}