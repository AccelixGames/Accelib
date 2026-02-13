# Accelib.R3Extension

R3 Observable 확장 메서드 모듈.
자주 사용하는 연산자 조합을 간결한 메서드로 제공한다.

## 디렉토리 구조

```
Accelib.R3Extension/
├── Accelib.R3Extension.asmdef     # 어셈블리 정의
├── README.md                      # 문서
├── CHANGELOG.md                   # 변경 이력
└── Runtime/
    └── ObservableExtension.cs     # Observable<T> 확장 메서드
```

## 주요 클래스

### ObservableExtension

`Observable<T>`에 대한 확장 메서드를 제공하는 정적 클래스.

#### Delta() (int 전용)

연속된 두 값의 차이(델타)를 발행하는 Observable을 반환한다.

- **내부 체인:** `DistinctUntilChanged()` → `Pairwise()` → `Select(Current - Previous)`
- **용도:** ReactiveProperty의 값 변화량을 구독할 때 반복적인 연산자 체인을 제거
- **반환:** `Observable<int>` — 이전 값과 현재 값의 차이

## 사용 예시

```csharp
using Accelib.R3Extension.Runtime;
using R3;

// Before (반복적인 체인)
status.Exp.Skip(1).DistinctUntilChanged().Pairwise().Subscribe(pair =>
{
    var amount = pair.Current - pair.Previous;
    if (amount > 0) expUI.Alert($"+{amount} xp");
});

// After (Delta() 사용)
status.Exp.Delta().Subscribe(delta =>
{
    if (delta > 0) expUI.Alert($"+{delta} xp");
});
```

## 의존성

| 패키지 | 용도 |
|--------|------|
| `R3` (Cysharp/R3) | `Observable<T>`, `Pairwise()`, `DistinctUntilChanged()` 등 |

## 네임스페이스

```
Accelib.R3Extension.Runtime    — ObservableExtension
```
