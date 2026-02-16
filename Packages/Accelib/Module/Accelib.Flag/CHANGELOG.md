# Changelog

이 문서는 Accelib.Flag 모듈의 주요 변경 내역을 기록한다.

## [0.2.0] - 2026-02-16

### 추가
- `SO_TokenFlagDrawer` Odin 커스텀 드로어 추가 — IsActive 상태를 bool 토글로 표시하고 옆에 ObjectField로 asset 연결
- `Accelib.Flag.Editor` asmdef 추가 (Editor 전용)

### 변경
- 모듈 이름 변경: `Accelib.InputState` → `Accelib.Flag`
- 클래스 이름 변경: `SO_InputState` → `SO_TokenFlag`
- 토큰 타입 변경: `GameObject` → `MonoBehaviour`
- 프로퍼티 이름 변경: `IsLocked` → `IsActive`
- 네임스페이스 변경: `Accelib.InputState` → `Accelib.Flag`
- `_lockTokens`에 `[ShowInInspector, ReadOnly]` 추가 (디버깅용 인스펙터 표시)
- Odin Inspector 의존성: 제거 → 디버깅용으로 재추가

## [0.1.0] - 2026-02-16

### 추가
- `SO_InputState` ScriptableObject 신규 생성
  - `GameObject` 토큰 기반 입력 잠금/해제
  - `HashSet`으로 중복 Lock 방지
  - `OnStateChanged` 이벤트로 외부 시스템 구독 지원
  - `ForceUnlockAll()`로 씬 전환 시 안전 초기화
