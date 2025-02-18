using System;
using UnityEngine;
using Accelib.Module.UI.InfoBox.Base.Model;
namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// IPair of type `&lt;Accelib.Module.UI.InfoBox.Base.Model.InfoDataBase&gt;`. Inherits from `IPair&lt;Accelib.Module.UI.InfoBox.Base.Model.InfoDataBase&gt;`.
    /// </summary>
    [Serializable]
    public struct InfoDataBasePair : IPair<Accelib.Module.UI.InfoBox.Base.Model.InfoDataBase>
    {
        public Accelib.Module.UI.InfoBox.Base.Model.InfoDataBase Item1 { get => _item1; set => _item1 = value; }
        public Accelib.Module.UI.InfoBox.Base.Model.InfoDataBase Item2 { get => _item2; set => _item2 = value; }

        [SerializeField]
        private Accelib.Module.UI.InfoBox.Base.Model.InfoDataBase _item1;
        [SerializeField]
        private Accelib.Module.UI.InfoBox.Base.Model.InfoDataBase _item2;

        public void Deconstruct(out Accelib.Module.UI.InfoBox.Base.Model.InfoDataBase item1, out Accelib.Module.UI.InfoBox.Base.Model.InfoDataBase item2) { item1 = Item1; item2 = Item2; }
    }
}