using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Flag
{
    /// <summary>
    /// 토큰 기반 플래그 ScriptableObject.
    /// 여러 시스템이 MonoBehaviour를 토큰으로 사용하여 플래그를 활성화/비활성화할 수 있다.
    /// </summary>
    [CreateAssetMenu(menuName = "Accelib/TokenFlag")]
    public class SO_TokenFlag : ScriptableObject
    {
        [ShowInInspector, ReadOnly] private readonly HashSet<MonoBehaviour> _lockTokens = new();

        /// <summary>활성 상태인지 여부. 하나 이상의 토큰이 잠금 중이면 true.</summary>
        public bool IsActive => _lockTokens.Count > 0;

        /// <summary>현재 잠금 토큰 수.</summary>
        public int LockCount => _lockTokens.Count;

        /// <summary>플래그 활성화 요청. 자기 자신의 MonoBehaviour를 토큰으로 전달한다.</summary>
        public bool Lock(MonoBehaviour token)
        {
            if (token == null) return false;
            var added = _lockTokens.Add(token);
            if (added) OnStateChanged?.Invoke(IsActive);
            return added;
        }

        /// <summary>플래그 비활성화 요청. Lock 시 사용한 동일 토큰을 전달한다.</summary>
        public bool Unlock(MonoBehaviour token)
        {
            if (token == null) return false;
            var removed = _lockTokens.Remove(token);
            if (removed) OnStateChanged?.Invoke(IsActive);
            return removed;
        }

        /// <summary>모든 잠금을 강제 해제한다. 씬 전환 등에서 사용.</summary>
        public void ForceUnlockAll()
        {
            if (_lockTokens.Count == 0) return;
            _lockTokens.Clear();
            OnStateChanged?.Invoke(false);
        }

        /// <summary>상태 변경 콜백. bool 인자는 IsActive 값.</summary>
        public event Action<bool> OnStateChanged;

        private void OnDisable() => _lockTokens.Clear();
    }
}
