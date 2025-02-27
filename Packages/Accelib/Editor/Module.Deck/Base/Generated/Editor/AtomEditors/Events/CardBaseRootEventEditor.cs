#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityEngine.UIElements;
using UnityAtoms.Editor;
using Accelib.Module.Deck.Base.View;

namespace UnityAtoms.BaseAtoms.Editor
{
    /// <summary>
    /// Event property drawer of type `Accelib.Module.Deck.Base.View.CardBaseRoot`. Inherits from `AtomEventEditor&lt;Accelib.Module.Deck.Base.View.CardBaseRoot, CardBaseRootEvent&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomEditor(typeof(CardBaseRootEvent))]
    public sealed class CardBaseRootEventEditor : AtomEventEditor<Accelib.Module.Deck.Base.View.CardBaseRoot, CardBaseRootEvent> { }
}
#endif
