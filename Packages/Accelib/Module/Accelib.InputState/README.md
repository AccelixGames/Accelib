# Accelib.InputState

토큰 기반 입력 상태 관리 모듈.
여러 시스템(팝업, 대화, 가구 배치 등)이 `GameObject`를 토큰으로 사용하여 입력 잠금을 요청/해제할 수 있다.

> **TODO:** 프로젝트 분석에 맞춰 기능 업그레이드 필요

## 디렉토리 구조

```
Accelib.InputState/
├── Accelib.InputState.asmdef   # 어셈블리 정의
├── README.md                   # 이 문서
├── CHANGELOG.md                # 변경 이력
└── Runtime/
    └── SO_InputState.cs        # 입력 상태 ScriptableObject
```

## 주요 클래스

### SO_InputState

`ScriptableObject` 기반 입력 상태 관리자. `HashSet<GameObject>`으로 잠금 토큰을 관리한다.

- `IsLocked` — 하나 이상의 토큰이 잠금 중인지 여부 (`bool`)
- `LockCount` — 현재 잠금 토큰 수 (`int`)
- `Lock(GameObject token)` — 잠금 요청. 중복 호출 시 무시. 상태 변경 시 `OnStateChanged` 발생
- `Unlock(GameObject token)` — 잠금 해제. 상태 변경 시 `OnStateChanged` 발생
- `ForceUnlockAll()` — 모든 잠금 강제 해제. 씬 전환 시 활용
- `OnStateChanged` — `event Action<bool>`. 인자는 `IsLocked` 값

## 사용 예시

```csharp
// SO 에셋을 인스펙터에서 연결
[SerializeField] private SO_InputState inputState;

// 잠금 요청 (자기 자신의 gameObject를 토큰으로)
inputState.Lock(gameObject);

// 잠금 해제
inputState.Unlock(gameObject);

// 외부 시스템에서 상태 구독
inputState.OnStateChanged += isLocked =>
{
    if (isLocked) Cursor.visible = true;
    else Cursor.visible = false;
};
```

## 의존성

| 패키지 | 용도 |
|--------|------|
| (없음) | 외부 의존성 없음 |

## 네임스페이스

```
Accelib.InputState              — SO_InputState
```
