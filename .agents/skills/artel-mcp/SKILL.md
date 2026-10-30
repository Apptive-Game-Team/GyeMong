---
name: artel-mcp
description: >-
  Set up the ARTEL MCP server (artel-mcp, local mode) so Claude Code, Codex or another MCP client can
  observe and drive a Unity game running the ARTEL SDK directly, without the hosted servers. Use
  when the user asks to "connect ARTEL to Claude Code", "MCP 셋팅", "artel mcp", "drive my game from
  the agent", or wants a local QA loop with no ARTEL account.
---

# ARTEL MCP (local mode)

`artel-mcp` is one local process that plays both roles the hosted ARTEL stack plays: it answers the
SDK's REST and WebSocket protocol, and it exposes the game to an MCP client over stdio. Nothing
leaves the machine except what the MCP client sends to its own model.

```
Unity game + ARTEL SDK  <-- ws://127.0.0.1:4320/ws/sdk -->  artel-mcp  <-- stdio MCP -->  Claude Code / Codex
                                                              └─ .artel/artel.db + store/
```

**Status: prototype** (project-artel/artel-mcp-poc). Verified end to end on the WordVenture sample.
For production QA use the hosted stack and the artel-cli skill. Say this to the user before setting
it up.

## 1. Install

Requires `git`, Python and [`uv`](https://docs.astral.sh/uv/).

```bash
git clone https://github.com/project-artel/artel-mcp-poc.git ~/artel-mcp
cd ~/artel-mcp && uv sync
```

## 2. Register it with the MCP client

Claude Code (run in the game repository, or add `-s user` for every project):

```bash
claude mcp add artel -- "$HOME/artel-mcp/.venv/bin/artel-mcp"
claude mcp list          # artel: connected
```

Codex (`~/.codex/config.toml`):

```toml
[mcp_servers.artel]
command = "/home/<you>/artel-mcp/.venv/bin/artel-mcp"
```

Any other MCP client: a stdio server whose command is `<checkout>/.venv/bin/artel-mcp`.

`.artel/` (SQLite database and captures) is created where the server runs; set `ARTEL_HOME` to keep
it elsewhere. Commit it to share the content map and run history through the game repository, or
ignore it.

## 3. Point the game at it

Build a development build with the SDK (artel-unity-sdk skill), then launch it against the local
server. Any token works and there is one project, `local`:

```bash
ARTEL_SDK_TOKEN=local ./Build/Game.exe -artel-server 127.0.0.1:4320 -artel-secure false -artel-project local
```

A scene that carries its own `ArtelManager` keeps its serialized `Server` and ignores
`-artel-server`; remove that manager from the scene for local mode. The MCP tool `game_status()`
prints the exact launch line.

## 4. Use it

| Group | Tools |
|---|---|
| status | `game_status`, `scan_game`, `start_readings`, `stop_readings` |
| observe | `observe(whole)`, `inspect(selector)`, `capture_screen(target_id)` |
| act | `click_button`, `click_at`, `move_pointer`, `press_key`, `enter_text`, `drag_pointer`, `pause_game`, `resume_game`, `reset_game` |
| content map | `list_scenes`, `describe_scene`, `find_capability` |
| run | `start_run`, `report_step`, `finish_run`, `run_summary`, `list_runs` |

A typical first session: `game_status` → `scan_game` → `start_readings` → `observe` → act →
`observe` again → `report_step`.

Action tools return the game's answer plus a reading captured after the action's frame, so the
agent judges the screen the action produced, not the one before it.

## Limits of the prototype

- No cheat or behavior tools yet; game functions (artel-cheats and artel-behaviors skills) need the hosted stack.
- No test case generation, scenario feasibility check, dashboard, or knowledge search.
- macOS scripts in the repository build the sample; on Windows build with the Unity command line.

## References

- [artel-mcp-poc README](https://github.com/project-artel/artel-mcp-poc/blob/main/README.md) — setup, tools, layout, what is missing
- [SDK protocol](https://github.com/project-artel/artel-sdk/blob/develop/docs/protocol.md) — the socket protocol artel-mcp answers
