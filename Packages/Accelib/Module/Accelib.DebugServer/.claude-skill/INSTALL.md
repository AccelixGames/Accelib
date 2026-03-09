# debug-server Skill — Installation

Unity Debug HTTP Server skill for Claude Code.

## Installation

Create a junction from your project's `.claude/skills/` to this folder.

**PowerShell (권장):**
```powershell
New-Item -ItemType Junction -Path "C:\WorkSpace\UnityProjects\MaidCafeSimulator\ProjectMaid\.claude\skills\debug-server" -Target "C:\WorkSpace\github.com\AccelixGames\Accelib\Packages\Accelib\Module\Accelib.DebugServer\.claude-skill"
```

**CMD (관리자 권한 불필요):**
```cmd
mklink /J "C:\WorkSpace\UnityProjects\MaidCafeSimulator\ProjectMaid\.claude\skills\debug-server" "C:\WorkSpace\github.com\AccelixGames\Accelib\Packages\Accelib\Module\Accelib.DebugServer\.claude-skill"
```

## Verification

Start a new Claude Code session. The skill auto-registers if `.claude/skills/debug-server/SKILL.md` is found.

Test with any of these prompts:

- "디버그 서버 엔드포인트 추가해줘"
- "curl로 게임 상태 조회하고 싶어"
- "debug server에 새 API 만들어줘"

## Uninstallation

```powershell
Remove-Item "C:\WorkSpace\UnityProjects\MaidCafeSimulator\ProjectMaid\.claude\skills\debug-server"
```
