# Accelib.SonityExtension

Sonity SoundEvent 확장 유틸리티 모듈.
Intensity 기반 루프 사운드 재생/정지를 캡슐화하여 인스펙터 중심으로 사운드 변조를 설정한다.

## 디렉토리 구조

```
Accelib.SonityExtension/
├── Accelib.SonityExtension.asmdef     # 런타임 어셈블리
├── README.md                          # 문서
├── CHANGELOG.md                       # 변경 이력
├── Runtime/
│   └── LoopSoundPlayer.cs             # Intensity 기반 루프 사운드 재생기
└── Editor/
    ├── Accelib.SonityExtension.Editor.asmdef  # 에디터 어셈블리
    └── LoopSoundPlayerEditor.cs               # 커스텀 에디터 (OdinEditor 기반)
```

## 주요 클래스

### LoopSoundPlayer

Intensity 기반 루프 사운드 재생/정지를 캡슐화하는 MonoBehaviour 컴포넌트.

SoundContainer의 **Intensity → Pitch/Volume 커브**를 활용하여,
인스펙터에서 사운드 변조 곡선을 설정하고 코드에서는 `Play(duration)` 호출만으로 자동 변조된다.

#### 필드

- `loopSound` — SoundEvent 할당 (SoundContainer에 Loop 설정 필요)
- `startIntensity` — 재생 시작 Intensity (0~1)

#### 프로퍼티

- `IsPlaying` — 현재 재생 중 여부
- `Intensity` — 현재 Intensity 값 (0~1). ProgressBar로 시각화

#### 메서드

- `Play(float duration)` — 루프 사운드 재생. duration 동안 startIntensity→1 자동 보간
- `Stop(bool allowFadeOut = true)` — 정지 (페이드아웃 옵션)

### LoopSoundPlayerEditor (Editor)

OdinEditor 기반 커스텀 에디터. Odin 어트리뷰트를 유지하면서 SoundContainer Quick Setup GUI를 추가한다.

#### 기능

- **현재 상태 표시** — 할당된 SoundEvent의 SoundContainer별 Loop/PitchIntensity 설정 읽기전용 표시
- **Apply Loop + Pitch Intensity** — 모든 SC에 Loop + Pitch Intensity 원클릭 적용
- **Apply Loop Only** — Loop 기본 설정만 적용

## 사용 예시

```csharp
using Accelib.SonityExtension.Runtime;

[SerializeField] private LoopSoundPlayer cookingSound;

// 요리 시작 (5초간 피치 상승)
cookingSound.Play(5f);

// 요리 종료
cookingSound.Stop();
```

### SoundContainer 인스펙터 설정 (수동)

1. SoundContainer 선택 → `Loop` 체크
2. `Intensity` 섹션 → **Pitch 커브** 설정 (예: 0→0, 1→12 semitone = 1옥타브 상승)
3. 필요 시 **Volume 커브**도 설정 (점점 커지는 효과)

### SoundContainer 인스펙터 설정 (에디터 버튼)

1. LoopSoundPlayer 컴포넌트에 SoundEvent 할당
2. 인스펙터 하단 **SoundContainer Quick Setup** 펼치기
3. Pitch Low/High semitone 설정
4. **Apply Loop + Pitch Intensity** 버튼 클릭

## Odin 디버그 기능

런타임에서만 활성화되는 디버그 버튼:
- **Play** — `EnableIf(Application.isPlaying && !IsPlaying)`
- **Stop** — `EnableIf(Application.isPlaying && IsPlaying)`
- **Intensity** — `ProgressBar(0, 1)` 시각화

## 의존성

| 패키지 | 용도 |
|--------|------|
| `Sonity` (Sonigon) | SoundEvent, SoundParameterIntensity, UpdateMode |
| `Odin Inspector` | 디버그 어트리뷰트 (Button, ProgressBar, ShowInInspector), 커스텀 에디터 (OdinEditor) |

## 네임스페이스

```
Accelib.SonityExtension.Runtime    — LoopSoundPlayer
Accelib.SonityExtension.Editor     — LoopSoundPlayerEditor
```
