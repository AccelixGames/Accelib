using System.Collections.Generic;
using Accelib.Module.UI.InfoBox.Base.Model;
// ReSharper disable InconsistentNaming

namespace Accelib.Module.UI.InfoBox.Base.Control
{
    public interface _IInfoProvider
    {
        protected internal InfoDataBase GetInfo();
        protected internal List<InfoDataBase> GetInfoList();
    }
}