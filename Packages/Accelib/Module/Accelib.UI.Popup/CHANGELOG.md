# Changelog

이 문서는 Accelib.UI.Popup 모듈의 주요 변경 내역을 기록한다.

## [0.3.0] - 2026-02-16

### 변경
- `LayerPopup_Modal`을 `abstract` 베이스 클래스로 변환
  - `SetText`를 `protected abstract` 메서드로 분리
  - SerializeField 접근 제한자 `private` → `protected` 변경
- `ModalOpenOption`에서 `useLocale` 필드 제거

### 추가
- `LayerPopup_PlainModal` — 일반 텍스트 모달 다이얼로그
- `LayerPopup_LocalizedModal` — 로컬라이제이션 키 기반 모달 다이얼로그
- asmdef 참조: `Accelib.Localization` 추가

## [0.2.0] - 2026-02-16

### 변경
- `PopupSingleton`의 `SO_InputState inputLock` → `SO_TokenFlag showCursor`로 변경
  - `Accelib.Flag` 모듈 이름 변경에 따른 참조 업데이트
  - 토큰 타입 `gameObject` → `this` (MonoBehaviour) 변경
- asmdef 참조: `Accelib.InputState` → `Accelib.Flag`

## [0.1.0] - 2026-02-16

### 추가
- `Accelib.Runtime`의 `Module.UI.Popup`에서 독립 모듈로 추출
- `PopupSingleton`에 `SO_InputState` 필드 추가 (Optional)
  - 팝업 열기 시 `inputState.Lock(gameObject)` 호출
  - 팝업 닫기 시 `inputState.Unlock(gameObject)` 호출
- 기존 클래스 모두 이동 (네임스페이스 유지)
  - `PopupSingleton`, `LayerPopupBase`, `LayerPopup_Default`
  - `LayerPopup_Modal`, `ModalOpenOption`, `PopupOpener_Modal`
