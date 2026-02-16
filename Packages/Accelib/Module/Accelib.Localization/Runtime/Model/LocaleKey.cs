namespace Accelib.Module.Localization.Model
{
    /// <summary>
    /// 로케일 키를 감싸는 직렬화 가능한 구조체.
    /// </summary>
    [System.Serializable]
    public struct LocaleKey
    {
        public string key;

        public static explicit operator string(LocaleKey localeKey) => localeKey.key;
        public static explicit operator LocaleKey(string value) => new() { key = value };

        public LocaleKey(string key = null) => this.key = key;

        public override string ToString() => key;
        
#if UNITY_EDITOR
        public string EditorPreview => 
            Accelib.Localization.EditorUtility.LocaleUtility.LocaleAsset.TryGetValue(key, out var preview) ? preview : key;
#endif
    }
}
