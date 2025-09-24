using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace Accelib.ShaderGraph
{
    [RequireComponent(typeof(Image))]
    [ExecuteAlways]
    public class UIOutline : MonoBehaviour
    {
        private static readonly int Thickness = Shader.PropertyToID("_thickness");

        [Header("# 두께(픽셀)")]
        [SerializeField] private float outlineThickness = 10f;
        
        [Header("# 컴포넌트")]
        [SerializeField, ReadOnly] private Image image;
        [SerializeField, ReadOnly] private float thicknessRatio;

        private RectTransform _rt;
        private Material _mat;
        
        private void Awake()
        {
            image = GetComponent<Image>();
            _rt = image.rectTransform;
            _mat = image.material;
        }

        private void LateUpdate()
        {
            if (!_rt || !_mat) return;
            
            thicknessRatio = Mathf.Clamp01(outlineThickness / (_rt.rect.width * _rt.lossyScale.x));
            _mat.SetFloat(Thickness, thicknessRatio);
        }

        private void Reset()
        {
            Awake();
            LateUpdate();
        }
    }
}
