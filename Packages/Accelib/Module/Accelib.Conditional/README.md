# Accelib.Conditional

조건식 평가 시스템. 비교/논리 연산자로 규칙 기반 로직을 구성하고, 인스펙터에서 조건을 편집하며 텍스트 미리보기를 제공한다.

## 디렉토리 구조

```
Accelib.Conditional/
├── Accelib.Conditional.asmdef       # 어셈블리 정의
├── README.md                        # 모듈 문서
├── CHANGELOG.md                     # 변경 이력
└── Runtime/
    ├── Condition.cs                 # 단일 조건 구조체
    ├── Conditional.cs               # 다중 조건식 컨테이너
    ├── ValueProvider.cs             # 값 제공자 (리터럴/SO/MemberRef)
    ├── SO_Conditional.cs            # 조건식 ScriptableObject 래퍼
    ├── SO_PresetValue.cs            # MemberRef 프리셋 값 제공자
    ├── SO_ValueProviderBase.cs      # 값 제공자 추상 베이스
    ├── Data/
    │   ├── EComparisonOperator.cs   # 비교 연산자 열거형
    │   ├── ELogicalOperator.cs      # 논리 연산자 열거형
    │   └── EValueSourceType.cs      # 값 소스 타입 열거형
    └── Utility/
        └── OperatorStringUtility.cs # 연산자 문자열 변환 유틸
```

## 주요 클래스

### Conditional

다중 조건식 컨테이너. `List<Condition>`을 논리 연산자(AND/OR)로 연결하여 평가한다.

- `Evaluate()` — 전체 조건식 평가 (bool 반환)
- `Preview` — 조건식의 텍스트 미리보기 (읽기 전용)
- `Subscribe(Action)` — 조건 내 모든 ValueProvider의 값 변경을 구독. 값이 바뀌면 콜백 발행. `IDisposable` 반환 (구독 해제용)

인스펙터에서 Odin의 ListDrawer로 조건을 추가/제거/순서 변경할 수 있으며, 리스트 헤더의 Evaluate 버튼으로 실시간 평가 결과를 확인할 수 있다.

### Condition

단일 조건 (`좌변 연산자 우변`). 논리 연산자로 다음 조건과 연결된다.

- `LogicalOperator` — 다음 조건과의 논리 연산자 (And/Or)
- `Evaluate()` — 현재 조건 평가 (bool 반환)
- `Preview` — 조건의 텍스트 표현 (예: `'MaidData.level'[5] >= 3`)
- `SubscribeLhs(Action<double>)` — 좌변 값 변경 구독
- `SubscribeRhs(Action<double>)` — 우변 값 변경 구독

### ValueProvider

값 제공자. 다양한 소스에서 double 값을 제공한다.

**지원하는 값 소스:**
- `Integer` — 정수 리터럴
- `Double` — 실수 리터럴
- `Boolean` — bool 리터럴 (true=1, false=0)
- `ScriptableObject` — `SO_ValueProviderBase` 참조
- `Custom` — `MemberRef` (Accelib.Reflection 기반 리플렉션 멤버 접근)

주요 멤버:
- `Value` — 평가된 double 값
- `Preview` — 값 소스의 미리보기 문자열
- `CompareTo(ValueProvider, EComparisonOperator)` — 다른 값과 비교
- `Subscribe(Action<double>)` — SO 또는 Custom(MemberRef) 소스의 값 변경 구독

### SO_Conditional

조건식을 ScriptableObject 에셋으로 저장하는 래퍼.

- `Conditional` — 내부 `Conditional` 인스턴스

### SO_ValueProviderBase

값 제공자 ScriptableObject의 추상 베이스 클래스.

- `Preview` — 미리보기 문자열 (추상)
- `Value` — double 값 (추상)
- `Subscribe(Action<double>)` — 값 변경 구독 (가상, 기본 `null` 반환). 서브클래스에서 오버라이드하여 구독 지원 가능

### SO_PresetValue

`MemberRef` 기반 프리셋 값 제공자. `SO_ValueProviderBase`를 상속하여 리플렉션 멤버 참조를 ScriptableObject로 저장한다.

### 열거형

| 열거형 | 설명 |
|--------|------|
| `EComparisonOperator` | 비교 연산자: `==`, `!=`, `>`, `>=`, `<`, `<=` |
| `ELogicalOperator` | 논리 연산자: `And`, `Or` |
| `EValueSourceType` | 값 소스 타입: `Integer`, `Double`, `Boolean`, `ScriptableObject`, `Custom` |

### OperatorStringUtility

비교 연산자를 문자열 기호로 변환하는 확장 메서드.

- `ToStringSign(EComparisonOperator)` — 연산자를 문자열로 변환 (예: `Equal` → `"=="`)

## 사용 예시

```csharp
// 1. SO_Conditional 에셋을 인스펙터에서 구성
//    조건 예시: PlayerLevel >= 5 AND CafeTier == 2

// 2. 런타임에서 평가
if (conditionalAsset.Conditional.Evaluate())
{
    // 조건 충족
}

// 3. SO_ValueProviderBase를 상속하여 커스텀 값 소스 구현
public class MyValueProvider : SO_ValueProviderBase
{
    public override string Preview => "MyValue";
    public override double Value => 42;
}
```

## 의존성

| 패키지 | 용도 |
|--------|------|
| `Accelib.Reflection` | `MemberRef` (리플렉션 멤버 참조) |
| `Accelib.Preview` | `IPreviewNameProvider` 인터페이스 |
| `Sirenix.OdinInspector` | 인스펙터 UI 어트리뷰트 |
| `Unity.Collections` | Unity 컬렉션 타입 |
| `ZLinq.Unity` | LINQ 확장 |

## 네임스페이스

```
Accelib.Conditional                 — Conditional, Condition, ValueProvider, SO_* 클래스
Accelib.Conditional.Data            — EComparisonOperator, ELogicalOperator, EValueSourceType
Accelib.Conditional.Utility         — OperatorStringUtility
```
