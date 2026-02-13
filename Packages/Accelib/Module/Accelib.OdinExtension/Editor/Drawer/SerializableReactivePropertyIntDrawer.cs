#if UNITY_EDITOR && ODIN_INSPECTOR
using R3;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Accelib.OdinExtension.Editor.Drawer
{
    /// <summary>
    /// SerializableReactiveProperty&lt;int&gt; 전용 Odin 드로어.
    /// R3 기본 CustomPropertyDrawer를 대체하여 순수 IntField로 편집한다.
    /// 값 변경 시 ForceNotify()로 reactive 구독자에게 알린다.
    /// </summary>
    public sealed class SerializableReactivePropertyIntDrawer
        : OdinValueDrawer<SerializableReactiveProperty<int>>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var rp = this.ValueEntry.SmartValue;
            if (rp == null)
            {
                SirenixEditorGUI.ErrorMessageBox("SerializableReactiveProperty<int> is null.");
                return;
            }

            EditorGUI.BeginChangeCheck();
            var next = EditorGUILayout.IntField(label, rp.Value);
            if (EditorGUI.EndChangeCheck())
            {
                rp.Value = next;
                this.ValueEntry.Values.ForceMarkDirty();
                ReactivePropertyDrawerHelper.InvokeForceNotify(rp);
            }
        }
    }
}
#endif
