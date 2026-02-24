# Accelib.Editor.AutoBuild

Unity 빌드 → SteamCMD 업로드 → Discord 알림을 자동화하는 에디터 전용 모듈.
인스펙터에서 빌드 설정을 구성하고 원클릭으로 전체 파이프라인을 실행한다.

## 디렉토리 구조

```
Accelib.Editor.AutoBuild/
├── Accelib.Editor.AutoBuild.asmdef   # 어셈블리 정의 (Editor 전용)
├── AutoBuildConfig.cs                # 빌드 자동화 설정 SO (핵심)
├── Architecture/
│   ├── AppConfig.cs                  # Steam 앱 설정 (앱 ID, 브랜치, 디포)
│   ├── BuildInfo.cs                  # 빌드 정보 구조체
│   ├── DepotConfig.cs                # 디포 설정 구조체
│   ├── EAddressablesBuildMode.cs     # Addressables 빌드 모드 열거형
│   └── UploadInfo.cs                 # 업로드 정보 구조체
├── Discord/
│   ├── DiscordWebhook.cs             # Discord Webhook 전송 유틸리티
│   ├── JDiscordEmbed.cs              # Discord Embed JSON 구조체
│   └── JDiscordMsg.cs                # Discord Message JSON 구조체
├── Steamwork/
│   ├── DepotUtility.cs               # VDF 스크립트 생성 유틸리티
│   ├── TerminalUtility.cs            # SteamCMD 실행 디스패치
│   └── Control/
│       ├── TerminalControl.cs        # 터미널 제어 추상 베이스
│       ├── TerminalControl_Windows.cs # Windows 터미널 구현
│       └── TerminalControl_OSX.cs    # macOS 터미널 구현
└── AppInToss/                        # 프로젝트별 빌드 설정 에셋
    └── AutoBuildConfig_AIT.cs        # AppInToss 전용 설정
```

## 빌드 파이프라인 플로우

```
Phase 0: 사전 검증
  → SDK 경로, steamcmd 파일, Username, 로그인, Discord URL, 활성 디포

Phase 1: 준비
  → 씬 저장, 버전 설정, StackTrace 비활성화

Phase 2: VDF 스크립트 생성
  → 앱/디포별 .vdf 파일 생성, BuildInfo/UploadInfo 수집

Phase 3: Addressables 빌드 (선택)
  → Skip / CleanBuild / ContentUpdate

Phase 4: 플레이어 빌드
  → 디포별 BuildPlayer → Remote 콘텐츠 복사 → Discord 결과 알림

Phase 5: SteamCMD 업로드 (선택)
  → steamcmd +login +run_app_build_http +quit → Discord 결과 알림
```

## 주요 클래스

### AutoBuildConfig

빌드 자동화의 핵심 ScriptableObject. 인스펙터에서 모든 설정을 구성하고 빌드를 실행한다.

- `StartBuildProcess()` — 전체 빌드 파이프라인 실행 (Phase 0~5)
- `TestSteamCmdLogin()` — SteamCMD 로그인 수동 테스트
- `Validate_PreCheck()` — 빌드 전 사전 검증 (경로/파일/로그인 등)
- `Internal_BuildAddressables()` — Addressables 빌드 수행 (모드에 따라 분기)
- `Internal_CopyAddressablesRemote()` — Remote 콘텐츠를 빌드 출력 폴더에 복사
- `Internal_Build()` — 플레이어 빌드 실행, 에러 시 Discord 알림 후 throw

#### 인스펙터 구성

| 섹션 | 필드 | 설명 |
|------|------|------|
| #앱 | `apps` | Steam 앱/디포 설정 리스트 |
| #디스코드 메세지 | `sendDiscordMessage`, `discordWebhookUrl` | Discord 알림 토글 및 Webhook URL |
| #빌드 옵션 | `Username` | EditorPrefs 기반 Steam 계정명 |
| #빌드 옵션 | `sdkPath` | Steamworks SDK 루트 경로 (검증 InfoBox 포함) |
| #빌드 옵션 | `version` | 빌드 버전 (Vector3Int) |
| #빌드 옵션 | `SteamCmdPath` | steamcmd 실행파일 경로 (ReadOnly, computed) |
| #빌드 옵션 | `UnityBuildPath` | 빌드 출력 경로 (ReadOnly + 폴더 열기) |
| #빌드 옵션 | `AddressablesSrcPath` | Addressables Remote 원본 경로 (ReadOnly + 폴더 열기) |
| #빌드 옵션 | `AddressablesDstPath` | Addressables Remote 복사 대상 경로 (ReadOnly + 폴더 열기) |
| #빌드 | `addressablesBuildMode` | Addressables 빌드 모드 (Skip/ContentUpdate/CleanBuild) |
| #빌드 | `skipBuild`, `skipUpload` | 빌드/업로드 건너뛰기 토글 |
| #빌드 | `patchNote` | 패치 노트 텍스트 |

### EAddressablesBuildMode

Addressables 빌드 모드 열거형.

| 값 | 설명 |
|----|------|
| `Skip` | Addressables 빌드 건너뜀 |
| `ContentUpdate` | 기존 빌드 기반 콘텐츠 업데이트 |
| `CleanBuild` | 전체 클린 빌드 |

### AppConfig

Steam 앱 설정.

- `name` — 앱 이름 (폴더명으로도 사용)
- `appID` — Steam 앱 ID
- `liveBranch` — 배포 브랜치명
- `depots` — 디포 설정 리스트

### TerminalUtility

SteamCMD 실행 유틸리티. 플랫폼에 따라 적절한 `TerminalControl`을 생성하여 디스패치한다.

- `OpenTerminal(sdkPath, username, uploadInfos)` — SteamCMD 업로드 실행
- `VerifyLogin(sdkPath, username)` — SteamCMD 로그인 검증 (exit code 0 = 성공)

### DiscordWebhook

Discord Webhook API를 통해 메시지를 전송한다.

- `SendMsg(url, message)` — 일반 텍스트 메시지 전송
- `SendMsg(url, message, embed)` — Embed 포함 메시지 전송
- `SendMsg(url, JDiscordMsg)` — 구조화된 메시지 전송

## 사용 예시

1. `Assets > Create > Accelib > AutoBuildConfig`로 설정 에셋을 생성한다.
2. 인스펙터에서 Steam 앱 정보, SDK 경로, Discord Webhook URL을 설정한다.
3. `SteamCMD 로그인 테스트` 버튼으로 로그인을 확인한다.
4. Addressables 빌드 모드를 선택한다 (필요 시).
5. `빌드` 버튼을 클릭하면 전체 파이프라인이 자동 실행된다.

## 의존성

| 패키지 | 용도 |
|--------|------|
| `Odin Inspector` | 인스펙터 UI 어트리뷰트 (TitleGroup, InfoBox, Button 등) |
| `UniTask` | 비동기 처리 |
| `Unity.Addressables` | Addressables 빌드 API |
| `Unity.Addressables.Editor` | Addressables 에디터 API (ContentUpdateScript 등) |

## 네임스페이스

```
Accelib.Editor                              — AutoBuildConfig
Accelib.Editor.Architecture                 — AppConfig, BuildInfo, DepotConfig, UploadInfo, EAddressablesBuildMode
Accelib.Editor.Steamwork                    — TerminalUtility, TerminalControl
Accelib.Editor.AutoBuild.Steamwork          — DepotUtility
Accelib.Editor.Utility.Discord              — DiscordWebhook, JDiscordEmbed, JDiscordMsg
```
