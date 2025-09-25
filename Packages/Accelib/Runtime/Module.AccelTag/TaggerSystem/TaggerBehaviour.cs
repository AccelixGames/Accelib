using System.Collections.Generic;
using UnityEngine;

namespace Accelib.Module.AccelTag.TaggerSystem
{
    public class TaggerBehaviour : MonoBehaviour, ITagger
    {
        [SerializeField] private List<SO_AccelTag> tags = new();
        
        public IEnumerable<SO_AccelTag> TagList => tags;
    }
}