# Changelog

이 문서는 Accelib 패키지의 주요 변경 내역을 기록한다.

형식은 [Keep a Changelog](https://keepachangelog.com/en/1.1.0/)를 기반으로 하며,
[Semantic Versioning](https://semver.org/spec/v2.0.0.html)을 따른다.

## [0.1.7] - 2026-02-23

### 수정
- `Accelib.UI.Transition` — `TransitionSingleton` 카메라 활성화 타이밍 수정 (콜백 내부로 이동, 종료 시 비활성화 추가)
- `MonoBehaviourExtension.FindComponents<T>()` — static `_visitedBuffer`를 로컬 변수로 변경 (호출 간 상태 누출 방지)

## [0.1.6] - 2026-02-20

### 추가
- `Accelib.Conditional` — Contains 비교 연산자 추가 (`EComparisonOperator.Contains`, 인스펙터 표기: "Contains To")
- `Accelib.Conditional` — `EValueSourceType.String` 소스 타입 추가 (문자열 리터럴 입력)
- `Accelib.Conditional` — `ValueProvider.StringValue` / `GetRawValue()` 추가
- `Accelib.Conditional` — `Condition.EvaluateContains()` 추가 (ICollection<string>/IEnumerable<string> 멤버십 검사)
- `Accelib.Reflection` — `MemberRef.RawValue` 프로퍼티 추가 (double 변환 없이 원본 객체 반환)
- `Accelib.Reflection` — `ReflectionUtility`에 문자열 컬렉션(IEnumerable<string>) 필드/프로퍼티 드롭다운 지원 추가
- `Accelib.Reflection` — `ReflectionUtility`에 implicit 숫자 변환 연산자(op_Implicit → float/double/int/long) 보유 타입 드롭다운 지원 추가
- `Accelib.Reflection` — `CachedReflectionUtility.ConvertToDoubleFast`에 implicit 숫자 변환 fallback 추가 (PriceUnit 등 지원)

### 수정
- `Accelib.Reflection` — `ReflectionUtility.ShouldRecurse`에서 Delegate 타입 재귀 탐색 차단 (드롭다운 폭발 방지)

## [0.1.5] - 2026-02-19

### 추가
- `Accelib.Reflection` — `MemberRef.Subscribe`에 ReactiveProperty 자동 감지 구독 추가 (R3 참조 없이 리플렉션으로 동작)
- `Accelib.Reflection` — `CachedChain.ReactivePropertyChainLength` 필드 추가

## [0.1.4] - 2026-02-19

### 수정
- `Accelib.Reflection` — `CachedReflectionUtility.BuildChain` ReactiveProperty `.Value` 경로 리졸브 실패 수정 (`.Value` → `CurrentValue` 자동 치환)
- `Accelib.Reflection` — `ReflectionUtility` 드롭다운 경로 `.Value` → `.CurrentValue`로 통일

## [0.1.3] - 2026-02-19

### 추가
- `Accelib.Reflection` — `INotifyValueChanged` 인터페이스 신규 생성 (값 변경 알림용)
- `Accelib.Reflection` — `MemberRef.Subscribe()` 메서드 추가 (target이 `INotifyValueChanged` 구현 시 값 변경 구독)
- `Accelib.Conditional` — `SO_ValueProviderBase.Subscribe()` 가상 메서드 추가
- `Accelib.Conditional` — `ValueProvider.Subscribe()` 메서드 추가 (SO/Custom 소스 구독)
- `Accelib.Conditional` — `Condition.SubscribeLhs()`/`SubscribeRhs()` 메서드 추가
- `Accelib.Conditional` — `Conditional.Subscribe()` 메서드 추가 (전체 조건식 값 변경 일괄 구독)

## [0.1.2] - 2026-02-19

### 변경
- `Accelib.Pool` — `PrefabPool<T>` 인스펙터 레이아웃 개선 (`parent`/`prefab` 최상위 노출, 디버깅 그룹 분리)
- `Accelib.Pool` — `ComponentPool<T>._releasedList`를 `디버깅` TitleGroup + `ReadOnly`로 변경
- `Module.Reference` — `TransformRefFinder`에서 `FindProvider()`를 별도 메서드로 분리, LateUpdate 시 참조 소실 시 자동 재탐색
- `LevelDesignEditorWindow` — `EditorPreviewName`이 null/빈 문자열일 때 기본 에셋 이름 유지
- `Accelib.UI.Popup` — `SO_ModalOpenOptionLocalized.OpenAsync()`에 `[Button]` 추가
- `Accelib.Localization` — `LocaleKey.GetLocalized()` 편의 메서드 추가

### 수정
- `Accelib.Pool` — `PrefabPool<T>.Initialize()`에서 `prefab`/`parent` null 방어 처리 추가
- `ScriptableObjectCached` — `System` using 누락 수정

## [0.1.1] - 2026-02-16

### 추가
- `Accelib.Flag` — `SO_TokenFlagDrawer` Odin 커스텀 드로어 추가 (IsActive bool 토글 + ObjectField 한 줄 표시)
- `Accelib.Flag` — `Accelib.Flag.Editor` asmdef 추가 (Editor 전용)

### 변경
- `Accelib.Flag` — `SO_TokenFlag` 인스펙터 레이아웃 개선 (TitleGroup으로 상태/디버그 그룹 분리)

### 수정
- `MonoBehaviourExtension.FindComponents<T>()` — 동일 GameObject에 T를 구현하는 컴포넌트가 여러 개 있을 때 첫 번째만 수집되던 버그 수정 (`TryGetComponent` → `GetComponents`)

## [0.1.0] - 2026-02-16

### 추가
- `Accelib.Core` 모듈 신규 생성 — 핵심 런타임 (MonoSingleton, Logging)
  - `Accelib.Runtime`의 `Core/MonoSingleton*`을 독립 모듈로 추출
  - `Accelib.Runtime`의 `Logging/Deb`을 독립 모듈로 이동
- `Accelib.Extensions` 모듈 신규 생성 — 확장 메서드 (List, IReadonlyList)
  - `Accelib.Runtime`의 `Extensions/`를 독립 모듈로 추출
- `Accelib.Pool` 모듈 신규 생성 — 오브젝트 풀링 시스템
  - `IPoolTarget`, `ResourcePool<T>`, `ComponentPool<T>`, `PrefabPool<T>`
- `Accelib.Flag` 모듈 신규 생성 — 토큰 기반 플래그 관리
  - `SO_TokenFlag`: MonoBehaviour 토큰으로 플래그 활성화/비활성화
- `Accelib.UI.Popup` 모듈 신규 생성 — 레이어 팝업 및 모달 시스템
  - `Accelib.Runtime`의 `Module.UI.Popup`에서 독립 모듈로 추출
  - `LayerPopup_Modal`을 abstract 베이스 클래스로 변환 (UI 필드 서브클래스 위임)
  - `LayerPopup_PlainModal` (일반 텍스트 모달, `TMP_Text` 직접 소유) 추가
  - `LayerPopup_LocalizedModal` (로컬라이제이션 모달, `LocalizedTMP` 직접 소유) 추가
  - `SO_TokenFlag showCursor` 연동으로 팝업 시 커서 표시 자동 처리
- `Accelib.UI.Transition` 모듈 신규 생성 — 화면 전환 이펙트 시스템
  - `Accelib.Runtime`의 `Module.Transition`에서 독립 모듈로 추출
  - `SO_TokenFlag showCursor` 연동 추가
- `Accelib.Localization` 모듈 신규 생성 — 로컬라이제이션 시스템
  - `Accelib.Runtime`의 `Module.Localization`에서 독립 모듈로 추출
  - Editor 코드 별도 asmdef(`Accelib.Localization.Editor`)로 분리
  - `LocaleKeyDrawer`, `LocaleUtility` Runtime → Editor 이동
  - `DownloadLocaleWindow`, `EditorObjectField`, `EditorPrefsField` Accelib.Editor → 모듈 내 Editor 이동

### 변경
- `Accelib.Preview` 모듈 MODULE_RULES 표준 준수 리팩토링
  - 폴더 구조 UPM 표준 재구성, asmdef 표준화, `#if ODIN_INSPECTOR` 전처리기 래핑
- `Accelib.Conditional` 모듈 MODULE_RULES 표준 준수 개선
  - 폴더 구조·네임스페이스 표준화, XML 주석 추가
- `PopupSingleton`의 `isPaused`/`SO_InputState` → `SO_TokenFlag showCursor`로 변경
- `ModalOpenOption`에서 `useLocale` 필드 제거
- `Accelib.UI.Popup` — `ModalOpenOption`/`ModalOpenOptionBase` → `IModalOptionProvider` 인터페이스로 전환
- `Accelib.UI.Popup` — 네임스페이스 `Accelib.Module.UI.Popup.*` → `Accelib.UI.Popup.Runtime.*` 변경
- `Accelib.UI.Popup` — `SO_ModalOpenOption`, `SO_ModalOpenOptionLocalized` SO 추가
- `Accelib.Flag` — `SO_TokenFlag._lockTokens`에 `[ShowInInspector, ReadOnly]` 추가 (디버깅용)
- `LocalizedTypewriter` 주석 처리 (Febucci TextAnimator 의존성 제거)
- `EditorObjectField` 네임스페이스 통일
- `Accelib.Runtime` asmdef에 신규 모듈 참조 추가

### 수정
- `Accelib.Localization` — `LocaleSO.TryGetValue()` null 초기화 버그 수정
- `Accelib.Localization` — `LocaleFontData.GetMaterial()` null 안전성 추가

### 제거
- 레거시 `Module.ObjectPool` 삭제 — `Accelib.Pool` 모듈로 대체
- `Accelib.InputState` 모듈 삭제 — `Accelib.Flag` 모듈로 대체 (이름·API 전면 변경)
- `Accelib.Runtime`의 `Module.Localization` 삭제 — `Accelib.Localization` 모듈로 대체
- `Accelib.Runtime`의 `Module.Transition` 삭제 — `Accelib.UI.Transition` 모듈로 대체
- `Accelib.Runtime`의 `Module.UI.Popup` 삭제 — `Accelib.UI.Popup` 모듈로 대체
- `Accelib.Runtime`의 `Core/MonoSingleton*` 삭제 — `Accelib.Core` 모듈로 대체
- `Accelib.Runtime`의 `Extensions/` 삭제 — `Accelib.Extensions` 모듈로 대체
- `Accelib.Runtime`의 `Logging/SimpleLog.cs` 삭제
- `Accelib.Editor`의 `Module.Localization` 삭제 — `Accelib.Localization.Editor`로 대체
- `Test_TMPFontAsset.cs` 삭제

## [0.0.13] - 2025-02-13

### 추가
- `Accelib.R3Extension` 모듈 신규 생성 — R3 Observable 확장 메서드
  - `ObservableExtension.Delta()`: `Observable<int>`의 연속 값 차이(델타) 발행

### 변경
- `CHANGELOG.md` Keep a Changelog 형식으로 정비
- `README.md` 패키지 소개 및 모듈 목록으로 재작성
- `MODULE_CATALOG.md` R3Extension 모듈 추가
- `MODULE_RULES.md` 패키지 루트 문서 업데이트 규칙 및 버전 관리 규칙 추가 (§7, §8)

## [0.0.3] - 2024-04-16

### 추가
- CHANGELOG.md 추가

### 변경
- README.md 파일 내용 수정

## [0.0.2] - 2024-04-16

### 추가
- A lot of tools Updated.

## [0.0.1] - 2024-02-29

### 추가
- Initial commit
