using System.Collections.Generic;
using UnityEngine;

namespace Accelib.Extensions
{
    public static class ColorExtension
    {
        /// <summary>
        /// 여러 색상(Color)을 t값(0~1)에 따라 선형 보간합니다.
        /// </summary>
        public static Color Lerp(this Color[] colors, float[] stops, float t)
        {
            if (colors == null || stops == null || colors.Length != stops.Length || colors.Length < 2)
            {
                Debug.LogError("색상 배열과 정지점 배열은 같고 2개 이상이어야 합니다.");
                return Color.black;
            }

            t = Mathf.Clamp01(t);

            // 추가: t가 가장 앞 구간보다 작으면 첫 색상 반환
            if (t <= stops[0])
                return colors[0];

            // 추가: t가 마지막 구간보다 크면 마지막 색상 반환
            if (t >= stops[^1])
                return colors[^1];

            // 구간 보간
            for (var i = 0; i < stops.Length - 1; i++)
            {
                if (!(t >= stops[i]) || !(t <= stops[i + 1])) continue;
                
                var segmentT = Mathf.InverseLerp(stops[i], stops[i + 1], t);
                return Color.Lerp(colors[i], colors[i + 1], segmentT);
            }

            return colors[colors.Length - 1]; // fallback (거의 도달하지 않음)
        }

        // List<Color>도 지원
        public static Color Lerp(this List<Color> colors, float[] stops, float t)
        {
            return colors.ToArray().Lerp(stops, t);
        }
    }
}