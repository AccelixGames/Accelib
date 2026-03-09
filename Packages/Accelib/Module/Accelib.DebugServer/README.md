# Accelib.DebugServer

Unity 에디터/개발 빌드에서 사용할 수 있는 HTTP 디버그 서버 프레임워크.
`DebugServerCore`가 하위 `IDebugEndpointProvider` 컴포넌트를 자동 탐색하여 `[DebugEndpoint]` 메서드를 라우트로 등록한다.

## 디렉토리 구조

```
Accelib.DebugServer/
├── Accelib.DebugServer.asmdef        # 어셈블리 정의
├── README.md                          # 이 문서
├── CHANGELOG.md                       # 변경 이력
└── Runtime/
    ├── Core/                          # 서버 인프라
    │   ├── DebugServerCore.cs         # sealed MonoSingleton — HTTP, 라우팅, SSE, 내장 엔드포인트
    │   └── DebugServerGUI.cs          # IMGUI 상태 오버레이 (internal)
    ├── Endpoint/                      # 엔드포인트 정의 API
    │   ├── DebugEndpointAttribute.cs  # [DebugEndpoint] 어트리뷰트 정의
    │   ├── DebugEndpointInfo.cs       # 엔드포인트 메타데이터 (문서 생성용)
    │   └── IDebugEndpointProvider.cs  # 마커 인터페이스 (엔드포인트 컴포넌트 탐색용)
    ├── Http/                          # HTTP 요청/응답
    │   ├── RequestContext.cs          # HTTP 요청/응답 캡슐화
    │   └── RequestContextExtensions.cs # 확장 메서드 (응답, 파싱, 유틸리티)
    └── Sse/                           # SSE 실시간 이벤트 스트림
        ├── DebugEvent.cs              # 이벤트 데이터 구조체
        ├── DebugEventBus.cs           # 스레드 안전 이벤트 버스 + 링 버퍼
        └── SseClient.cs              # SSE 클라이언트 연결 관리
```

## 아키텍처

```
[GameObject: DebugServer]
├── DebugServerCore              ← HTTP 서버 코어 (Accelib)
├── [Child: Cafe]
│   └── CafeEndpoints            ← 게임별 엔드포인트 (프로젝트)
└── [Child: Player]
    └── PlayerEndpoints           ← 게임별 엔드포인트 (프로젝트)
```

- `DebugServerCore`가 `GetComponentsInChildren<IDebugEndpointProvider>()`로 하위 컴포넌트 탐색
- 각 컴포넌트의 `[DebugEndpoint]` 메서드를 리플렉션으로 자동 등록
- 엔드포인트는 서버를 참조하지 않고 `RequestContext` 확장 메서드로 응답

## 주요 클래스

### DebugServerCore

`MonoSingleton<DebugServerCore>` 기반 HTTP 서버 코어. 엔드포인트 탐색, 라우팅, 스레드 안전을 담당한다.

- `StartServer()` — 서버 시작 (기본 포트: 7860)
- `StopServer()` — 서버 중지
- `GenerateMarkdownDoc()` — Markdown API 문서 생성
- `GetEndpointInfos()` — 등록된 엔드포인트 메타데이터 목록

**내장 엔드포인트:**
- `GET /api/ping` — 서버 상태 확인
- `GET /api/help` — 전체 API 목록 (JSON)
- `GET /api/help/markdown` — API 문서 (Markdown)

### IDebugEndpointProvider

마커 인터페이스. 이 인터페이스를 구현한 MonoBehaviour를 `DebugServerCore` 하위에 배치하면 자동으로 엔드포인트가 등록된다.

### RequestContextExtensions

엔드포인트 구현에 필요한 확장 메서드를 제공한다.

- `ctx.Respond(json)` — JSON 성공 응답
- `ctx.RespondOk()` — 데이터 없는 성공 응답
- `ctx.RespondError(msg, code)` — 에러 응답
- `ctx.RespondDto<T>(dto)` — 구조체 JSON 변환 응답
- `ctx.ParseBodyInt(field, default)` — POST body int 파싱
- `ctx.ParseBodyFloat(field, default)` — POST body float 파싱
- `ctx.ParseBodyBool(field, default)` — POST body bool 파싱
- `EscapeJson(string)` — JSON 이스케이프 유틸리티

### DebugEndpointAttribute

메서드에 붙여 HTTP 엔드포인트를 선언한다. 서버 시작 시 리플렉션으로 자동 탐색된다.

- `HttpMethod` — "GET" 또는 "POST"
- `Route` — URL 패턴 (예: `/api/users/{id}`)
- `Description` — 엔드포인트 설명
- `Category` — 문서 그룹핑용 카테고리 (선택)

### RequestContext

HTTP 요청과 응답을 캡슐화한다. 엔드포인트 핸들러 메서드의 유일한 파라미터.

- `HttpMethod` — 요청 메서드
- `Path` — 요청 경로
- `Body` — 요청 본문 (POST)
- `GetPathParam(string)` — 경로 파라미터 조회
- `GetPathParamInt(string)` — 경로 파라미터를 int로 조회
- `SendResponse(json, statusCode)` — JSON 응답 직접 전송

## 사용 예시

```csharp
// 1. 엔드포인트 컴포넌트 정의
public sealed class MyEndpoints : MonoBehaviour, IDebugEndpointProvider
{
    [DebugEndpoint("GET", "/api/status", "게임 상태 조회", Category = "상태")]
    private void GetStatus(RequestContext ctx)
    {
        ctx.Respond("{\"health\":100,\"score\":42}");
    }

    [DebugEndpoint("POST", "/api/reset", "게임 리셋", Category = "제어")]
    private void ResetGame(RequestContext ctx)
    {
        // 게임 리셋 로직
        ctx.RespondOk();
    }

    [DebugEndpoint("GET", "/api/player/{id}", "플레이어 조회", Category = "플레이어")]
    private void GetPlayer(RequestContext ctx)
    {
        var id = ctx.GetPathParamInt("id");
        ctx.Respond($"{{\"id\":{id},\"name\":\"Player{id}\"}}");
    }
}

// 2. 씬 구성
// [DebugServer GO] + DebugServerCore
//   └── [Child GO] + MyEndpoints

// 3. curl로 호출
// curl http://localhost:7860/api/status
// curl -X POST http://localhost:7860/api/reset
// curl http://localhost:7860/api/player/3
// curl http://localhost:7860/api/help
```

## SSE (Server-Sent Events) 실시간 이벤트 스트림

게임 시스템에서 발생하는 이벤트를 실시간으로 클라이언트에 푸시한다.

### 이벤트 발행

```csharp
// 메인 스레드에서 (대부분의 게임 코드)
DebugEventBus.PublishSafe("player.jumped", "{\"position\":{\"x\":1,\"y\":0,\"z\":3}}");

// 또는 EventBus 직접 접근
DebugServerCore.Instance.EventBus.Publish("custom.event", "{\"data\":42}", Time.realtimeSinceStartup);
```

### 클라이언트 연결

```bash
# 전체 이벤트 수신
curl -N http://localhost:7860/api/events/stream

# 특정 이벤트만 필터링
curl -N "http://localhost:7860/api/events/stream?filter=player.jumped,player.landed"

# 최근 이벤트 버퍼 조회 (폴링 폴백)
curl http://localhost:7860/api/events/recent

# 연결된 SSE 클라이언트 수 확인
curl http://localhost:7860/api/events/clients
```

### SSE 출력 형식

```
event: player.jumped
data: {"position":{"x":1.2,"y":0.0,"z":3.4}}

: heartbeat

event: warning
data: {"message":"다른 클라이언트가 이미 연결되어 있습니다","activeClients":1}
```

### 주요 특성

- **스레드 안전:** `DebugEventBus.Publish()`는 모든 스레드에서 호출 가능
- **링 버퍼:** 최근 256개 이벤트를 저장. 늦게 연결한 클라이언트에 catch-up 전송
- **다중 연결 경고:** 이미 연결된 클라이언트가 있으면 `warning` 이벤트 자동 전송
- **하트비트:** 15초 간격으로 `: heartbeat` SSE 코멘트 전송 (연결 유지 확인)
- **자동 정리:** 끊긴 클라이언트는 다음 전송 시 자동 감지 및 제거

## 상태 오버레이

`DebugServerGUI` 컴포넌트를 인스펙터에서 직접 추가하면 화면 우측 하단에 서버 상태를 IMGUI로 표시한다.

**인스펙터 설정 (`DebugServerGUI`):**
- `scale` — 오버레이 배율 (0.25~2.0, 기본값: 0.5)
- `alpha` — 오버레이 투명도 (0~1, 기본값: 0.7)

**표시 내용:**
- **상태:** `Running (N Endpoint)` (초록) / `Stopped` (빨강)
- **포트:** 현재 수신 포트 번호
- **curl 명령어:** `curl localhost:{port}/api/help` 참조용

## 스레드 안전

- `HttpListener`는 백그라운드 스레드에서 요청을 수신한다.
- 요청은 `ConcurrentQueue`에 저장되고, `Update()`에서 메인 스레드로 전달된다.
- 모든 엔드포인트 핸들러는 **메인 스레드**에서 실행되므로 Unity API를 안전하게 사용할 수 있다.

## 조건부 컴파일

전체 모듈이 `#if UNITY_EDITOR || DEVELOPMENT_BUILD`로 감싸져 있다.
릴리스 빌드에는 포함되지 않는다.

## 의존성

| 패키지 | 용도 |
|--------|------|
| Accelib.Core | MonoSingleton\<T\> 베이스 |
| Odin Inspector | 인스펙터 레이아웃 (조건부) |

## 네임스페이스

```csharp
using Accelib.DebugServer;
```
