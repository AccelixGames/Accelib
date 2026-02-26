using Accelib.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Accelib.Module.SceneManagement
{
    [CreateAssetMenu(fileName = "(Config) Scene", menuName = "Accelib/SceneConfig", order = 0)]
    public class SO_SceneConfig : ScriptableObjectResource<SO_SceneConfig>
    {
        public override string EditorPreviewName => "씬 설정";
        
        [field: SerializeField, NaughtyAttributes.Scene] public string BootScnName { get; private set; }
        [field: SerializeField, NaughtyAttributes.Scene] public string EmptyScnName { get; private set; }
        
        [field: TitleGroup("옵션", indent:true)]
        [field: SerializeField] public bool ChangeBackgroundLoadSpd { get; private set; }
        [field: SerializeField] public bool GCOnUnload { get; private set; }
        
        [TitleGroup("디버그-상태", indent:true)]
        [ShowInInspector, ReadOnly] internal bool isLocked;
        [TitleGroup("디버그-상태")]
        [ShowInInspector, ReadOnly] internal bool isLockedAdditive;
        [TitleGroup("디버그-상태")]
        [ShowInInspector, ReadOnly] private string ActiveScnName => SceneManager.GetActiveScene().name;
        
        [TitleGroup("디버그-캐싱", indent:true)]
        [ShowInInspector, ReadOnly] internal AssetReference prevScn;
        [TitleGroup("디버그-캐싱")]
        [ShowInInspector, ReadOnly] internal AssetReference currScn;

        protected override void OnEnable()
        {
            base.OnEnable();
            Init();
        }

        private void OnDisable() => Init();

        internal void Init()
        {
            isLocked = false;
            isLockedAdditive = false;
            prevScn = null;
            currScn = null;
        }
    }
}