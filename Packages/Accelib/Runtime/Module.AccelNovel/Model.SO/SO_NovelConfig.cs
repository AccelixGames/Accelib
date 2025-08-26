using System;
using System.Collections.Generic;
using System.Linq;
using Accelib.Logging;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

namespace Accelib.Module.AccelNovel.Model.SO
{
    [CreateAssetMenu(fileName = "novelConfig", menuName = "Accelib/AccelNovel/NovelConfig", order = 0)]
    public class SO_NovelConfig : ScriptableObject
    {
        [field: Header("시나리오")] 
        [field: SerializeField, Scene] public string DialogueScn { get; private set; }
        [SerializeField] private List<SO_Scenario> scenarios;
        
        public IReadOnlyList<SO_Scenario> Scenarios => scenarios;
        
        [Header("캐릭터")]
        [SerializeField] private List<SO_Character> characters;
        public SO_Character GetCharacter(string key) => characters.FirstOrDefault(x=>x.key == key);

        public SO_Scenario GetScenario(string scnKey) => scenarios.FirstOrDefault(x=>x.ScnKey == scnKey);

        public SO_Scenario GetFirstScenario() => scenarios[0];
        
        
#if UNITY_EDITOR
        [ContextMenu(nameof(LoadAllFromSheets))]
        [Button]
        private async UniTaskVoid LoadAllFromSheets()
        {
            var count = 0;
            try
            {
                foreach (var scn in scenarios)
                {
                    var progress = count / (float)scenarios.Count;
                    await scn.LoadGoogleSheetTask(progress);
                    count++;
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                Deb.Log($"시나리오 로딩됨: {count}/{scenarios.Count}개");
            }
        }
#endif
    }
}