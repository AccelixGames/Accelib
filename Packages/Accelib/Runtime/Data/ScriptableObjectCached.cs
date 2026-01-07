using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Accelib.Data
{
    public abstract class ScriptableObjectCached<T> : ScriptableObject, IPreviewNameProvider where T : ScriptableObject
    {
        [field: SerializeField, LabelText("에셋 이름"), PropertyOrder(float.MinValue + 1)]
        public string EditorPreviewName { get; private set; }
        
#if  UNITY_EDITOR
        
        private static T _editorInstance;
        [ShowInInspector, LabelText("에디터 에셋"), PropertyOrder(float.MinValue)] public static T EditorInstance
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