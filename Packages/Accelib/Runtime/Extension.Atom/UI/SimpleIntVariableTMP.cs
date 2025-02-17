using Accelib.Extension.Atom.UI.Base;
using UnityAtoms;
using UnityAtoms.BaseAtoms;

namespace Accelib.Extension.Atom.UI
{
    public class SimpleIntVariableTMP : SimpleVariableTMP<IntVariable, int>
    {
        protected override AtomEvent<int> Changed => variable?.Changed;
        protected override int GetValue => variable?.Value ?? 0;
    }
}