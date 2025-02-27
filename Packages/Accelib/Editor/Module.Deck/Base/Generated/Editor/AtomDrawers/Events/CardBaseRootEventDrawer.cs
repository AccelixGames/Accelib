#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityAtoms.Editor;

namespace UnityAtoms.BaseAtoms.Editor
{
    /// <summary>
    /// Event property drawer of type `Accelib.Module.Deck.Base.View.CardBaseRoot`. Inherits from `AtomDrawer&lt;CardBaseRootEvent&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomPropertyDrawer(typeof(CardBaseRootEvent))]
    public class CardBaseRootEventDrawer : AtomDrawer<CardBaseRootEvent> { }
}
#endif
