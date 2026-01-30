using System.Collections.Generic;
using UnityEngine;

namespace Accelib.Preview
{
    public interface ISubAssetProvider
    {
        public IReadOnlyList<ScriptableObject> SubAssets { get; }
    }
}