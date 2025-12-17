using System.Collections.Generic;
using UnityEngine;

namespace Accelib.Module.AccelNovel.Control.Resource
{
    [CreateAssetMenu(fileName = "SO_RquireAddressableKey", menuName = "Scriptable Objects/SO_RquireAddressableKey")]
    public class SO_RquireAddressableKey : ScriptableObject
    {
        [field: SerializeField] public List<string> resourceKeys { get; private set; }
    }
}
