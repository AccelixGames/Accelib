using System.Collections.Generic;

namespace Accelib.Module.Localization.Model
{
    /// <summary>
    /// LocaleKey의 직렬화 가능한 리스트 래퍼.
    /// </summary>
    [System.Serializable]
    public class LocaleKeyList
    {
        public List<LocaleKey> list;
    }
}
