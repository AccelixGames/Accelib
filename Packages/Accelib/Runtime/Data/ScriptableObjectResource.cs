using System.Linq;
using Accelib.Preview;
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
                if (!_instance) LoadInstance();
                return _instance;
            }
        }

        protected virtual void OnEnable() => LoadInstance();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void LoadInstance()
        {
            //_instance = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();
            _instance = Resources.LoadAll<T>("Accelix").FirstOrDefault();
        }
    }
}