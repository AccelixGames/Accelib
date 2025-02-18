#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityAtoms.Editor;

namespace UnityAtoms.BaseAtoms.Editor
{
    /// <summary>
    /// Event property drawer of type `InfoDataBasePair`. Inherits from `AtomDrawer&lt;InfoDataBasePairEvent&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomPropertyDrawer(typeof(InfoDataBasePairEvent))]
    public class InfoDataBasePairEventDrawer : AtomDrawer<InfoDataBasePairEvent> { }
}
#endif
