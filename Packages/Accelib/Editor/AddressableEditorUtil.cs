using UnityEditor.AddressableAssets;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Accelib.Editor
{
    public static class AddressableEditorUtil
    {
        public static AssetReference FindAssetReferenceByKey (string key)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
                return null;

            foreach (var group in settings.groups)
            {
                foreach (var entry in group.entries)
                {
                    Debug.Log(entry.address);
                    if (entry.address == key)
                        return new AssetReference(entry.guid);
                }
            }

            return null;
        }
    }
}