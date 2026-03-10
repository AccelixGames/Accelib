# Changelog

이 문서는 Accelib 패키지의 주요 변경 내역을 기록한다.

형식은 [Keep a Changelog](https://keepachangelog.com/en/1.1.0/)를 기반으로 하며,
[Semantic Versioning](https://semver.org/spec/v2.0.0.html)을 따른다.

## [0.1.30] - 2026-03-11

### 수정
- `RigidbodyUtility.Toggle(false)` — 이미 비활성화된 상태에서 재호출 시 null Rigidbody 접근으로 NRE 발생하던 문제 수정

## [0.1.29] - 2026-03-10

### 수정
- `Accelib.Editor.AutoBuild` — Addressables CleanBuild 시 이전 Remote 폴더 미정리로 과거 번들 누적되던 문제 수정
- `Accelib.Editor.AutoBuild` — Remote 복사 시 대상 폴더 미정리로 이전 빌드 파일 잔류하던 문제 수정
- `Accelib.Editor.AutoBuild` — 인스펙터 경로 열기 버튼 클릭 불가 수정 (`[ReadOnly]` → `[EnableGUI]`)

## [0.1.28] - 2026-03-10

### 수정
- `Accelib.Editor.AutoBuild` — Addressables 빌드 실패 시 에러 사유가 Discord에 출력되지 않던 문제 수정
- `Accelib.Editor.AutoBuild` — 빌드 결과 null 감지 누락 수정

## [0.1.27] - 2026-03-09

### 수정
- `Accelib.DebugServer` — `DebugServerGUI` IMGUI 렌더링 안정성 수정 (배경 텍스처 소실, 폰트 색상 변동)

## [0.1.26] - 2026-03-09

### 추가
- `Accelib.DebugServer` — **SSE (Server-Sent Events) 실시간 이벤트 스트림 지원**
  - `DebugEvent`, `DebugEventBus`, `SseClient` — 스레드 안전 이벤트 발행/링 버퍼/SSE 전송
  - `DebugServerCore.EventBus` — 이벤트 버스 public 접근자
  - `GET /api/events/stream` — SSE 스트림 (`?filter=` 필터링)
  - `GET /api/events/recent` — 링 버퍼 조회 (폴링 폴백)
  - `GET /api/events/clients` — SSE 클라이언트 수 조회
  - 다중 연결 경고, 15초 하트비트, 자동 끊김 감지

## [0.1.25] - 2026-03-09

### 변경
- `Accelib.DebugServer` — `IDebugEndpointProvider`에 `RoutePrefix`, `CategoryName` 프로퍼티 추가 (Provider 레벨에서 라우트 prefix·카테고리 정의)
- `Accelib.DebugServer` — `DebugServerCore.RegisterEndpointsFrom()` 수정: Provider의 `RoutePrefix`를 라우트에 자동 결합, `CategoryName`을 기본 카테고리로 사용

### 추가
- `Accelib.DebugServer` — `DebugEndpointProviderBase` 추상 기반 클래스: Odin 인스펙터에 `RoutePrefix`/`CategoryName` 자동 표시

## [0.1.24] - 2026-03-09

### 수정
- `Accelib.DebugServer` — `DebugServerGUI` 우측 하단 앵커 수정 (`GUI.matrix` 제거, 직접 픽셀 계산 방식)

## [0.1.23] - 2026-03-09

### 변경
- `Accelib.DebugServer` — `Runtime/` 디렉토리를 서브폴더로 정리: `Core/`, `Endpoint/`, `Http/` (네임스페이스 변경 없음)

## [0.1.22] - 2026-03-09

### 추가
- `Accelib.DebugServer` — `DebugServerGUI`: IMGUI 상태 오버레이 (인스펙터에서 직접 추가, scale/alpha 자체 설정)
- `Accelib.DebugServer` — `DebugServerCore`에 `IsRunning`, `Port`, `RegisteredEndpointCount` public 프로퍼티 추가

### 제거
- `Accelib.DebugServer` — `DebugServerCore`의 `showOverlay`, `overlayScale`, `overlayAlpha` 필드 제거 (오버레이 설정은 `DebugServerGUI` 자체 필드로 이동)

## [0.1.21] - 2026-03-09

### 추가
- `Accelib.DebugServer` — 신규 모듈. 컴포지션 기반 디버그 HTTP 서버 프레임워크
- `Accelib.DebugServer` — `DebugServerCore`: HttpListener 기반 sealed 싱글톤. 하위 `IDebugEndpointProvider` 컴포넌트 자동 탐색
- `Accelib.DebugServer` — `IDebugEndpointProvider`: 엔드포인트 마커 인터페이스
- `Accelib.DebugServer` — `[DebugEndpoint]` 어트리뷰트: 메서드에 HTTP 엔드포인트 선언, 리플렉션 자동 라우팅
- `Accelib.DebugServer` — `RequestContextExtensions`: 응답 전송 + POST body 파싱 확장 메서드
- `Accelib.DebugServer` — 내장 엔드포인트: `/api/ping` (연결 확인), `/api/help` (JSON 문서), `/api/help/markdown` (Markdown 문서)
- `Accelib.DebugServer` — `RequestContext`: HTTP 요청 캡슐화 (Method, Path, Body, PathParams)

## [0.1.20] - 2026-03-06

### 추가
- `Accelib.Editor.AutoBuild` — `DiscordWebhookQueue` 큐잉 전송 클래스 추가 (ConcurrentQueue + 백그라운드 HttpClient, 429 Rate Limit 재시도)
- `Accelib.Editor.AutoBuild` — Addressables 빌드 완료 메시지에 이전 빌드 대비 증감량 표시

### 변경
- `Accelib.Editor.AutoBuild` — Discord 알림을 큐잉 방식으로 전환하여 메시지 순서 보장
- `Accelib.Editor.AutoBuild` — 최종 빌드 완료 메시지에 순수 빌드/Addressables/총 용량 3개로 분리 표시

## [0.1.19] - 2026-03-06

### 추가
- `Accelib.UI.Transition` — `TransitionSingleton`에 `onTransitionStarted` UnityEvent 추가 (트랜지션 시작 시 발화, SFX 바인딩용)
- `Accelib.SonityExtension` — 신규 모듈. Sonity SoundEvent 확장 유틸리티
- `Accelib.SonityExtension` — `LoopSoundPlayer` 컴포넌트: duration 기반 Intensity 자동 보간 루프 사운드 재생기
- `Accelib.SonityExtension` — `LoopSoundPlayerEditor`: SoundContainer Loop/Pitch Intensity 원클릭 설정 에디터

## [0.1.18] - 2026-03-04

### 추가
- `Module.SceneManagement` — `SceneRefDrawer` Odin 커스텀 드로어 추가 (한 줄 레이아웃: Enum + 씬/Addressable 필드)

### 변경
- `Module.SceneManagement` — `SceneRef.bool _isBuiltIn` → `ESceneRefType _type` enum 변경 (`FormerlySerializedAs`로 직렬화 호환 유지)

## [0.1.17] - 2026-03-04

### 추가
- `Accelib.OdinExtension` — `SceneDropdownAttribute` 추가: Build Settings 씬 목록을 드롭다운으로 표시하는 Odin 속성 (NaughtyAttributes.Scene 대체)
- `Accelib.OdinExtension` — Runtime/Editor asmdef 분리 (`Accelib.OdinExtension.Runtime` + `Accelib.OdinExtension.Editor`)

### 변경
- `Module.SceneManagement` — `SO_SceneConfig`/`SceneRef`의 `[NaughtyAttributes.Scene]` → `[SceneDropdown]` 교체

## [0.1.16] - 2026-03-04

### 추가
- `Module.SceneManagement` — `SceneRef` 구조체 추가: Addressable/Built-in 씬을 통합 참조하는 직렬화 가능한 타입 (암묵적 `AssetReference` → `SceneRef` 변환 지원)

### 변경
- `Module.SceneManagement` — `SceneManagerAddressable.ChangeScnAsync()` 매개변수 `AssetReference` → `SceneRef`, 반환 타입 `UniTask<SceneInstance?>` → `UniTask<Scene?>` (Built-in 씬 지원, 하위 호환 유지)
- `Module.SceneManagement` — `SO_SceneConfig.GameScn`/`prevScn`/`currScn` 타입 `AssetReference` → `SceneRef`
- `Module.SceneManagement` — `SceneChanger`/`SO_SceneChanger`의 `sceneAsset` 타입 `AssetReference` → `SceneRef`

## [0.1.15] - 2026-03-04

### 변경
- `Module.SceneManagement` — `Resources.UnloadUnusedAssets()`를 `SO_SceneConfig.UnloadUnUsed` 옵션으로 조건부 실행 (기본값: false)
- `Module.SceneManagement` — `SO_SceneConfig`에 `UnloadUnUsed` 필드 추가, `GCOnUnload` 기본값 false로 변경
- `Module.SceneManagement` — `CollectGarbage()`에 디버그 로그 추가 (`Deb.Log`)
- `Accelib.UI.Transition` — `TransitionSingleton`의 `backgroundLoadingPriority` 변경 코드 비활성화 (주석 처리)
- `Accelib.Editor.AutoBuild` — Addressables 빌드 시작/완료 Discord 메시지 포맷 통일 (빌더 정보, 빌드 모드, 크기, 소요시간 표시)

### 수정
- `Accelib.Editor.AutoBuild` — Addressables 크기 측정 시 Remote 폴더 퍼지 매칭 누락 수정 (항상 0 B 표시되던 버그)

## [0.1.14] - 2026-03-03

### 추가
- `Module.SceneManagement` — `SO_SceneConfig.GameScn` 필드 추가 (게임 씬 어드레서블 등록용)
- `Module.SceneManagement` — `SceneManagerAddressable.ChangeScnGameAsync()` 메서드 추가 (게임 씬으로 전환)

## [0.1.13] - 2026-02-27

### 추가
- `Accelib.Editor.AutoBuild` — 빌드 크기 추적 기능 (플레이어 빌드 크기 + Addressables 크기 측정, 직전 빌드 대비 변화량 콘솔/Discord 출력)
- `Accelib.Editor.AutoBuild` — `BuildSizeRecord` 데이터 모델, `BuildSizeUtility` 측정/포맷/히스토리 유틸리티

## [0.1.12] - 2026-02-27

### 수정
- `Accelib.Core` — `MonoSingleton<T>.Awake()` 중복 인스턴스 가드 추가 (DontDestroyOnLoad 씬 재진입 시 assertion 에러 수정)

## [0.1.11] - 2026-02-26

### 추가
- `ScriptableObjectCached<T>` / `SerializedScriptableObjectCached<T>` — 인스펙터 컨텍스트 메뉴에 `ResetInstance` 추가 (에디터 에셋 캐시 초기화 후 재탐색)

### 수정
- `SceneManagerAddressable.AddScnAsync` — Stale OperationHandle 감지 및 해제 추가 (ChangeScnAsync의 Single 모드로 파괴된 Additive 씬의 핸들 잔류 문제 수정)
- `SceneManagerAddressable.AddScnAsync` — null/빈 AssetReference 검증 추가 (LogError)
- `SceneManagerAddressable.RemoveScnAsync` — null/빈 AssetReference 및 이미 해제된 핸들 검증 추가 (LogError/LogWarning)
- `Accelib.Flag` — `SO_TokenFlag.IsActive`에서 파괴된 토큰 자동 정리 (씬 전환 시 플래그 잔류 버그 수정)

## [0.1.10] - 2026-02-24

### 변경
- `Accelib.Conditional` — `Conditional.Evaluate()` 버튼을 리스트 헤더 버튼(`OnTitleBarGUI`)으로 이동

## [0.1.9] - 2026-02-24

### 변경
- `Accelib.Editor.AutoBuild` — Remote 복사 대상을 `{buildDir}/Remote/` → `{exeName}_Data/Remote/`로 변경
- `Accelib.Editor.AutoBuild` — Addressables 빌드 Skip 모드에서도 Remote 폴더 복사 수행

### 수정
- `Accelib.Editor.AutoBuild` — macOS `steamcmd.sh` 실행 권한 문제 수정 (실행 전 `chmod +x` 자동 적용)

## [0.1.8] - 2026-02-24

### 추가
- `Accelib.Editor.AutoBuild` — SteamCMD 사전 검증 추가 (SDK 경로/실행파일 존재 확인, InfoBox 실시간 피드백)
- `Accelib.Editor.AutoBuild` — SteamCMD 로그인 검증 (`TerminalControl.VerifyLogin`) — 빌드 전 로그인 상태 확인, 실패 시 즉시 중단
- `Accelib.Editor.AutoBuild` — SteamCMD 로그인 테스트 버튼 추가 (수동 검증용)
- `Accelib.Editor.AutoBuild` — Addressables 빌드 옵션 (`EAddressablesBuildMode`: Skip/ContentUpdate/CleanBuild)
- `Accelib.Editor.AutoBuild` — Addressables Remote 콘텐츠 자동 복사 (빌드 출력 폴더에 `Remote/` 포함)
- `Accelib.Editor.AutoBuild` — 인스펙터 경로 표시: UnityBuildPath, AddressablesSrcPath, AddressablesDstPath, SteamCmdPath (ReadOnly + 폴더 열기 버튼)
- `Accelib.Editor.AutoBuild` — `Editor/AutoBuild/`에서 `Module/Accelib.Editor.AutoBuild/`로 독립 모듈화
- `Accelib.Editor.AutoBuild` — 모듈 README.md, CHANGELOG.md 신규 작성

### 변경
- `Accelib.Editor.AutoBuild` — asmdef에서 NaughtyAttributes 제거, Addressables 참조 추가
- `Accelib.Editor.AutoBuild` — `AppConfig` 어트리뷰트를 NaughtyAttributes `[Header]`에서 Odin `[TitleGroup]`으로 마이그레이션
- `Accelib.Editor.AutoBuild` — 빌드 파이프라인 Phase 분리 (Phase 0 사전 검증 → Phase 1 준비 → Phase 2 VDF → Phase 3 Addressables → Phase 4 플레이어 빌드 → Phase 5 업로드)
- `Accelib.Editor.AutoBuild` — Discord 빌드 성공 알림에 Addressables 복사 결과 포함
- `Accelib.Editor.AutoBuild` — 인스펙터 레이아웃 재구성 (계정 및 앱 → 앱, Username → 빌드 옵션으로 이동)

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
