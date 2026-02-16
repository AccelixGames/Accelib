# Changelog

이 문서는 Accelib.Pool 모듈의 주요 변경 내역을 기록한다.

## [0.1.0] - 2026-02-16

### 추가
- `IPoolTarget` — 풀 대상 인터페이스 (OnRelease 기본 구현)
- `ResourcePool<T>` — Stack 기반 제네릭 리소스 풀 (비-MonoBehaviour 용)
- `ComponentPool<T>` — List 기반 델리게이트 구동 컴포넌트 풀
- `PrefabPool<T>` — MonoBehaviour 프리팹 전용 풀 (ComponentPool 확장)

### 변경
- ProjectMaid `Accelix.GameSystem.Utility.Pool`에서 이관, 네임스페이스를 `Accelib.Pool`로 변경
- NaughtyAttributes 의존성 제거, Odin Inspector 기반으로 전환
- `PrefabPool.Get()`의 예외 타입을 `NullReferenceException`에서 `InvalidOperationException`으로 변경

### 문서
- 모든 public/protected abstract 메서드 및 델리게이트 필드에 XML `<summary>` 주석 추가
