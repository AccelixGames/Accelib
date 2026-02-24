using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Conditional
{
    /// <summary> 값 제공자 ScriptableObject 추상 베이스 </summary>
    public abstract class SO_ValueProviderBase : ScriptableObject
    {
        /// <summary> 인스펙터 미리보기 문자열 </summary>
        [TitleGroup("Preview", indent:true, order:float.MaxValue), ShowInInspector, ReadOnly] 
        public abstract string Preview { get; }

        /// <summary> 평가된 double 값 </summary>
        [TitleGroup("Preview"), ShowInInspector, ReadOnly]
        public abstract double Value { get; }

        /// <summary> 값 변경 구독. 기본 구현은 구독 미지원(null 반환). </summary>
        public virtual IDisposable Subscribe(Action<double> onChanged) => null;
    }
}
