# Accelib.Reflection

Reflection 기반 멤버 접근 및 UI 바인딩 모듈.
ScriptableObject의 중첩 필드/프로퍼티 경로를 인스펙터에서 드롭다운으로 선택하고, 런타임에서 캐시된 리플렉션을 통해 값을 읽을 수 있다.

## 디렉토리 구조

```
Accelib.Reflection/
├── Accelib.Reflection.asmdef       # 어셈블리 정의
├── Runtime/
│   ├── MemberRef.cs                # 멤버 참조 클래스 (인스펙터 연동, Subscribe 포함)
│   ├── INotifyValueChanged.cs      # 값 변경 알림 인터페이스
│   ├── Data/
│   │   ├── CachedChain.cs          # 리플렉션 체인 캐시 구조체
│   │   └── ENumericType.cs         # 숫자 타입 열거형
│   └── Utility/
│       └── CachedReflectionUtility.cs  # 런타임 캐시 유틸리티
└── Editor/
    └── Utility/
        └── ReflectionUtility.cs    # 에디터 전용 멤버 스캔 유틸리티
```

## 주요 클래스

### MemberRef

직렬화 가능한 멤버 참조 클래스. 인스펙터에서 ScriptableObject 대상의 멤버 경로를 선택하고, 해당 값을 `double`로 읽는다.

- `target` — 대상 ScriptableObject
- `path` — 점(`.`) 구분 멤버 경로 (예: `stats.health`)
- `Value` — 캐시된 리플렉션을 통해 읽은 double 값
- `GetPreview()` — `"ObjectName.path"` 형태의 미리보기 문자열 반환
- `Subscribe(Action<double>)` — target이 `INotifyValueChanged`를 구현하면 값 변경 구독. 미구현 시 `null` 반환

### INotifyValueChanged

값 변경 알림 인터페이스. MemberRef의 target SO가 이 인터페이스를 구현하면 `MemberRef.Subscribe()`로 값 변경을 감지할 수 있다.

- `event Action OnValueChanged` — 값 변경 이벤트
- `NotifyValueChanged()` — 값 변경 알림 발행

Odin Inspector의 `HorizontalGroup`, `ValueDropdown`, `HideLabel` 등을 사용하여 인스펙터 UI를 구성한다.

### CachedChain

리플렉션 체인 캐시 데이터 구조.

- `Chain` — 각 깊이의 `MemberInfo` 배열 (FieldInfo 또는 PropertyInfo)
- `FinalType` — 체인 끝의 최종 타입
- `IsValid` — 체인 유효 여부

### ENumericType

지원하는 숫자 타입 열거형.

| 분류 | 타입 |
|------|------|
| 정수 | `SByte`, `Byte`, `Short`, `UShort`, `Int`, `UInt`, `Long`, `ULong` |
| 실수 | `Float`, `Double`, `Decimal` |
| 기타 | `Boolean`, `Enum` |

### CachedReflectionUtility

런타임 캐시 기반 리플렉션 유틸리티. 비용이 큰 리플렉션 작업을 한 번만 수행하고 결과를 캐시한다.

- `BuildChain(Object target, string memberPath)` — 멤버 경로를 분석하여 `CachedChain`을 생성한다. 비용이 크므로 한 번만 호출하고 결과를 캐시할 것을 권장한다.
- `BuildDoubleGetter(Object target, CachedChain cached)` — 캐시된 체인을 사용하여 `Func<double>` 클로저를 생성한다. 이후 반복 호출 시 리플렉션 스캔 없이 값을 읽는다.
- `ConvertToDoubleFast(object value)` — 패턴 매칭으로 빠르게 double 변환한다. `int`, `float`, `double` 등 주요 타입에 대해 최적화되어 있다.

### ReflectionUtility (에디터 전용)

`#if UNITY_EDITOR`로 감싸진 에디터 전용 유틸리티. 인스펙터 드롭다운 구성을 위해 대상의 멤버 경로를 탐색한다.

- `GetMemberList(Object target)` — 대상의 모든 읽기 가능한 숫자 멤버 경로를 반환한다.
- `ScanType(Type type, string prefix, int depth)` — 재귀적으로 타입을 스캔한다 (최대 깊이 6).
- `TryGetReactivePropertyValueType(Type t, out Type valueType)` — R3의 `ReactiveProperty<T>` 계열 타입을 감지하고, 내부 `Value` 타입을 반환한다.
- `TryMapNumeric(Type t, out ENumericType numericType)` — 타입이 숫자형인지 판별한다.
- `IsUnityHeavyProperty(Type, PropertyInfo)` — `transform`, `gameObject` 등 무거운 Unity 프로퍼티를 필터링한다.

#### ReactiveProperty 지원

R3(Cysharp/R3)의 `ReactiveProperty<T>` 계열 타입을 자동으로 감지하여 `.Value` 경로를 드롭다운에 추가한다.
R3 어셈블리 참조 없이 타입 이름 기반으로 판별하므로, asmdef에 별도 참조를 추가할 필요가 없다.

지원하는 타입:
- `ReactiveProperty<T>`
- `SerializableReactiveProperty<T>`
- `ReadOnlyReactiveProperty<T>`
- `BindableReactiveProperty<T>`

내부 `T`가 숫자형(`bool` 포함)일 때 `fieldName.Value` 경로가 드롭다운에 노출된다.

## 사용 예시

```csharp
// ScriptableObject에 MemberRef 필드 선언
[Serializable]
public class DamageConfig : ScriptableObject
{
    public MemberRef targetRef;
}
```

1. 인스펙터에서 `target`에 ScriptableObject를 할당한다.
2. 드롭다운에서 원하는 멤버 경로를 선택한다 (예: `stats.health`).
3. 런타임에서 `targetRef.Value`로 해당 값을 `double`로 읽는다.

## 의존성

| 패키지 | 용도 |
|--------|------|
| `Accelib.Preview` | `IPreviewNameProvider` 인터페이스 |
| `Sirenix.OdinInspector` | 인스펙터 UI 어트리뷰트 |

## 네임스페이스

```
Accelib.Reflection                  — MemberRef
Accelib.Reflection.Data             — CachedChain, ENumericType
Accelib.Reflection.Utility          — CachedReflectionUtility, ReflectionUtility
```
