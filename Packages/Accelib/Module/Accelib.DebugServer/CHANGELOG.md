# Changelog

이 문서는 Accelib.DebugServer 모듈의 주요 변경 내역을 기록한다.

## [0.4.0] - 2026-03-09

### 변경
- `Runtime/` 디렉토리를 서브폴더로 정리: `Core/`, `Endpoint/`, `Http/`
  - 네임스페이스는 `Accelib.DebugServer` 플랫 유지 (코드 변경 없음)

## [0.3.0] - 2026-03-09

### 추가
- `DebugServerGUI` — IMGUI 상태 오버레이 (서버 상태/포트/curl 명령어를 화면 우측 하단에 표시)
  - 인스펙터에서 직접 추가하여 사용 (자동 생성하지 않음)
  - `scale` — 오버레이 배율 (0.25~2.0, 기본값: 0.5)
  - `alpha` — 오버레이 투명도 (0~1, 기본값: 0.7)
- `DebugServerCore` — `IsRunning`, `Port`, `RegisteredEndpointCount` public 프로퍼티 추가

### 제거
- `DebugServerCore` — `showOverlay`, `overlayScale`, `overlayAlpha` 필드 제거 (오버레이 설정은 `DebugServerGUI` 자체 필드로 이동)

## [0.2.0] - 2026-03-09

### 변경
- **[Breaking]** `DebugServerBase<T>` 추상 제네릭 싱글톤 → `DebugServerCore` 구체 sealed 싱글톤으로 전환
- **[Breaking]** 상속 기반 → 컴포지션 기반 아키텍처 전환. 엔드포인트는 하위 `IDebugEndpointProvider` 컴포넌트에서 정의
- `Respond()`, `RespondOk()`, `RespondError()` 등 응답 헬퍼를 `RequestContextExtensions` 확장 메서드로 이동
- `RequestContext.SendResponse()` 접근 제한자 `internal` → `public` 변경

### 추가
- `IDebugEndpointProvider` 마커 인터페이스 — 엔드포인트 컴포넌트 자동 탐색용
- `RequestContextExtensions` 정적 클래스 — 응답 전송, POST body 파싱 확장 메서드
  - `ctx.Respond()`, `ctx.RespondOk()`, `ctx.RespondError()`
  - `ctx.RespondDto<T>()`, `ctx.RespondArray<T>()`
  - `ctx.ParseBodyInt()`, `ctx.ParseBodyFloat()`, `ctx.ParseBodyBool()`
  - `EscapeJson()` 정적 유틸리티

### 제거
- `DebugServerBase.cs` 삭제 (`DebugServerCore.cs`로 대체)

## [0.1.0] - 2026-03-09

### 추가
- `DebugServerBase<T>` 추상 MonoSingleton — HttpListener 기반 HTTP 디버그 서버 프레임워크
- `DebugEndpointAttribute` — 메서드에 붙여 HTTP 엔드포인트를 자동 등록하는 어트리뷰트
- `DebugEndpointInfo` — 엔드포인트 메타데이터 구조체 (API 문서 자동 생성용)
- `RequestContext` — HTTP 요청/응답 캡슐화 (경로 파라미터, POST body 파싱 포함)
- 내장 엔드포인트: `/api/ping`, `/api/help` (JSON), `/api/help/markdown` (Markdown)
- 스레드 안전 요청 처리 (ConcurrentQueue → Update 메인 스레드)
- CORS preflight (OPTIONS) 자동 응답
- 조건부 컴파일 (`UNITY_EDITOR || DEVELOPMENT_BUILD`)
