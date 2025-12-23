using System.Collections.Generic;
using Accelib.Extensions;
using TMPro;
using UnityEngine;

namespace Accelib.Module.Localization.Architecture
{
    [System.Serializable]
    public class LocaleFontData
    {
        [field: SerializeField] public TMP_FontAsset FontAsset {get; private set;}
        
        [SerializeField] private List<Material> fontMaterials = new();
        
        public Material GetMaterial(int index) => fontMaterials.GetOrDefault(index, FontAsset.material);
        public IReadOnlyList<Material> FontMaterials => fontMaterials;
    }
}