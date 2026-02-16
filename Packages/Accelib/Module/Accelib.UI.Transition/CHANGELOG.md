# Changelog

이 문서는 Accelib.UI.Transition 모듈의 주요 변경 내역을 기록한다.

## [0.1.0] - 2026-02-16

### 추가
- `Accelib.Runtime`의 `Module.Transition`에서 독립 모듈로 추출
- `TransitionSingleton`에 `SO_TokenFlag showCursor` 필드 추가 (Optional)
  - 트랜지션 시작 시 `showCursor.Lock(this)` 호출
  - 트랜지션 종료 시 `showCursor.Unlock(this)` 호출
- 기존 클래스 모두 이동 (네임스페이스 유지)
  - `TransitionSingleton`, `TransitionEffect` (베이스)
  - `TransitionEffect_Door`, `TransitionEffect_Fade`, `TransitionEffect_Mask`
  - `TransitionEffect_Pop`, `TransitionEffect_Rect`
