#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityEngine.UIElements;
using UnityAtoms.Editor;
using Accelib.Module.UI.InfoBox.Base.Model;

namespace UnityAtoms.BaseAtoms.Editor
{
    /// <summary>
    /// Event property drawer of type `InfoDataBasePair`. Inherits from `AtomEventEditor&lt;InfoDataBasePair, InfoDataBasePairEvent&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomEditor(typeof(InfoDataBasePairEvent))]
    public sealed class InfoDataBasePairEventEditor : AtomEventEditor<InfoDataBasePair, InfoDataBasePairEvent> { }
}
#endif
