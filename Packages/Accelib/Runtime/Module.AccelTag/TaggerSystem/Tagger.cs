using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Accelib.Module.AccelTag.TaggerSystem
{
    [System.Serializable]
    public class Tagger : ITagger
    {
        // Inspector에서 편집되는 리스트
        [SerializeField] private List<SO_AccelTag> tags = new();
        public IEnumerable<SO_AccelTag> TagList => tags;
    }
}