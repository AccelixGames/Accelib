using System.Linq;
using Accelib.EditorTool.Provider;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Data
{
    public abstract class ScriptableObjectResource<T> : ScriptableObject , IPreviewNameProvider, IPreviewIconProvider where T : ScriptableObject
    { 
        public abstract string EditorPreviewName { get; }
        public virtual SdfIconType EditorPreviewIcon => SdfIconType.None;
        
        private static T _instance;
        
        [FoldoutGroup("에셋 정보", Order = float.MinValue, Expanded = true)]
        [ShowInInspector, HideLabel]
        public static T Instance
        {
            get
            {
                _instance ??= Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();
                return _instance;
            }
        }
    }
}