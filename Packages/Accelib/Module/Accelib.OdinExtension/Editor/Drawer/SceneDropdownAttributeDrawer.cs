#if UNITY_EDITOR && ODIN_INSPECTOR
using System;
using System.IO;
using System.Linq;
using Accelib.OdinExtension;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Accelib.OdinExtension.Editor.Drawer
{
    /// <summary>
    /// SceneDropdownAttribute 전용 Odin 드로어.
    /// Build Settings에 등록된 씬 목록을 Popup으로 표시한다.
    /// </summary>
    public sealed class SceneDropdownAttributeDrawer
        : OdinAttributeDrawer<SceneDropdownAttribute, string>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            // Build Settings 씬 수집
            var scenes = EditorBuildSettings.scenes
                .Where(s => s.enabled)
                .Select(s => Path.GetFileNameWithoutExtension(s.path))
                .ToArray();

            var current = this.ValueEntry.SmartValue;
            var idx = Array.IndexOf(scenes, current);

            // Popup
            EditorGUI.BeginChangeCheck();
            var newIdx = EditorGUILayout.Popup(label, idx, scenes);
            if (EditorGUI.EndChangeCheck() && newIdx >= 0)
                this.ValueEntry.SmartValue = scenes[newIdx];
        }
    }
}
#endif
