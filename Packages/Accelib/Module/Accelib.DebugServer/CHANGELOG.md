# Changelog

이 문서는 Accelib.DebugServer 모듈의 주요 변경 내역을 기록한다.

## [0.8.0] - 2026-03-09

### 수정
- `DebugServerGUI` — 박스 배경이 간헐적으로 보이지 않던 버그 수정 (Texture2D 파괴 감지 + HideFlags 적용)
- `DebugServerGUI` — 폰트 색상이 실행마다 변동되던 버그 수정 (모든 GUIStyleState에 색상 명시적 설정)

## [0.7.0] - 2026-03-09

### 추가
- **SSE (Server-Sent Events) 실시간 이벤트 스트림 지원**
  - `DebugEvent` — 이벤트 데이터 구조체 (`EventType`, `DataJson`, `Timestamp`)
  - `DebugEventBus` — 스레드 안전 이벤트 버스 (ConcurrentQueue + 링 버퍼 256개)
    - `Publish()` — 모든 스레드에서 이벤트 발행
    - `PublishSafe()` — 서버 비활성 시 무시하는 정적 헬퍼
  - `SseClient` — SSE 클라이언트 연결 관리 (필터링, 자동 끊김 감지)
  - `DebugServerCore.EventBus` — 이벤트 버스 public 접근자
- 빌트인 엔드포인트:
  - `GET /api/events/recent` — 링 버퍼 조회 (폴링 폴백)
  - `GET /api/events/clients` — 연결된 SSE 클라이언트 수

### 변경
- `DebugServerCore` — SSE 클라이언트 수명 관리 추가 (수락, 브로드캐스트, 하트비트, 정리)
  - `ListenLoop()` — `GET /api/events/stream` SSE 요청 인터셉트 (`?filter=type1,type2` 필터링)
  - `Update()` — 클라이언트 수락, 이벤트 브로드캐스트, 15초 간격 하트비트
  - `StopServer()` — SSE 클라이언트 Dispose
  - 다중 연결 시 `warning` 이벤트 자동 전송

## [0.6.0] - 2026-03-09

### 변경
- `IDebugEndpointProvider` — `RoutePrefix`, `CategoryName` 프로퍼티 추가. Provider 레벨에서 라우트 prefix와 카테고리를 정의
- `DebugServerCore` — `RegisterEndpointsFrom()` 수정: Provider의 `RoutePrefix`를 라우트에 자동 결합, `CategoryName`을 기본 카테고리로 사용

### 추가
- `DebugEndpointProviderBase` — 추상 기반 클래스. Odin 인스펙터에 `RoutePrefix`/`CategoryName` 자동 표시

## [0.5.0] - 2026-03-09

### 수정
- `DebugServerGUI` — 우측 하단 앵커 수정: `GUI.matrix` 스케일링 제거, 직접 픽셀 계산 방식으로 전환
  - 스케일 변경 시 좌측/상단으로 확장되며 우측 하단 고정 유지
  - `fontSize`를 `scale`에 비례하여 동적 조정

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
