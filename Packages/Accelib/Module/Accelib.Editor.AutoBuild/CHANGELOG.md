# Changelog

이 문서는 Accelib.Editor.AutoBuild 모듈의 주요 변경 내역을 기록한다.

## [0.1.17] - 2026-03-10

### 수정
- Addressables 빌드 실패 시 에러 사유가 Discord에 출력되지 않던 문제 수정 — 최상위 catch 블록에 Discord Embed 실패 알림 추가
- `BuildPlayerContent` / `BuildContentUpdate` 결과가 null인 경우 에러로 감지하지 않던 문제 수정

## [0.1.16] - 2026-03-06

### 추가
- `DiscordWebhookQueue` — Discord Webhook 메시지 큐잉 전송 클래스 신규 추가 (ConcurrentQueue + 백그라운드 HttpClient, 429 Rate Limit 재시도 지원)
- Addressables 빌드 완료 메시지에 이전 빌드 대비 증감량 표시 추가

### 변경
- Discord 알림을 `DiscordWebhook`(fire-and-forget) → `DiscordWebhookQueue`(큐잉)로 전환하여 메시지 순서 보장
- 최종 빌드 완료 메시지에 순수 빌드/Addressables/총 용량 3개로 분리 표시
- 불필요한 `Thread.Sleep(10)` 제거

## [0.1.15] - 2026-03-04

### 변경
- Addressables 빌드 시작 메시지에 빌더 정보(`*Username | version | note*`) 및 빌드 모드(`- CleanBuild`) 표시 추가
- Addressables 빌드 완료 메시지에 크기 및 소요시간 표시 추가

### 수정
- Addressables 크기 측정 시 Remote 폴더 퍼지 매칭 누락 수정 — `StandaloneWindows64` ↔ `StandaloneWindows` 등 폴더명 불일치로 항상 0 B 표시되던 버그
- `FindRemoteSrcPath()` 헬퍼 추출 — 복사(`Internal_CopyAddressablesRemote`)와 측정(`Internal_MeasureAndReportBuildSize`)에서 동일한 경로 탐색 로직 재사용
- 인스펙터 `AddressablesSrcPath` 표시에도 퍼지 매칭 적용

## [0.1.13] - 2026-02-27

### 추가
- 빌드 크기 추적 기능 — 플레이어 빌드 크기 + Addressables 크기 측정 및 보고
- 빌드 크기 히스토리 — 직전 빌드 기록을 JSON 파일로 보관 (`Library/Accelib/AutoBuild/`)
- 이전 빌드 대비 크기 변화량 콘솔 로그 및 Discord 알림에 출력 (절대값 + 백분율)
- `BuildSizeRecord` — 빌드 크기 기록 데이터 모델
- `BuildSizeUtility` — 디렉토리 크기 측정, 포맷팅, 히스토리 I/O 정적 유틸리티

## [0.1.9] - 2026-02-24

### 변경
- Addressables Remote 복사 대상을 `{buildDir}/Remote/` → `{exeName}_Data/Remote/`로 변경
- Addressables 빌드 모드가 Skip이어도 Remote 폴더 복사 수행 (기존 빌드 콘텐츠 활용)

### 수정
- macOS `steamcmd.sh` 실행 권한 문제 수정 — 실행 전 `chmod +x` 자동 적용 (`EnsureExecutable`)
- `VerifyLogin()`과 `OpenTerminal()` 양쪽에 적용

## [0.1.8] - 2026-02-24

### 추가
- SteamCMD 사전 검증 — SDK 경로/실행파일 존재 확인, InfoBox 실시간 피드백
- SteamCMD 로그인 검증 (`TerminalControl.VerifyLogin`) — 빌드 전 로그인 상태 확인, 실패 시 즉시 중단
- SteamCMD 로그인 테스트 버튼 추가 (수동 검증용)
- Addressables 빌드 옵션 (`EAddressablesBuildMode`: Skip/ContentUpdate/CleanBuild)
- Addressables Remote 콘텐츠 자동 복사 (빌드 출력 폴더에 `Remote/` 포함)
- 인스펙터 경로 표시: `UnityBuildPath`, `AddressablesSrcPath`, `AddressablesDstPath`, `SteamCmdPath` (ReadOnly + 폴더 열기 버튼)
- `Editor/AutoBuild/` → `Module/Accelib.Editor.AutoBuild/`로 독립 모듈화

### 변경
- asmdef에서 NaughtyAttributes 제거, Addressables 참조 추가
- `AppConfig` 어트리뷰트를 NaughtyAttributes `[Header]`에서 Odin `[TitleGroup]`으로 마이그레이션
- 빌드 파이프라인 Phase 분리 (Phase 0~5)
- Discord 빌드 성공 알림에 Addressables 복사 결과 포함
- 인스펙터 레이아웃 재구성 (계정 및 앱 → 앱, Username → 빌드 옵션으로 이동)

### 문서
- README.md 신규 작성
- CHANGELOG.md 신규 작성
