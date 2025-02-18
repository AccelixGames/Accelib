using UnityEngine;
using Accelib.Module.UI.InfoBox.Base.Model;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Event of type `InfoDataBasePair`. Inherits from `AtomEvent&lt;InfoDataBasePair&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-cherry")]
    [CreateAssetMenu(menuName = "Unity Atoms/Events/InfoDataBasePair", fileName = "InfoDataBasePairEvent")]
    public sealed class InfoDataBasePairEvent : AtomEvent<InfoDataBasePair>
    {
    }
}
