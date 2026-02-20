using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        /// <summary> 체인 끝의 원본 객체를 반환한다 (double 변환 없음). Contains 등 비숫자 비교용. </summary>
        public object RawValue
        {
            get
            {
                if (!(_cached?.IsValid ?? false)) BuildCache();
                if (_cached?.Chain == null) return null;

                object current = target;
                foreach (var m in _cached.Chain)
                {
                    if (current == null) return null;
                    if (m is FieldInfo fi) current = fi.GetValue(current);
                    else if (m is PropertyInfo pi) current = pi.GetValue(current);
                }
                return current;
            }
        }

        public string GetPreview()
        {
            if (!target || target == null) return "";

            var name = target.name;
            if (Target is IPreviewNameProvider preview)
                name = preview.EditorPreviewName;

            return $"{name}.{path}";
        }

        /// <summary>
        /// 값 변경을 구독한다.
        /// 1) 체인 내 ReactiveProperty가 있으면 R3 Subscribe로 자동 구독
        /// 2) 없으면 target의 INotifyValueChanged로 fallback
        /// </summary>
        public IDisposable Subscribe(Action<double> onChanged)
        {
            // 캐시 빌드
            if (!(_cached?.IsValid ?? false)) BuildCache();

            // 1. ReactiveProperty 자동 구독 시도
            if (_cached is { ReactivePropertyChainLength: > 0 })
            {
                var rpSub = TrySubscribeReactiveProperty(onChanged);
                if (rpSub != null) return rpSub;
            }

            // 2. Fallback: INotifyValueChanged
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

#region ReactiveProperty Auto-Subscribe
        // R3 Subscribe 메서드 캐시 (Assembly 탐색은 최초 1회만)
        private static MethodInfo _r3SubscribeMethod;
        private static bool _r3SearchDone;

        /// <summary>
        /// 체인 내 ReactiveProperty 객체를 추출하고, R3 Subscribe를 리플렉션으로 호출한다.
        /// R3 어셈블리 참조 없이 동작함.
        /// </summary>
        private IDisposable TrySubscribeReactiveProperty(Action<double> onChanged)
        {
            try
            {
                // 체인을 ReactivePropertyChainLength까지 순회하여 RP 객체 추출
                object rpObj = target;
                for (var i = 0; i < _cached.ReactivePropertyChainLength; i++)
                {
                    if (rpObj == null) return null;
                    var m = _cached.Chain[i];
                    if (m is FieldInfo fi) rpObj = fi.GetValue(rpObj);
                    else if (m is PropertyInfo pi) rpObj = pi.GetValue(rpObj);
                    else return null;
                }

                if (rpObj == null) return null;

                // RP 객체에서 Observable<T>의 T를 추출
                var elementType = FindObservableElementType(rpObj.GetType());
                if (elementType == null) return null;

                // SubscribeHelper<T>를 MakeGenericMethod로 호출
                var helperMethod = typeof(MemberRef)
                    .GetMethod(nameof(SubscribeHelper), BindingFlags.NonPublic | BindingFlags.Static);
                if (helperMethod == null) return null;

                var genericHelper = helperMethod.MakeGenericMethod(elementType);
                return (IDisposable)genericHelper.Invoke(null, new object[] { rpObj, onChanged, this });
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[MemberRef] ReactiveProperty 자동 구독 실패: {e}");
                return null;
            }
        }

        /// <summary>
        /// 제네릭 T가 결정된 상태에서 R3 Subscribe를 리플렉션으로 호출한다.
        /// R3 타입을 직접 참조하지 않음 — Subscribe 확장 메서드를 리플렉션으로 찾아 호출.
        /// </summary>
        // ReSharper disable once UnusedMember.Local — 리플렉션으로 호출됨
        private static IDisposable SubscribeHelper<T>(object rpObj, Action<double> onChanged, MemberRef self)
        {
            // Action<T>: 값 무시, MemberRef.Value를 통해 double로 변환하여 콜백
            Action<T> onNext = _ => onChanged(self.Value);

            // R3 Subscribe 메서드 캐시 탐색
            if (!_r3SearchDone)
            {
                _r3SearchDone = true;
                _r3SubscribeMethod = FindR3SubscribeMethod(rpObj.GetType().Assembly);
            }

            if (_r3SubscribeMethod == null)
            {
                Debug.LogWarning("[MemberRef] R3 Subscribe 메서드를 찾지 못함");
                return null;
            }

            var genericSubscribe = _r3SubscribeMethod.MakeGenericMethod(typeof(T));
            var result = genericSubscribe.Invoke(null, new object[] { rpObj, onNext });
            return (IDisposable)result;
        }

        /// <summary> Observable<T>의 상속 트리에서 T를 추출한다. </summary>
        private static Type FindObservableElementType(Type type)
        {
            var current = type;
            while (current != null)
            {
                if (current.IsGenericType && current.GetGenericTypeDefinition().Name.StartsWith("Observable"))
                    return current.GetGenericArguments()[0];
                current = current.BaseType;
            }
            return null;
        }

        /// <summary> R3.ObservableSubscribeExtensions.Subscribe{T}(Observable{T}, Action{T}) 메서드를 찾는다. </summary>
        private static MethodInfo FindR3SubscribeMethod(Assembly r3Assembly)
        {
            var extType = r3Assembly.GetType("R3.ObservableSubscribeExtensions");
            if (extType == null)
            {
                // 다른 어셈블리에서 탐색
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    extType = asm.GetType("R3.ObservableSubscribeExtensions");
                    if (extType != null) break;
                }
            }

            if (extType == null) return null;

            foreach (var m in extType.GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                if (m.Name != "Subscribe" || !m.IsGenericMethod) continue;
                var ps = m.GetParameters();
                if (ps.Length != 2) continue;
                var p1Type = ps[1].ParameterType;
                if (p1Type.IsGenericType && p1Type.GetGenericTypeDefinition() == typeof(Action<>))
                    return m;
            }

            return null;
        }
#endregion

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
