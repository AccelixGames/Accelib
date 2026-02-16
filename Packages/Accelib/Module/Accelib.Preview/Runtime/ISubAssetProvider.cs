using System.Collections.Generic;
using UnityEngine;

namespace Accelib.Preview
{
    /// <summary>하위 ScriptableObject 에셋 목록을 제공하는 인터페이스.</summary>
    public interface ISubAssetProvider
    {
        public IReadOnlyList<ScriptableObject> SubAssets { get; }
    }
}
