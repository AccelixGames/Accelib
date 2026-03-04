#if UNITY_EDITOR && ODIN_INSPECTOR
using System;
using System.IO;
using System.Linq;
using Accelib.Module.SceneManagement;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Accelib.Editor.Drawer
{
    /// <summary>
    /// SceneRef 전용 Odin 드로어.
    /// 한 줄에 [라벨] [타입 Enum] [씬 이름 또는 AssetReference 필드]를 표시한다.
    /// </summary>
    public sealed class SceneRefDrawer : OdinValueDrawer<SceneRef>
    {
        private const float EnumWidth = 90f;
        private const float Spacing = 2f;

        private InspectorProperty _typeProp;
        private InspectorProperty _builtInProp;
        private InspectorProperty _addressableProp;

        protected override void Initialize()
        {
            // Children 순회로 안전하게 프로퍼티 탐색
            for (var i = 0; i < Property.Children.Count; i++)
            {
                var child = Property.Children[i];
                switch (child.Name)
                {
                    case "_type": _typeProp = child; break;
                    case "_builtInSceneName": _builtInProp = child; break;
                    case "_addressableRef": _addressableProp = child; break;
                }
            }
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            var value = ValueEntry.SmartValue;

            if (value.Type == ESceneRefType.BuiltIn)
            {
                // BuiltIn: GetControlRect 기반 단일 라인
                var rect = EditorGUILayout.GetControlRect(label != null);
                if (label != null) rect = EditorGUI.PrefixLabel(rect, label);

                DrawEnumPopup(rect, out var fieldRect);
                DrawScenePopup(fieldRect);
            }
            else
            {
                // Addressable: 수평 레이아웃
                EditorGUILayout.BeginHorizontal();

                // 라벨
                if (label != null)
                    EditorGUILayout.PrefixLabel(label);

                // 타입 Enum (고정 너비)
                EditorGUI.BeginChangeCheck();
                var newType = (ESceneRefType)EditorGUILayout.EnumPopup(
                    value.Type, GUILayout.Width(EnumWidth));
                if (EditorGUI.EndChangeCheck() && _typeProp != null)
                {
                    _typeProp.ValueEntry.WeakSmartValue = newType;
                    _typeProp.ValueEntry.ApplyChanges();
                }

                // AssetReference: Odin 기본 드로어 위임
                if (_addressableProp != null)
                    _addressableProp.Draw(null);

                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawEnumPopup(Rect rect, out Rect remaining)
        {
            var enumRect = new Rect(rect.x, rect.y, EnumWidth, rect.height);
            remaining = new Rect(
                rect.x + EnumWidth + Spacing, rect.y,
                rect.width - EnumWidth - Spacing, rect.height);

            EditorGUI.BeginChangeCheck();
            var newType = (ESceneRefType)EditorGUI.EnumPopup(enumRect, ValueEntry.SmartValue.Type);
            if (EditorGUI.EndChangeCheck() && _typeProp != null)
            {
                _typeProp.ValueEntry.WeakSmartValue = newType;
                _typeProp.ValueEntry.ApplyChanges();
            }
        }

        private void DrawScenePopup(Rect rect)
        {
            // Build Settings 씬 드롭다운 (SceneDropdownAttributeDrawer 로직 인라인)
            var scenes = EditorBuildSettings.scenes
                .Where(s => s.enabled)
                .Select(s => Path.GetFileNameWithoutExtension(s.path))
                .ToArray();

            var current = ValueEntry.SmartValue.BuiltInSceneName;
            var idx = Array.IndexOf(scenes, current);

            EditorGUI.BeginChangeCheck();
            var newIdx = EditorGUI.Popup(rect, idx, scenes);
            if (EditorGUI.EndChangeCheck() && newIdx >= 0 && _builtInProp != null)
            {
                _builtInProp.ValueEntry.WeakSmartValue = scenes[newIdx];
                _builtInProp.ValueEntry.ApplyChanges();
            }
        }
    }
}
#endif
