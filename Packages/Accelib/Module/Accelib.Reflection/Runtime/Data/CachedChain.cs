using System;
using System.Reflection;

namespace Accelib.Reflection.Data
{
    [Serializable]
    public sealed class CachedChain
    {
        /// <summary>각 depth의 멤버 정보임(FieldInfo 또는 PropertyInfo)</summary>
        public MemberInfo[] Chain;

        /// <summary>최종 멤버 타입임(변환/검증용)</summary>
        public Type FinalType;

        /// <summary>체인이 유효한지 여부임</summary>
        public bool IsValid => Chain is { Length: > 0 } && FinalType != null;
    }
}
