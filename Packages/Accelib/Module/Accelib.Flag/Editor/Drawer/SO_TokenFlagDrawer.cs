#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Accelib.Flag.Editor.Drawer
{
    /// <summary>
    /// SO_TokenFlag 전용 Odin 드로어.
    /// IsActive 상태를 bool 토글로 표시하고, 옆에 ObjectField로 asset을 연결할 수 있다.
    /// </summary>
    public sealed class SO_TokenFlagDrawer : OdinValueDrawer<SO_TokenFlag>
    {
        private const float ToggleWidth = 18f;
        private const float Spacing = 2f;

        protected override void DrawPropertyLayout(GUIContent label)
        {
            var flag = this.ValueEntry.SmartValue;

            // 전체 영역 확보
            var rect = EditorGUILayout.GetControlRect(label != null);

            // 라벨 영역
            if (label != null)
                rect = EditorGUI.PrefixLabel(rect, label);

            // IsActive 토글 (ReadOnly)
            var toggleRect = new Rect(rect.x, rect.y, ToggleWidth, rect.height);
            var prevEnabled = GUI.enabled;
            GUI.enabled = false;
            EditorGUI.Toggle(toggleRect, flag != null && flag.IsActive);
            GUI.enabled = prevEnabled;

            // ObjectField
            var fieldRect = new Rect(
                rect.x + ToggleWidth + Spacing,
                rect.y,
                rect.width - ToggleWidth - Spacing,
                rect.height);

            EditorGUI.BeginChangeCheck();
            var next = (SO_TokenFlag)EditorGUI.ObjectField(fieldRect, flag, typeof(SO_TokenFlag), false);
            if (EditorGUI.EndChangeCheck())
            {
                this.ValueEntry.SmartValue = next;
                this.ValueEntry.Values.ForceMarkDirty();
            }
        }
    }
}
#endif
