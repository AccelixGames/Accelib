# Accelib.Flag

토큰 기반 플래그 관리 모듈.
여러 시스템(팝업, 트랜지션, 노벨 등)이 `MonoBehaviour`를 토큰으로 사용하여 플래그를 활성화/비활성화할 수 있다.

## 디렉토리 구조

```
Accelib.Flag/
├── Accelib.Flag.asmdef              # 런타임 어셈블리 정의
├── README.md                        # 이 문서
├── CHANGELOG.md                     # 변경 이력
├── Runtime/
│   └── SO_TokenFlag.cs              # 토큰 플래그 ScriptableObject
└── Editor/
    ├── Accelib.Flag.Editor.asmdef   # 에디터 어셈블리 정의
    └── Drawer/
        └── SO_TokenFlagDrawer.cs    # Odin 커스텀 드로어
```

## 주요 클래스

### SO_TokenFlag

`ScriptableObject` 기반 토큰 플래그 관리자. `HashSet<MonoBehaviour>`으로 잠금 토큰을 관리한다.

- `IsActive` — 하나 이상의 토큰이 잠금 중인지 여부 (`bool`). 접근 시 파괴된 토큰 자동 정리
- `LockCount` — 현재 잠금 토큰 수 (`int`)
- `Lock(MonoBehaviour token)` — 플래그 활성화 요청. 중복 호출 시 무시. 상태 변경 시 `OnStateChanged` 발생
- `Unlock(MonoBehaviour token)` — 플래그 비활성화 요청. 상태 변경 시 `OnStateChanged` 발생
- `ForceUnlockAll()` — 모든 잠금 강제 해제. 씬 전환 시 활용
- `OnStateChanged` — `event Action<bool>`. 인자는 `IsActive` 값

## 사용 예시

```csharp
// SO 에셋을 인스펙터에서 연결
[SerializeField] private SO_TokenFlag showCursor;

// 플래그 활성화 (자기 자신을 토큰으로)
showCursor.Lock(this);

// 플래그 비활성화
showCursor.Unlock(this);

// 외부 시스템에서 상태 구독
showCursor.OnStateChanged += isActive =>
{
    if (isActive) Cursor.visible = true;
    else Cursor.visible = false;
};
```

## 에디터 드로어

`SO_TokenFlag`를 필드로 참조하면, 인스펙터에서 다음과 같이 표시된다:

```
[IsActive 토글(ReadOnly)] [ObjectField]
```

- **토글:** 현재 `IsActive` 상태를 실시간으로 표시 (편집 불가, 런타임 토큰 기반)
- **ObjectField:** SO_TokenFlag asset을 드래그&드롭으로 할당

## 의존성

| 패키지 | 용도 |
|--------|------|
| `Odin Inspector` | 런타임: 디버깅용 인스펙터 표시, 에디터: 커스텀 드로어 |

## 네임스페이스

```
Accelib.Flag              — SO_TokenFlag
Accelib.Flag.Editor       — SO_TokenFlagDrawer (Editor 전용)
```
