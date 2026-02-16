# Accelib 모듈 카탈로그

이 문서는 Accelib 패키지의 모든 모듈과 코어 어셈블리의 개요를 기록한다.
모듈 작업 시 이 파일을 먼저 읽고, 필요한 모듈의 README를 참조한다.

## 모듈 목록

| 모듈 | 요약 | 의존성 | README |
|------|------|--------|--------|
| **Accelib.Core** | 핵심 런타임 (MonoSingleton, Logging) | (없음) | — |
| **Accelib.Extensions** | 확장 메서드 (List, IReadonlyList) | (없음) | — |
| **Accelib.Preview** | 프리뷰 이름/아이콘/서브에셋 인터페이스 정의 | Odin Inspector (조건부) | [README](Accelib.Preview/README.md) |
| **Accelib.Reflection** | 리플렉션 기반 멤버 접근 및 UI 바인딩. SO의 중첩 필드 경로를 드롭다운으로 선택, 캐시된 리플렉션으로 런타임 읽기 | Accelib.Preview | [README](Accelib.Reflection/README.md) |
| **Accelib.Conditional** | 조건식 평가 시스템. 비교/논리 연산자로 규칙 기반 로직 구성. 인스펙터에서 조건 편집 및 텍스트 프리뷰 | Accelib.Reflection, Accelib.Preview, Odin Inspector, ZLinq, Collections | [README](Accelib.Conditional/README.md) |
| **Accelib.OdinExtension** | R3 ReactiveProperty용 Odin Drawer. SerializableReactiveProperty 순수 값 편집 | R3, Odin Inspector | [README](Accelib.OdinExtension/README.md) |
| **Accelib.R3Extension** | R3 Observable 확장 메서드. Delta() 등 자주 쓰는 연산자 조합 제공 | R3 | [README](Accelib.R3Extension/README.md) |
| **Accelib.Pool** | 오브젝트 풀링 (리소스/컴포넌트/프리팹) | Odin Inspector | [README](Accelib.Pool/README.md) |
| **Accelib.Flag** | 토큰 기반 플래그 관리. MonoBehaviour를 토큰으로 사용하여 플래그 활성화/비활성화 | Odin Inspector (디버깅) | [README](Accelib.Flag/README.md) |
| **Accelib.UI.Popup** | 레이어 팝업 및 모달 다이얼로그. 스택 기반 레이어 관리, 비동기 모달 패턴 | Accelib.Core, Accelib.Flag, Accelib.Localization, UniTask, Unity Atoms | [README](Accelib.UI.Popup/README.md) |
| **Accelib.UI.Transition** | 화면 전환 이펙트 (페이드, 마스크, 도어 등). DOTween 기반 트랜지션 | Accelib.Runtime, Accelib.Flag, DOTween, Odin | [README](Accelib.UI.Transition/README.md) |
| **Accelib.Localization** | 로컬라이제이션 시스템. 다국어 텍스트, 언어별 폰트 교체, Google Sheets 다운로드 | Accelib.Runtime, TMP, SerializedCollections, Odin | [README](Accelib.Localization/README.md) |

## 모듈 상세

### Accelib.Core
- **경로:** `Accelib.Core/`
- **주요 클래스:** `MonoSingleton<T>` (싱글톤 베이스), `MonoSingletonSerialized<T>` (직렬화 싱글톤), `MonoSingletonStatic<T>` (정적 싱글톤), `Deb` (로깅 유틸리티)
- 외부 의존성 없는 핵심 모듈. 다른 모듈들이 공통으로 참조함

### Accelib.Extensions
- **경로:** `Accelib.Extenstions/`
- **주요 클래스:** `ListExtension` (List 확장), `IReadonlyListExtenstion` (IReadOnlyList 확장)
- 외부 의존성 없는 독립 모듈

### Accelib.Preview
- **경로:** `Accelib.Preview/`
- **인터페이스:** `IPreviewNameProvider` (이름), `IPreviewIconProvider` (Odin SdfIcon, `#if ODIN_INSPECTOR`), `ISubAssetProvider` (서브에셋 목록)
- Odin 조건부 의존 (`IPreviewIconProvider`만 Odin 필요). 다른 모듈들이 공통으로 참조함

### Accelib.Reflection
- **경로:** `Accelib.Reflection/`
- **주요 클래스:** `MemberRef` (직렬화 멤버 참조), `CachedChain` (리플렉션 캐시), `CachedReflectionUtility` (런타임 유틸리티), `ReflectionUtility` (에디터 스캔)
- R3 ReactiveProperty 타입 자동 감지 (어셈블리 참조 없이 타입명 기반)

### Accelib.Conditional
- **경로:** `Accelib.Conditional/`
- **주요 클래스:** `Conditional` (조건 컨테이너, Evaluate()), `Condition` (좌/우 ValueProvider + 비교연산자), `ValueProvider` (리터럴/SO/MemberRef 값 소스), `SO_Conditional` (ScriptableObject 래퍼), `SO_ValueProviderBase` (값 제공자 추상 베이스), `SO_PresetValue` (MemberRef 프리셋)
- **연산자:** `EComparisonOperator` (==, !=, >, >=, <, <=), `ELogicalOperator` (And, Or)
- **값 소스:** Integer/Double/Boolean 리터럴, ScriptableObject, Custom (MemberRef)
- Odin Inspector 필수 의존 (`defineConstraints: ODIN_INSPECTOR`)

### Accelib.OdinExtension
- **경로:** `Accelib.OdinExtension/`
- **드로어:** `SerializableReactiveProperty<int/bool/float>` (편집)
- **유틸리티:** `ReactivePropertyDrawerHelper` (ForceNotify 리플렉션 캐시)

### Accelib.R3Extension
- **경로:** `Accelib.R3Extension/`
- **주요 클래스:** `ObservableExtension` (Delta 등 Observable 확장 메서드)
- Runtime 전용 모듈. Odin 의존성 없음

### Accelib.Pool
- **경로:** `Accelib.Pool/`
- **주요 클래스:** `IPoolTarget` (풀 대상 인터페이스), `ResourcePool<T>` (Stack 기반 리소스 풀), `ComponentPool<T>` (델리게이트 구동 컴포넌트 풀), `PrefabPool<T>` (프리팹 전용 풀)
- Odin Inspector 필수 의존 (`defineConstraints: ODIN_INSPECTOR`)

### Accelib.Flag
- **경로:** `Accelib.Flag/`
- **주요 클래스:** `SO_TokenFlag` (MonoBehaviour 토큰 기반 플래그 활성화/비활성화 ScriptableObject)
- Odin Inspector 디버깅용 의존 (`[ShowInInspector, ReadOnly]`)

### Accelib.UI.Popup
- **경로:** `Accelib.UI.Popup/`
- **주요 클래스:** `PopupSingleton` (싱글톤 팝업 매니저), `LayerPopupBase` (추상 베이스), `LayerPopup_Default` (기본 레이어), `LayerPopup_Modal` (비동기 모달 추상 베이스), `LayerPopup_PlainModal` (일반 텍스트 모달), `LayerPopup_LocalizedModal` (로컬라이제이션 모달), `IModalOptionProvider` (모달 옵션 인터페이스), `SO_ModalOpenOption` (일반 텍스트 모달 옵션 SO), `SO_ModalOpenOptionLocalized` (로컬라이제이션 모달 옵션 SO), `PopupOpener_Modal` (UnityEvent 헬퍼)
- `Accelib.Runtime`의 `Module.UI.Popup`에서 독립 모듈로 추출
- `SO_TokenFlag showCursor` 연동으로 팝업 열기/닫기 시 커서 표시 자동 처리

### Accelib.UI.Transition
- **경로:** `Accelib.UI.Transition/`
- **주요 클래스:** `TransitionSingleton` (싱글톤 트랜지션 매니저), `TransitionEffect` (추상 이펙트 베이스), `TransitionEffect_Fade/Mask/Door/Pop/Rect` (이펙트 구현)
- `Accelib.Runtime`의 `Module.Transition`에서 독립 모듈로 추출
- `SO_TokenFlag showCursor` 연동으로 트랜지션 시작/종료 시 커서 표시 자동 처리

### Accelib.Localization
- **경로:** `Accelib.Localization/`
- **주요 클래스:** `LocalizationSingleton` (싱글톤 매니저), `LocaleSO` (로케일 SO), `LocaleFontData` (폰트 데이터), `ILocaleChangedEventListener` (변경 리스너), `LocaleKey` (키 구조체), `LocalizedTMP` (TMP 컴포넌트), `LocalizedFont` (폰트 전용), `LocalizedImage` (이미지 교체), `LocalizedEvent` (이벤트 전달)
- `Accelib.Runtime`의 `Module.Localization`에서 독립 모듈로 추출
- Editor 코드 별도 asmdef(`Accelib.Localization.Editor`)로 분리

## 의존성 그래프

```
Accelib.Core (의존성 없음, 핵심)
Accelib.Extensions (의존성 없음, 독립)

Accelib.Preview (Odin 조건부 의존)
    ↑
    ├── Accelib.Reflection
    │       ↑
    │       └── Accelib.Conditional (Odin, ZLinq 외부 의존)
    │
    └── Accelib.OdinExtension (R3, Odin 외부 의존)

Accelib.R3Extension (R3 외부 의존, 독립)

Accelib.Pool (Odin 외부 의존, 독립)

Accelib.Flag (Odin 디버깅용 의존)
    ↑
    ├── Accelib.UI.Popup (Accelib.Core, Accelib.Localization, UniTask 외부 의존)
    └── Accelib.UI.Transition (Accelib.Runtime, DOTween, Odin 외부 의존)

Accelib.Localization (Accelib.Runtime, TMP, SerializedCollections, Odin 외부 의존)
    ↑
    └── Accelib.Localization.Editor (Editor 전용, UniTask 외부 의존)
```

## 코어 어셈블리

| 어셈블리 | 경로 | 요약 |
|----------|------|------|
| **Accelib.Runtime** | `../Runtime/` | 메인 런타임. Collections, Core, Data, Effect, Extensions, UI 등 서브시스템 포함 |
| **Accelib.Editor** | `../Editor/` | 에디터 도구. AutoBuild, CustomWindow, 각종 Drawer/Editor 포함 |

---

*이 파일은 모듈 생성/수정/삭제 시 반드시 업데이트한다. 상세 규칙은 [MODULE_RULES.md](MODULE_RULES.md) 참조.*
