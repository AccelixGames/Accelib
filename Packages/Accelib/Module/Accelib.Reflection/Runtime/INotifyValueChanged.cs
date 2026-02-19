using System;

namespace Accelib.Reflection
{
    /// <summary> 값 변경 알림 인터페이스. MemberRef 구독에 사용된다. </summary>
    public interface INotifyValueChanged
    {
        event Action OnValueChanged;
        void NotifyValueChanged();
    }
}
