# Accelib.UI.Transition

화면 전환(트랜지션) 이펙트 시스템.
페이드, 마스크, 도어, 팝, 렉트 등 다양한 전환 효과를 지원한다.

## 디렉토리 구조

```
Accelib.UI.Transition/
├── Accelib.UI.Transition.asmdef      # 어셈블리 정의
├── README.md                         # 이 문서
├── CHANGELOG.md                      # 변경 이력
└── Runtime/
    ├── TransitionSingleton.cs        # 싱글톤 트랜지션 매니저
    └── Effect/
        ├── TransitionEffect.cs       # 추상 이펙트 베이스
        ├── TransitionEffect_Door.cs  # 도어 이펙트
        ├── TransitionEffect_Fade.cs  # 페이드 이펙트
        ├── TransitionEffect_Mask.cs  # 마스크 이펙트
        ├── TransitionEffect_Pop.cs   # 팝 이펙트
        └── TransitionEffect_Rect.cs  # 렉트 이펙트
```

## 주요 클래스

### TransitionSingleton

`MonoSingleton<TransitionSingleton>` 기반 트랜지션 매니저. 여러 이펙트 중 인덱스로 선택하여 전환한다.

- `StartTransition(int index)` — 트랜지션 시작 (static). `Sequence` 반환
- `EndTransition()` — 트랜지션 종료 (static). `Sequence` 반환
- `IsActive` — 현재 트랜지션 활성 여부 (static)

**커서 플래그 연동:** `SO_TokenFlag showCursor` 필드(Optional)를 통해 트랜지션 시작/종료 시 자동 Lock/Unlock 호출.

### TransitionEffect

모든 전환 이펙트의 추상 베이스 클래스. DOTween `Sequence`로 애니메이션을 구성한다.

- `StartTransition()` — 전환 시작 애니메이션. `Sequence` 반환
- `EndTransition()` — 전환 종료 애니메이션. `Sequence` 반환
- `IsActive` — 이펙트 캔버스 활성 여부
- `duration` — 전환 시간 (0.01 ~ 5초)
- `easeStart` / `easeEnd` — DOTween Ease 설정

### 이펙트 종류

| 이펙트 | 설명 |
|--------|------|
| `TransitionEffect_Fade` | 캔버스 그룹 알파 페이드. 로딩 그룹 지원 |
| `TransitionEffect_Mask` | 원형 마스크 축소/확대. 사운드 이펙트 지원 |
| `TransitionEffect_Door` | 좌/우 도어 닫힘/열림 |
| `TransitionEffect_Pop` | 회전 + 스케일 팝 |
| `TransitionEffect_Rect` | RectTransform 앵커/사이즈 트위닝 |

## 사용 예시

```csharp
// 트랜지션 시작 (기본 이펙트)
await TransitionSingleton.StartTransition();

// 씬 로드 등 작업 수행...

// 트랜지션 종료
await TransitionSingleton.EndTransition();
```

## 의존성

| 패키지 | 용도 |
|--------|------|
| `Accelib.Runtime` | MonoSingleton, AudioRefSO |
| `Accelib.Flag` | 커서 플래그 연동 (Optional) |
| `DOTween` | 트위닝 애니메이션 |
| `Odin Inspector` | 인스펙터 어트리뷰트 (ShowIf, ReadOnly 등) |
| `NaughtyAttributes` | 인스펙터 어트리뷰트 (Button) |

## 네임스페이스

```
Accelib.Module.Transition               — TransitionSingleton
Accelib.Module.Transition.Effect        — TransitionEffect, TransitionEffect_*
```
