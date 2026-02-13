#if UNITY_EDITOR && ODIN_INSPECTOR
using System;
using System.Collections.Generic;
using System.Reflection;
using R3;

namespace Accelib.OdinExtension.Editor.Drawer
{
    /// <summary>
    /// SerializableReactiveProperty 드로어에서 ForceNotify 호출을 위한 공용 유틸리티.
    /// MethodInfo를 타입별로 캐시하여 반복 리플렉션 비용을 제거한다.
    /// </summary>
    internal static class ReactivePropertyDrawerHelper
    {
        private static readonly Dictionary<Type, MethodInfo> ForceNotifyCache = new();

        public static void InvokeForceNotify<T>(SerializableReactiveProperty<T> rp)
        {
            var type = rp.GetType();
            if (!ForceNotifyCache.TryGetValue(type, out var method))
            {
                method = type.GetMethod("ForceNotify",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                ForceNotifyCache[type] = method;
            }

            method?.Invoke(rp, Array.Empty<object>());
        }
    }
}
#endif
