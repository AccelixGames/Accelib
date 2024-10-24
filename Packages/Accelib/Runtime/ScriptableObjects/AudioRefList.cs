using System.Collections.Generic;
using Accelib.Module.Audio.Data._Base;
using UnityEngine;

namespace Accelib.ScriptableObjects
{
    [CreateAssetMenu(fileName = "AudioRefList", menuName = "Accelib/ListSO/AudioRefList", order = 0)]
    public class AudioRefList : ScriptableObject
    {
        [SerializeField] private List<AudioRefBase> list;

        public IReadOnlyList<AudioRefBase> List => list;
    }
}