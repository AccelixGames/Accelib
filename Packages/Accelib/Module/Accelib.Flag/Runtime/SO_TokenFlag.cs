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
        [TitleGroup("상태", indent:true)]
        [ShowInInspector, ReadOnly] public bool IsActive => _lockTokens.Count > 0;
        [ShowInInspector, ReadOnly] public int LockCount => _lockTokens.Count;
        
        [TitleGroup("디버그", indent:true)]
        [ShowInInspector, ReadOnly] private HashSet<MonoBehaviour> _lockTokens = new();

        /// <summary>상태 변경 콜백. bool 인자는 IsActive 값.</summary>
        public event Action<bool> OnStateChanged;
        
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

        private void OnDisable() => _lockTokens.Clear();

        private void Reset()
        {
            _lockTokens ??= new  HashSet<MonoBehaviour>();
            _lockTokens.Clear();
        }
    }
}
