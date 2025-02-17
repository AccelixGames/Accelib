using System.Collections.Generic;
using Accelib.Module.UI.InfoBox.Base.Model;

namespace Accelib.Module.UI.InfoBox.Base.Control
{
    public interface IInfoProviderT<out T> : _IInfoProvider where T : InfoDataBase
    {
        InfoDataBase _IInfoProvider.GetInfo() => ProvideInfo();

        List<InfoDataBase> _IInfoProvider.GetInfoList() => null;

        public T ProvideInfo();
    }
}