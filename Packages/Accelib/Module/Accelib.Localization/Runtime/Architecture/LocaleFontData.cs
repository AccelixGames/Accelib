using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Accelib.Module.Localization.Architecture
{
    /// <summary>
    /// 언어별 폰트 및 머테리얼 데이터.
    /// </summary>
    [System.Serializable]
    public class LocaleFontData
    {
        [field: SerializeField] public TMP_FontAsset FontAsset {get; private set;}

        [SerializeField] private List<Material> fontMaterials = new();

        public Material GetMaterial(int index)
        {
            if (fontMaterials != null && index < fontMaterials.Count && index >= 0)
                return fontMaterials[index];
            
            return FontAsset ? FontAsset.material : null;
        }

        public IReadOnlyList<Material> FontMaterials => fontMaterials;
    }
}
