using System;

namespace Accelib.OdinExtension
{
    /// <summary>
    /// 문자열 필드에 Build Settings 씬 드롭다운을 표시하는 Odin 속성.
    /// NaughtyAttributes.Scene 대체.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class SceneDropdownAttribute : Attribute { }
}
