#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityEngine.UIElements;
using UnityAtoms.Editor;
using Accelib.Module.UI.InfoBox.Base.Model;

namespace UnityAtoms.BaseAtoms.Editor
{
    /// <summary>
    /// Event property drawer of type `Accelib.Module.UI.InfoBox.Base.Model.InfoDataBase`. Inherits from `AtomEventEditor&lt;Accelib.Module.UI.InfoBox.Base.Model.InfoDataBase, InfoDataBaseEvent&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomEditor(typeof(InfoDataBaseEvent))]
    public sealed class InfoDataBaseEventEditor : AtomEventEditor<Accelib.Module.UI.InfoBox.Base.Model.InfoDataBase, InfoDataBaseEvent> { }
}
#endif
