using System.Collections.Generic;
using Accelib.Module.UI.InfoBox.Base.Model;

namespace Accelib.Module.UI.InfoBox.Base.Control
{
    public interface IInfoListProviderT<T> : _IInfoProvider where T : InfoDataBase
    {
        InfoDataBase _IInfoProvider.GetInfo() => null;

        List<InfoDataBase> _IInfoProvider.GetInfoList()
        {
            var list = ProvideInfo();
            return list == null ? null : new List<InfoDataBase>(list);
        }

        public List<T> ProvideInfo();
    }
}