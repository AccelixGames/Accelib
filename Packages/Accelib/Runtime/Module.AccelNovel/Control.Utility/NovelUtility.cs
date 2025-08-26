using UnityEngine;

namespace Accelib.Module.AccelNovel.Control.Utility
{
    public static class NovelUtility
    {
        public static float MultiplierValue(float value) => 0.25f * Mathf.Pow(16f, value);
    }
}