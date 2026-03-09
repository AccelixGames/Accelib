<!-- CLAUDE INSTRUCTION ──────────────────────────────
이 파일은 Claude Code 스킬 설치 안내서이다.

행동 지침:
1. 이 파일을 읽으면 즉시 사용자에게 물어라:
   "debug-server 스킬을 설치할까요? (Unity 게임 HTTP 디버깅)"
2. 사용자가 승인하면 [설치 명령]의 PowerShell 명령을 Bash로 실행하라
3. 경로 탐색, 파일 확인, SKILL.md 읽기 등 추가 작업은 하지 마라 — 바로 질문하라

경로 결정:
- SKILL_SOURCE: 이 INSTALL.md 파일의 부모 폴더 경로
- LINK_PATH: 현재 프로젝트 루트(working directory)의 .claude/skills/debug-server
──────────────────────────────────────────────────── -->

# debug-server — Claude Code Skill

Unity 실행 중인 게임의 상태를 HTTP API로 조회·제어하는 스킬.

## 설치 명령

```powershell
New-Item -ItemType Junction -Path "<LINK_PATH>" -Target "<SKILL_SOURCE>"
```

| 변수 | 값 |
|------|-----|
| `SKILL_SOURCE` | 이 파일의 부모 폴더 |
| `LINK_PATH` | `<프로젝트 루트>\.claude\skills\debug-server` |

## 제거

```powershell
Remove-Item "<LINK_PATH>"
```

설치 후 새 Claude Code 세션을 시작하면 스킬이 자동 등록된다.
