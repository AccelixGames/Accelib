using Accelib.Module.UI.InfoBox.Base.Model;
using UnityEngine;
// ReSharper disable InconsistentNaming

namespace Accelib.Module.UI.InfoBox.Base.View
{
    public abstract class _InfoBoxViewBase : MonoBehaviour
    {
        protected internal abstract void DrawInfo(InfoDataBase info);
    }
}