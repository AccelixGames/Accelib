using System.Collections.Generic;
using UnityEngine;

namespace Accelib.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SpriteList", menuName = "Accelix/SpriteList", order = 0)]
    public class SpriteList : ScriptableObject
    {
        [SerializeField] private List<Sprite> sprites;

        public IReadOnlyList<Sprite> Sprites => sprites;
    }
}