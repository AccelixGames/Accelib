# Changelog

이 문서는 Accelib.UI.Popup 모듈의 주요 변경 내역을 기록한다.

## [0.1.0] - 2026-02-16

### 추가
- `Accelib.Runtime`의 `Module.UI.Popup`에서 독립 모듈로 추출
- `PopupSingleton`에 `SO_InputState` 필드 추가 (Optional)
  - 팝업 열기 시 `inputState.Lock(gameObject)` 호출
  - 팝업 닫기 시 `inputState.Unlock(gameObject)` 호출
- 기존 클래스 모두 이동 (네임스페이스 유지)
  - `PopupSingleton`, `LayerPopupBase`, `LayerPopup_Default`
  - `LayerPopup_Modal`, `ModalOpenOption`, `PopupOpener_Modal`
