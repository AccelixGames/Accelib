using Accelib.Module.Localization.EditorTool;

namespace Accelib.Module.Localization.Model
{
    [System.Serializable]
    public struct LocaleKey
    {
        public string key;
        
        public static explicit operator string(LocaleKey localeKey) => localeKey.key;
        public static explicit operator LocaleKey(string value) => new() { key = value };

        public override string ToString() => key;
        
#if UNITY_EDITOR
        public string EditorPreview => 
            LocaleUtility.LocaleAsset.TryGetValue(key, out var preview) ? preview : key;
#endif
    }
}