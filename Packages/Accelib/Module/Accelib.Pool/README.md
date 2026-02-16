# Accelib.Pool

오브젝트 풀링 모듈.
Stack 기반 리소스 풀, 델리게이트 구동 컴포넌트 풀, MonoBehaviour 프리팹 풀을 제공한다.

## 디렉토리 구조

```
Accelib.Pool/
├── Accelib.Pool.asmdef          # 어셈블리 정의
├── README.md                    # 문서
├── CHANGELOG.md                 # 변경 이력
└── Runtime/
    ├── Core/                    # 순수 제네릭 풀링 기반
    │   ├── IPoolTarget.cs       # 풀 대상 인터페이스
    │   ├── ResourcePool.cs      # Stack 기반 리소스 풀
    │   └── ComponentPool.cs     # 델리게이트 구동 컴포넌트 풀
    └── Unity/                   # MonoBehaviour/Transform 의존 풀
        └── PrefabPool.cs        # 프리팹 전용 풀
```

## 주요 클래스

### IPoolTarget
- 풀 대상 오브젝트가 구현하는 인터페이스
- `OnRelease()`: 풀에 반환될 때 호출. 기본 구현은 빈 메서드

### ResourcePool\<T\> (abstract)
- `where T : IPoolTarget, new()`
- Stack 기반 LIFO 풀. MonoBehaviour가 아닌 일반 객체에 사용
- `Get()`: 풀에서 꺼내거나 `New()`로 생성
- `Dispose(T)`: `OnRelease()` 호출 후 풀에 반환
- `DestroyAll()`: 풀 내 모든 객체 파괴
- abstract: `New()`, `OnDestroy(T)`

### ComponentPool\<T\>
- `where T : class`
- List 기반 풀. 생성/풀링/반환 동작을 델리게이트로 외부에서 주입
- `New`: 생성 함수 (`Func<T>`)
- `OnPooled`: 풀에서 꺼낼 때 콜백 (`Action<T>`)
- `OnReleased`: 풀에 반환할 때 콜백 (`Action<T>`)
- `Get()`: 풀에서 꺼내거나 `New`로 생성, `OnPooled` 호출
- `Release(T)`: 중복 방지 후 `OnReleased` 호출, 풀에 반환

### PrefabPool\<T\>
- `where T : MonoBehaviour`, ComponentPool\<T\> 확장
- 프리팹 인스턴스화 및 활성/비활성 관리
- `Initialize()`: 프리팹/부모 Transform 기반 초기화, 기존 자식 자동 탐색
- `Get()`: 활성 목록에 추가하며 꺼냄
- `Release(T)`: 활성 목록에서 제거 후 풀에 반환
- `ReleaseAll()`: 모든 활성 인스턴스를 풀에 반환
- `EnabledList`: 현재 활성 인스턴스 목록 (읽기 전용)

## 사용 예시

### ResourcePool 상속 (비-MonoBehaviour)

```csharp
using Accelib.Pool;

// 풀 대상 클래스
public class Work : IPoolTarget
{
    public object Target;
    public float UpdatedAt;

    public void OnRelease()
    {
        Target = null;
        UpdatedAt = -1f;
    }
}

// 풀 구현
[System.Serializable]
public class WorkPool : ResourcePool<Work>
{
    protected override Work New() => new Work();
    protected override void OnDestroy(Work resource) { }
}
```

### PrefabPool 직렬화 (MonoBehaviour)

```csharp
using Accelib.Pool;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private PrefabPool<MyIconUI> iconPool;

    private void Awake()
    {
        iconPool.Initialize();
    }

    private void SpawnIcon()
    {
        var icon = iconPool.Get();
        icon.Setup(data);
    }

    private void DespawnIcon(MyIconUI icon)
    {
        iconPool.Release(icon);
    }

    private void DespawnAll()
    {
        iconPool.ReleaseAll();
    }
}
```

## 의존성

| 패키지 | 용도 |
|--------|------|
| Odin Inspector | ShowInInspector, TitleGroup, ReadOnly 어트리뷰트 |

## 네임스페이스

```
Accelib.Pool — 모든 풀 클래스 (IPoolTarget, ResourcePool, ComponentPool, PrefabPool)
```
