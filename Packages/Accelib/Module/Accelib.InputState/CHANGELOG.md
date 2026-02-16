# Changelog

이 문서는 Accelib.InputState 모듈의 주요 변경 내역을 기록한다.

## [0.1.0] - 2026-02-16

### 추가
- `SO_InputState` ScriptableObject 신규 생성
  - `GameObject` 토큰 기반 입력 잠금/해제
  - `HashSet`으로 중복 Lock 방지
  - `OnStateChanged` 이벤트로 외부 시스템 구독 지원
  - `ForceUnlockAll()`로 씬 전환 시 안전 초기화
