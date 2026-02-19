# Changelog

이 문서는 Accelib.Reflection 모듈의 주요 변경 내역을 기록한다.

## [0.5.0] - 2026-02-19

### 추가
- `MemberRef.Subscribe` — ReactiveProperty 자동 감지 구독 기능 추가
  - 체인 내 ReactiveProperty<T> 객체를 자동 탐색하여 R3 Subscribe를 리플렉션으로 호출
  - R3 어셈블리 참조 없이 동작 (타입 이름 기반 판별 + 리플렉션)
  - 우선순위: ReactiveProperty 자동 구독 → INotifyValueChanged fallback
  - SO에 INotifyValueChanged를 구현하지 않아도 내부 ReactiveProperty가 있으면 자동 구독됨
- `CachedChain.ReactivePropertyChainLength` — 체인 내 ReactiveProperty 위치 정보 저장

## [0.4.1] - 2026-02-19

### 수정
- `CachedReflectionUtility.BuildChain` — ReactiveProperty 계열 타입의 `.Value` 경로가 `ReadOnlyReactiveProperty<T>`에서 리졸브 실패하던 버그 수정
  - `ReadOnlyReactiveProperty<T>`에는 `Value` 프로퍼티가 없고 `CurrentValue`만 존재함
  - `.Value` 세그먼트를 자동으로 `CurrentValue`로 치환하는 `IsReactivePropertyType()` 판별 로직 추가
  - 기존 에셋의 `"Level.Value"` 경로가 런타임에서 정상 동작함
- `ReflectionUtility.ScanType` — ReactiveProperty 계열 드롭다운 경로를 `.Value` → `.CurrentValue`로 변경
  - 에디터와 런타임의 경로 생성 방식 통일

## [0.4.0] - 2026-02-19

### 추가
- `INotifyValueChanged` 인터페이스 신규 생성 (`Runtime/INotifyValueChanged.cs`)
  - `event Action OnValueChanged` — 값 변경 이벤트
  - `NotifyValueChanged()` — 값 변경 알림 메서드
  - MemberRef 구독 시 대상 SO가 이 인터페이스를 구현하면 값 변경을 감지할 수 있다
- `MemberRef.Subscribe(Action<double>)` — target이 `INotifyValueChanged`를 구현하면 값 변경을 구독한다
  - 내부 `CallbackDisposable` 클래스로 구독/해제 관리

## [0.3.0] - 2026-02-13

### 변경
- 폴더 구조를 UPM 표준에 맞게 재구성
  - `Runtime/` — 런타임 코드 (`MemberRef`, `Data/`, `Utility/`)
  - `Editor/` — 에디터 전용 코드 (`Utility/ReflectionUtility`)
- `Model/` → `Runtime/Data/`로 폴더명 변경
- 네임스페이스 변경: `Accelib.Reflection.Model` → `Accelib.Reflection.Data`

## [0.2.0] - 2026-02-13

### 추가
- `ReflectionUtility`에 R3 `ReactiveProperty<T>` 계열 타입 지원 추가
  - `TryGetReactivePropertyValueType()` 메서드 추가
  - `ReactiveProperty<T>`, `SerializableReactiveProperty<T>`, `ReadOnlyReactiveProperty<T>`, `BindableReactiveProperty<T>` 감지
  - 내부 `T`가 숫자형일 때 `fieldName.Value` 경로를 드롭다운에 자동 노출
  - R3 어셈블리 참조 없이 타입 이름 기반 판별 (asmdef 수정 불필요)

### 문서
- `README.md` 생성 (모듈 개요, 디렉토리 구조, 클래스 설명, 사용 예시, 의존성)
- `CHANGELOG.md` 생성

## [0.1.0] - 초기 버전

### 추가
- `MemberRef` — ScriptableObject의 중첩 멤버 경로를 인스펙터에서 선택·읽기
- `CachedChain` — 리플렉션 체인 캐시 데이터 구조
- `ENumericType` — 지원하는 숫자 타입 열거형
- `CachedReflectionUtility` — 런타임 캐시 기반 멤버 접근 유틸리티 (`BuildChain`, `BuildDoubleGetter`, `ConvertToDoubleFast`)
- `ReflectionUtility` — 에디터 전용 멤버 스캔 유틸리티 (`GetMemberList`, `ScanType`, `TryMapNumeric`)
