using System.Linq;
using Accelib.EditorTool.Provider;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Accelib.Data
{
    public abstract class ScriptableObjectCached<T> : ScriptableObject, IPreviewNameProvider, IPreviewIconProvider where T : ScriptableObject
    {
        [field: HorizontalGroup("에디터 에셋/그룹", Title = "", Order = float.MinValue + 1)]
        [field: SerializeField, HideLabel] public string EditorPreviewName { get; private set; }
        [field: HorizontalGroup("에디터 에셋/그룹")]
        [field: SerializeField, HideLabel] public SdfIconType EditorPreviewIcon { get; private set; }
        
#if  UNITY_EDITOR
        private static T _editorInstance;
        [FoldoutGroup("에디터 에셋", Order = float.MinValue, Expanded = true)]
        [ShowInInspector, HideLabel] public static T EditorInstance
        {
            get
            {
                var typeName = typeof(T).Name;
                _editorInstance ??= AssetDatabase.FindAssetGUIDs($"t: {typeName}")
                    .Select(AssetDatabase.LoadAssetByGUID<T>).FirstOrDefault();
                return _editorInstance;
            }
        }
#endif
    }
}