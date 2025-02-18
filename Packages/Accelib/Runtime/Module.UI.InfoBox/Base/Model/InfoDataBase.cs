using System;

namespace Accelib.Module.UI.InfoBox.Base.Model
{
    [System.Serializable]
    public class InfoDataBase : IEquatable<InfoDataBase>
    {
        public virtual bool Equals(InfoDataBase other) => 
            ReferenceEquals(this, other);
    }
}