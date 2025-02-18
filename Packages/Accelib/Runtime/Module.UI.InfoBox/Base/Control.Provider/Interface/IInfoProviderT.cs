using System.Collections.Generic;
using Accelib.Module.UI.InfoBox.Base.Model;

namespace Accelib.Module.UI.InfoBox.Base.Control.Provider.Interface
{
    public interface IInfoProviderT<out T> : _IInfoProvider where T : InfoDataBase
    {
        InfoDataBase _IInfoProvider.Provide() => ProvideInfo();

        public T ProvideInfo();
    }
}