# Accelib.UI.Popup

레이어 기반 팝업 및 모달 다이얼로그 시스템.
스택 기반 레이어 관리, 딤 오버레이, 비동기 모달(확인/취소) 패턴을 제공한다.

## 디렉토리 구조

```
Accelib.UI.Popup/
├── Accelib.UI.Popup.asmdef         # 어셈블리 정의
├── README.md                       # 이 문서
├── CHANGELOG.md                    # 변경 이력
└── Runtime/
    ├── PopupSingleton.cs           # 싱글톤 팝업 매니저
    ├── Data/
    │   └── ModalOpenOption.cs      # 모달 설정 데이터
    ├── Layer/
    │   ├── Base/
    │   │   └── LayerPopupBase.cs   # 추상 레이어 팝업 베이스
    │   ├── LayerPopup_Default.cs   # 인스펙터 설정 가능 기본 레이어 팝업
    │   └── LayerPopup_Modal.cs     # 비동기 모달 다이얼로그
    └── Utility/
        └── PopupOpener_Modal.cs    # UnityEvent 기반 모달 열기 헬퍼
```

## 주요 클래스

### PopupSingleton

`MonoSingleton<PopupSingleton>` 기반 팝업 매니저. 캔버스 위에 레이어/모달을 관리한다.

- `OpenLayer(LayerPopupBase prefab, object param)` — 레이어 팝업 열기. 딤 위치 자동 관리
- `OpenModal(ModalOpenOption option)` — 비동기 모달 열기. `UniTask<Result>` 반환
- `CloseLayer(LayerPopupBase target)` — 특정 레이어 닫기
- `CloseLastLayer()` — 최상위 레이어 닫기
- `IsModalActive` — 모달 활성 여부
- `LayerCount` — 열린 레이어 수

**입력 상태 연동:** `SO_InputState` 필드(Optional)를 통해 팝업 열기/닫기 시 자동으로 Lock/Unlock 호출.

### LayerPopupBase

모든 레이어 팝업의 추상 베이스 클래스.

- `AllowMultiInstance` — 중복 인스턴스 허용 여부 (기본 false)
- `HideOnLostFocus` — 포커스 잃을 때 숨김 여부 (기본 true)
- `GetId()` — 팝업 고유 식별자
- 라이프사이클: `OnPreOpen`, `OnPostOpen`, `OnClose`, `OnLostFocus`, `OnRegainFocus`

### LayerPopup_Modal

비동기 확인/취소 모달 다이얼로그. `UniTaskCompletionSource<Result>`로 결과 대기.

- `Open(ModalOpenOption option)` — 모달 열기, `UniTask<Result>` 반환
- `OnClickResult(int result)` — 버튼 onClick에서 호출
- `Result` — `OK(0)`, `NG(1)`, `Exception(-1)`

### ModalOpenOption

모달 설정 데이터 클래스.

- `useLocale` — 로컬라이제이션 키 사용 여부
- `title`, `desc` — 제목 및 설명 텍스트/키
- `descParams` — 설명 포맷 파라미터
- `ok`, `ng` — 확인/취소 버튼 텍스트/키

### PopupOpener_Modal

UnityEvent 기반 모달 헬퍼. 인스펙터에서 `ModalOpenOption` 설정 후 `onOK`/`onNG` 이벤트 바인딩.

## 사용 예시

```csharp
// 비동기 모달 열기
var option = new ModalOpenOption
{
    useLocale = true,
    title = "modal_confirm_title",
    desc = "modal_confirm_desc",
    ok = "common_ok",
    ng = "common_cancel"
};

var result = await PopupSingleton.Instance.OpenModal(option);
if (result == LayerPopup_Modal.Result.OK)
{
    // 확인 처리
}
```

## 의존성

| 패키지 | 용도 |
|--------|------|
| `Accelib.Runtime` | MonoSingleton, Deb 로깅 |
| `Accelib.InputState` | 입력 잠금 연동 (Optional) |
| `UniTask` | 비동기 모달 패턴 |
| `TextMeshPro` | UI 텍스트 |
| `NaughtyAttributes` | 인스펙터 어트리뷰트 |
| `Unity Atoms` | BoolVariable (일시정지 상태) |

## 네임스페이스

```
Accelib.Module.UI.Popup                 — PopupSingleton
Accelib.Module.UI.Popup.Data            — ModalOpenOption
Accelib.Module.UI.Popup.Layer.Base      — LayerPopupBase
Accelib.Module.UI.Popup.Layer           — LayerPopup_Default, LayerPopup_Modal
Accelib.Module.UI.Popup.Utility         — PopupOpener_Modal
```
