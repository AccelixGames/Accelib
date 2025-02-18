using System.Collections.Generic;
using Accelib.Module.UI.InfoBox.Base.Model;

// ReSharper disable InconsistentNaming

namespace Accelib.Module.UI.InfoBox.Base.Control.Provider.Interface
{
    public interface _IInfoProvider
    {
        protected internal InfoDataBase Provide();
    }
}