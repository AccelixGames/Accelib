#if UNITY_EDITOR
using Accelib.Editor.Module.Localization.Utility;
using Accelib.Localization.EditorUtility;
using Accelib.Module.Localization;
using Accelib.Module.Localization.Model;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Accelib.Editor.Module.Localization.Drawer
{
    /// <summary>
    /// LocaleKey 필드의 Odin 커스텀 드로어. 키와 프리뷰 텍스트를 나란히 표시한다.
    /// </summary>
    public class LocaleKeyDrawer : OdinValueDrawer<LocaleKey>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var rect = EditorGUILayout.GetControlRect();

            if (label != null)
            {
                rect = EditorGUI.PrefixLabel(rect, label);
            }

            var value = ValueEntry.SmartValue;
            var hasKey = LocaleUtility.LocaleAsset.TryGetValue(value.key, out var preview);
            if (!hasKey) preview = LocalizationSingleton.NullString;

            GUIHelper.PushLabelWidth(20);
            value.key = EditorGUI.TextField(rect.AlignLeft(rect.width * 0.4f), value.key);

            var width = rect.AlignRight(rect.width * 0.6f - 2);

            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.TextField(width, preview);
            EditorGUI.EndDisabledGroup();

            GUIHelper.PopLabelWidth();

            ValueEntry.SmartValue = value;
        }
    }
}
#endif
