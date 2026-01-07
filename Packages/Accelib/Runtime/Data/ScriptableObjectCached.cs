using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Accelib.Data
{
    public abstract class ScriptableObjectCached<T> : ScriptableObject where T : ScriptableObject
    {
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