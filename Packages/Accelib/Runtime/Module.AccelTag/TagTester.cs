using Accelib.Module.AccelTag.TaggerSystem;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Module.AccelTag
{
    public class TagTester : MonoBehaviour
    {
        [SerializeField] private Tagger tagger;
        [SerializeField] private TagFilter filter;

        [Button]
        private void Test()
        {
            Debug.Log($"필터 테스트 결과: {filter.Filter(tagger)}");
        }
    }
}