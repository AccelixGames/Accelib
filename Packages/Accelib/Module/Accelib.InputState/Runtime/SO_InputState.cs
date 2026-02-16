using System;
using System.Collections.Generic;
using UnityEngine;

namespace Accelib.InputState
{
    /// <summary>
    /// 토큰 기반 입력 상태 관리 ScriptableObject.
    /// 여러 시스템이 GameObject를 토큰으로 사용하여 입력 잠금을 요청/해제할 수 있다.
    /// </summary>
    /// <remarks>TODO: 프로젝트 분석에 맞춰 기능 업그레이드 필요</remarks>
    [CreateAssetMenu(menuName = "Accelib/InputState")]
    public class SO_InputState : ScriptableObject
    {
        private readonly HashSet<GameObject> _lockTokens = new();

        /// <summary>잠금 상태인지 여부</summary>
        public bool IsLocked => _lockTokens.Count > 0;

        /// <summary>현재 잠금 토큰 수</summary>
        public int LockCount => _lockTokens.Count;

        /// <summary>잠금 요청. 자기 자신의 GameObject를 토큰으로 전달한다.</summary>
        public bool Lock(GameObject token)
        {
            if (token == null) return false;
            var added = _lockTokens.Add(token);
            if (added) OnStateChanged?.Invoke(IsLocked);
            return added;
        }

        /// <summary>잠금 해제. Lock 시 사용한 동일 토큰을 전달한다.</summary>
        public bool Unlock(GameObject token)
        {
            if (token == null) return false;
            var removed = _lockTokens.Remove(token);
            if (removed) OnStateChanged?.Invoke(IsLocked);
            return removed;
        }

        /// <summary>모든 잠금을 강제 해제한다. 씬 전환 등에서 사용.</summary>
        public void ForceUnlockAll()
        {
            if (_lockTokens.Count == 0) return;
            _lockTokens.Clear();
            OnStateChanged?.Invoke(false);
        }

        /// <summary>상태 변경 콜백. bool 인자는 IsLocked 값.</summary>
        public event Action<bool> OnStateChanged;

        private void OnDisable() => _lockTokens.Clear();
    }
}
