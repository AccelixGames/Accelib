# Changelog

이 문서는 Accelib.Editor.AutoBuild 모듈의 주요 변경 내역을 기록한다.

## [0.1.9] - 2026-02-24

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
