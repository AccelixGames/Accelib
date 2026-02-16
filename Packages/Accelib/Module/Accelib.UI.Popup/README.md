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
    │   ├── IModalOptionProvider.cs        # 모달 옵션 인터페이스
    │   ├── SO_ModalOpenOption.cs          # 일반 텍스트 모달 옵션 SO
    │   └── SO_ModalOpenOptionLocalized.cs # 로컬라이제이션 모달 옵션 SO
    ├── Layer/
    │   ├── Base/
    │   │   ├── LayerPopupBase.cs   # 추상 레이어 팝업 베이스
    │   │   └── LayerPopup_Modal.cs # 비동기 모달 추상 베이스
    │   ├── LayerPopup_Default.cs   # 인스펙터 설정 가능 기본 레이어 팝업
    │   ├── LayerPopup_PlainModal.cs      # 일반 텍스트 모달
    │   └── LayerPopup_LocalizedModal.cs  # 로컬라이제이션 모달
    └── Utility/
        └── PopupOpener_Modal.cs    # UnityEvent 기반 모달 열기 헬퍼
```

## 주요 클래스

### PopupSingleton

`MonoSingleton<PopupSingleton>` 기반 팝업 매니저. 캔버스 위에 레이어/모달을 관리한다.

- `OpenLayer(LayerPopupBase prefab, object param)` — 레이어 팝업 열기. 딤 위치 자동 관리
- `OpenModal(IModalOptionProvider option)` — 비동기 모달 열기. `UniTask<Result>` 반환
- `CloseLayer(LayerPopupBase target)` — 특정 레이어 닫기
- `CloseLastLayer()` — 최상위 레이어 닫기
- `IsModalActive` — 모달 활성 여부
- `LayerCount` — 열린 레이어 수

**커서 플래그 연동:** `SO_TokenFlag showCursor` 필드(Optional)를 통해 팝업 열기/닫기 시 자동으로 Lock/Unlock 호출.

### LayerPopupBase

모든 레이어 팝업의 추상 베이스 클래스.

- `AllowMultiInstance` — 중복 인스턴스 허용 여부 (기본 false)
- `HideOnLostFocus` — 포커스 잃을 때 숨김 여부 (기본 true)
- `GetId()` — 팝업 고유 식별자
- 라이프사이클: `OnPreOpen`, `OnPostOpen`, `OnClose`, `OnLostFocus`, `OnRegainFocus`

### LayerPopup_Modal (abstract)

비동기 모달 다이얼로그의 추상 베이스 클래스. `UniTaskCompletionSource<Result>`로 결과 대기.
UI 필드는 베이스에 없으며, 각 서브클래스가 자체 필드를 소유한다.

- `Open(IModalOptionProvider option)` — 모달 열기, `UniTask<Result>` 반환
- `ApplyOption(IModalOptionProvider)` — **abstract**, 서브클래스에서 텍스트 설정 방식 결정
- `OnClickResult(int result)` — 버튼 onClick에서 호출
- `Result` — `OK(0)`, `NG(1)`, `Exception(-1)`

### LayerPopup_PlainModal

일반 텍스트 모달. `TMP_Text` SerializeField를 직접 소유하고 `text`에 직접 할당한다.

### LayerPopup_LocalizedModal

로컬라이제이션 키 기반 모달. `LocalizedTMP` SerializeField를 직접 소유하고 `ChangeKey()`를 통해 텍스트를 설정한다.

### IModalOptionProvider

모달 옵션 공통 인터페이스. `Title`, `Desc`, `DescParams`, `Ok`, `Ng` 프로퍼티를 정의한다.

### SO_ModalOpenOption

일반 텍스트 기반 모달 옵션 ScriptableObject. `IModalOptionProvider` 구현.

### SO_ModalOpenOptionLocalized

로컬라이제이션 키 기반 모달 옵션 ScriptableObject. `LocaleKey`를 사용하며 `IModalOptionProvider` 구현.

### PopupOpener_Modal

UnityEvent 기반 모달 헬퍼. 인스펙터에서 `SO_ModalOpenOption` 설정 후 `onOK`/`onNG` 이벤트 바인딩.

## 사용 예시

```csharp
// SO 에셋을 인스펙터에서 연결
[SerializeField] private SO_ModalOpenOption confirmOption;

// 모달 열기
var result = await PopupSingleton.Instance.OpenModal(confirmOption);
if (result == LayerPopup_Modal.Result.OK)
{
    // 확인 처리
}

// 로컬라이제이션 모달 (SO_ModalOpenOptionLocalized 사용)
[SerializeField] private SO_ModalOpenOptionLocalized localizedOption;

localizedOption.SetParams("아이템 이름");
var result = await PopupSingleton.Instance.OpenModal(localizedOption);
```

## 의존성

| 패키지 | 용도 |
|--------|------|
| `Accelib.Core` | MonoSingleton, Deb 로깅 |
| `Accelib.Flag` | 커서 플래그 연동 (Optional) |
| `Accelib.Localization` | 로컬라이제이션 모달 |
| `UniTask` | 비동기 모달 패턴 |
| `TextMeshPro` | UI 텍스트 |
| `Odin Inspector` | 인스펙터 어트리뷰트 |
| `Unity Atoms` | BoolVariable (일시정지 상태) |

## 네임스페이스

```
Accelib.UI.Popup.Runtime               — PopupSingleton
Accelib.UI.Popup.Runtime.Data          — IModalOptionProvider, SO_ModalOpenOption, SO_ModalOpenOptionLocalized
Accelib.UI.Popup.Runtime.Layer.Base    — LayerPopupBase, LayerPopup_Modal
Accelib.UI.Popup.Runtime.Layer         — LayerPopup_Default, LayerPopup_PlainModal, LayerPopup_LocalizedModal
Accelib.UI.Popup.Runtime.Utility       — PopupOpener_Modal
```
