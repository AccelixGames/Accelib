namespace Accelib.Module.Localization.Model
{
    [System.Serializable]
    public struct LocaleKey
    {
        public string key;
        
        public static explicit operator string(LocaleKey localeKey) => localeKey.key;
        public static explicit operator LocaleKey(string value) => new() { key = value };
    }
}