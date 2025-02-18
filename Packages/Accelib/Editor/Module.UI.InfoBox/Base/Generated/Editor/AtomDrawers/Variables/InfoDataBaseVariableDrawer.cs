#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityAtoms.Editor;

namespace UnityAtoms.BaseAtoms.Editor
{
    /// <summary>
    /// Variable property drawer of type `Accelib.Module.UI.InfoBox.Base.Model.InfoDataBase`. Inherits from `AtomDrawer&lt;InfoDataBaseVariable&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomPropertyDrawer(typeof(InfoDataBaseVariable))]
    public class InfoDataBaseVariableDrawer : VariableDrawer<InfoDataBaseVariable> { }
}
#endif
