#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using Accelib.Module.AccelNovel.Model.Collective.SO;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

namespace Accelib.Module.AccelNovel.Model.Collective.EditorTool
{
    [CreateAssetMenu(fileName = "CollectiveCreator", menuName = "Accelix/CollectiveCreator", order = 0)]
    public class SO_CollectiveCreator : ScriptableObject
    {
        private enum Type {Picture, Audio}

        [SerializeField] private Type type;
        [SerializeField] private List<Object> collectives;
        
        [Button]
        private void Create()
        {
            var assetPath = AssetDatabase.GetAssetPath(this);
            var path = Path.GetDirectoryName(assetPath);
            
            foreach (var collective in collectives)
            {
                if (type == Type.Picture)
                {
                    var so = CreateInstance<CollectiveSO_Picture>();
                    so.AssetKey = collective.name;
                    so.Title = "Title";
                    so.Description = "Desc";
                    so.Hint = "Hint";
                        
                    AssetDatabase.CreateAsset(so, Path.Combine(path, $"(Collective) {so.AssetKey}" + ".asset"));
                }
                else
                {
                    var so = CreateInstance<CollectiveSO_SoundTrack>();
                    so.AssetKey = collective.name;
                    so.Title = "Title";
                    so.Hint = "Hint";
                    so.ThumbnailKeys = null;
                        
                    AssetDatabase.CreateAsset(so, Path.Combine(path, $"(Collective) {so.AssetKey}" + ".asset"));
                }
                
                AssetDatabase.SaveAssets();
            }
        }
    }
}
#endif