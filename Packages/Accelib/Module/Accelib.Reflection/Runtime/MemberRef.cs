using System;
using System.Collections.Generic;
using System.Linq;
using Accelib.Preview;
using Accelib.Reflection.Data;
using Accelib.Reflection.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Reflection
{
    [System.Serializable]
    public class MemberRef
    {
        [HorizontalGroup("up"), SerializeField, HideLabel, OnValueChanged("OnTargetChanged")]
        private ScriptableObject target;
        [HorizontalGroup("down"), ValueDropdown("GetDropdownList", DropdownTitle = null), SerializeField, HideLabel, OnValueChanged("BuildCache")]
        private string path;

        [HorizontalGroup("up", width: 0.25f), ShowInInspector, HideLabel, ReadOnly]
        public double Value => GetValue();

        public ScriptableObject Target => target;

        public string GetPreview()
        {
            if (!target || target == null) return "";

            var name = target.name;
            if (Target is IPreviewNameProvider preview)
                name = preview.EditorPreviewName;

            return $"{name}.{path}";
        }

        /// <summary> target이 INotifyValueChanged를 구현하면 값 변경을 구독한다. </summary>
        public IDisposable Subscribe(Action<double> onChanged)
        {
            if (target is not INotifyValueChanged notifier) return null;

            void Handler() => onChanged(Value);
            notifier.OnValueChanged += Handler;

            return new CallbackDisposable(() => notifier.OnValueChanged -= Handler);
        }

        private sealed class CallbackDisposable : IDisposable
        {
            private Action _onDispose;
            public CallbackDisposable(Action onDispose) => _onDispose = onDispose;
            public void Dispose()
            {
                _onDispose?.Invoke();
                _onDispose = null;
            }
        }

#region Cache
        [NonSerialized] private CachedChain _cached;
        [NonSerialized] private Func<double> _getter;

        private double GetValue()
        {
            if (!(_cached?.IsValid ?? false)) BuildCache();
            return _getter?.Invoke() ?? 0;
        }

        private void BuildCache()
        {
            _cached = null;
            _getter = null;

            if (target == null || string.IsNullOrEmpty(path)) return;

            _cached = CachedReflectionUtility.BuildChain(target, path);
            _getter = CachedReflectionUtility.BuildDoubleGetter(target, _cached);
        }
#endregion

#if UNITY_EDITOR
        private IEnumerable<string> GetDropdownList() => ReflectionUtility.GetMemberList(target);

        private void OnTargetChanged()
        {
            path = GetDropdownList().FirstOrDefault();
            BuildCache();
        }
#endif
    }
}
