---
name: debug-server
description: >
  Unity Debug HTTP Server — interact with a running Unity game via HTTP.
  Use this skill when the user wants to query game state, control the game,
  test features, or automate gameplay via the debug server.
  The server is self-documenting: connect first, then discover available
  endpoints dynamically via /api/help.
  Korean triggers: "디버그 서버", "게임 상태 조회", "curl 테스트", "디버그 API",
  "게임 테스트 자동화", "HTTP 테스트", "엔드포인트 추가",
  "게임 디버깅 해줘", "게임 디버깅", "디버그 하자", "디버깅 하자",
  "테스트", "게임 테스트", "디버깅", "게임 확인"
  English triggers: "debug server", "game API", "curl test", "query game state",
  "game automation", "add endpoint", "http debug", "debug game", "test game",
  "game debugging", "game test"
---

# Unity Debug HTTP Server

The debug server runs inside Unity Play Mode and exposes game state via HTTP.
It is self-documenting — discover available endpoints at runtime instead of
relying on hardcoded lists.

## Workflow

Follow this sequence every time:

### 1. Connect & Discover

```bash
# Check server is alive (default port: 7860)
curl http://localhost:7860/api/ping

# Get all available endpoints as JSON
curl http://localhost:7860/api/help
```

The `/api/help` response lists every registered endpoint with method, route,
description, and category. This is the source of truth — always query it
rather than guessing endpoint names.

### 2. Match User Intent

Read the `/api/help` output and find the endpoint(s) that match what the
user wants to do. The `description` field explains what each endpoint does.

**If no matching endpoint exists:**
Do NOT guess or fabricate an endpoint. Instead, inform the user:
- Clearly state that the requested functionality does not have a debug endpoint yet.
- List the available endpoints that are closest to what they asked for (if any).
- Ask the user if they want to add a new endpoint for it (see "Adding New Endpoints" below).
- Example response: "현재 `/api/help`에 해당 기능의 엔드포인트가 없습니다. 새 엔드포인트를 추가할까요?"

### 3. Execute

```bash
# GET endpoints — read state
curl http://localhost:7860/api/{route}

# POST endpoints — perform actions
curl -X POST http://localhost:7860/api/{route}

# POST with JSON body
curl -X POST http://localhost:7860/api/{route} \
  -d '{"key": "value"}' -H "Content-Type: application/json"
```

### 4. Read Response

All endpoints return:
```json
{ "ok": true, "data": { ... } }       // success
{ "ok": false, "error": "message" }    // error
```

Check `ok` field first. On error, read `error` message and report to user.

## Troubleshooting

- **Connection refused** — Unity Play Mode must be active. Check Console for
  `[DebugServer] Started on port 7860` log.
- **No such endpoint** — Re-query `/api/help` to see current endpoints.
  Endpoints change as the codebase evolves.
- **Error response** — Read the `error` field. Common causes: invalid params,
  system not ready (scene not loaded), or handler exception (check Unity Console).
- **Release build** — Server is stripped via `#if UNITY_EDITOR || DEVELOPMENT_BUILD`.
  Only available in Editor and Development builds.

## Adding New Endpoints

When the user wants to add a new debug endpoint, read the existing endpoint
files in `Assets/Accelix/Scripts/GameSystem/Utility/Debug/` to understand
the current pattern, then follow the same convention:

1. Pick or create an `IDebugEndpointProvider` component file
2. Add a method with `[DebugEndpoint("METHOD", "/api/route", "description")]`
3. Use `ctx.Respond()` / `ctx.RespondError()` extension methods
4. Test: Play Mode → `curl /api/help` → verify new endpoint appears
