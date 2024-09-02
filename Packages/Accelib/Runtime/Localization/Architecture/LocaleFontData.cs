using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Accelib.Localization.Architecture
{
    [System.Serializable]
    public class LocaleFontData
    {
        [field: SerializeField] public TMP_FontAsset FontAsset {get; private set;}
        
        [SerializeField] private List<Material> fontMaterials = new();
        public IReadOnlyList<Material> FontMaterials => fontMaterials;
    }
}