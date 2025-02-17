using Accelib.Extension.Atom.UI.Base;
using UnityAtoms;
using UnityAtoms.BaseAtoms;

namespace Accelib.Extension.Atom.UI
{
    public class SimpleFloatVariableTMP : SimpleVariableTMP<FloatVariable, float>
    {
        protected override AtomEvent<float> Changed => variable?.Changed;
        protected override float GetValue => variable?.Value ?? 0f;

    }
}