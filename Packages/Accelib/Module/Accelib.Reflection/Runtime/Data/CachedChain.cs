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

        /// <summary>
        /// 체인 내 ReactiveProperty 객체가 위치한 깊이임 (0이면 RP 없음)
        /// - 양수일 때: chain[0..Length-1]까지 순회하면 ReactiveProperty 객체를 얻을 수 있음
        /// </summary>
        public int ReactivePropertyChainLength;

        /// <summary>체인이 유효한지 여부임</summary>
        public bool IsValid => Chain is { Length: > 0 } && FinalType != null;
    }
}
