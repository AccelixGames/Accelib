using UnityEditor;
using UnityAtoms.Editor;
using Accelib.Module.UI.InfoBox.Base.Model;

namespace UnityAtoms.BaseAtoms.Editor
{
    /// <summary>
    /// Variable Inspector of type `Accelib.Module.UI.InfoBox.Base.Model.InfoDataBase`. Inherits from `AtomVariableEditor`
    /// </summary>
    [CustomEditor(typeof(InfoDataBaseVariable))]
    public sealed class InfoDataBaseVariableEditor : AtomVariableEditor<Accelib.Module.UI.InfoBox.Base.Model.InfoDataBase, InfoDataBasePair> { }
}
