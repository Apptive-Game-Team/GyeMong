---
name: artel-unity-sdk
description: >-
  Install and connect the ARTEL Unity SDK (kr.artel.sdk) in a Unity game so the ARTEL QA agent can
  observe and drive it. Use when the user asks to "install the ARTEL SDK", "SDK 셋팅", "connect my
  Unity game to ARTEL", "the game does not register", "make a development build for ARTEL", or
  before adding cheats and behaviors (see the artel-cheats and artel-behaviors skills).
---

# ARTEL Unity SDK setup

The SDK is a Unity package. Installing it is the whole integration: in the editor and in
development builds it spawns its own `ArtelManager` after the first scene loads, reads the scenes,
and lets the QA agent click, press keys and capture the screen. Release builds compile none of it
(`#if UNITY_EDITOR || DEVELOPMENT_BUILD`).

## 1. Install the package

Unity **Window → Package Manager → + → Add package from git URL**:

```text
https://github.com/project-artel/artel-sdk.git?path=/Packages/kr.artel.sdk
```

That follows `develop`. Pin a branch, tag or commit with `#<revision>`:

```text
https://github.com/project-artel/artel-sdk.git?path=/Packages/kr.artel.sdk#develop
```

Or edit `Packages/manifest.json` directly:

```json
{ "dependencies": { "kr.artel.sdk": "https://github.com/project-artel/artel-sdk.git?path=/Packages/kr.artel.sdk#develop" } }
```

For a checkout of the SDK next to the game, use a file path instead:
`"kr.artel.sdk": "file:../../../Packages/kr.artel.sdk"`.

Verify: the Console shows no compile errors and `Packages/kr.artel.sdk` appears in the Project
window.

## 2. Sign in and pick a project (editor)

Press Play. The ARTEL overlay appears; its login button opens the console in the browser
(`<console>/sdk-login`). Sign in there, come back, and pick the project once. Later launches
register and connect with no interaction. Tokens live in the OS secret store (DPAPI on Windows,
Keychain on macOS); never write them to a file.

To point the SDK at a server other than the default one, put an `ArtelManager` on a scene object
and set `Server` (`secure`, `host`, `port`) and `frontendOrigin`. A manager in the scene wins over
the spawned one and over launch arguments.

## 3. Build for QA: a development build

**The SDK only attaches in a development build.** A normal build launches, never registers, and the
log has no `[Artel]` line.

```csharp
// Assets/Editor/ArtelBuild.cs
using UnityEditor;
public static class ArtelBuild
{
    public static void BuildWindows64()
    {
        var scenes = System.Array.ConvertAll(EditorBuildSettings.scenes, s => s.path);
        BuildPipeline.BuildPlayer(scenes, "Build/Game.exe", BuildTarget.StandaloneWindows64,
            BuildOptions.Development);
    }
}
```

```bash
"<Unity>/Editor/Unity.exe" -batchmode -quit -nographics -projectPath "<project>" \
  -executeMethod ArtelBuild.BuildWindows64 -logFile "<project>/Build/build.log"
```

Match the editor to `ProjectSettings/ProjectVersion.txt`. Check the build carries the SDK:

```bash
strings -a Build/Game_Data/Managed/Artel.Runtime.dll | grep -c InstallLaunchSession   # must be > 0
```

## 4. Launch and register

The easiest way is the ARTEL CLI (see the artel-cli skill), which launches the build already signed
in and waits for it to register:

```bash
artel game start --build ./Build/Game.exe --project <project id>
```

Launching by hand takes these arguments; the SDK token goes in the environment, never on the
command line (other users can read process arguments):

| Argument | Value |
|---|---|
| `-artel-server` | `host:port` of the orchestration server |
| `-artel-secure` | `true` / `false` (`https`/`wss` or not) |
| `-artel-frontend` | console origin for the login page |
| `-artel-project` | project id |
| `-artel-logout` | clear the stored session and quit |
| `-artel-window-label` | label drawn on the overlay |

```bash
ARTEL_SDK_TOKEN=<sdk token> ./Build/Game.exe -artel-server 127.0.0.1:8080 -artel-secure false -artel-project 1
```

Registered means: `artel game list --project <id>` shows the instance as connected.

## Things that break silently

| Symptom | Cause | Fix |
|---|---|---|
| `game_registration_timeout`, no `[Artel]` in the log | not a development build | rebuild with `BuildOptions.Development` |
| Two copies of the game overwrite each other's save and share one instance | same `productName` (PlayerPrefs and the SDK id are keyed by it) | give each copy its own `productName` |
| Socket closes with `4001` | token or instance refused | press the overlay's connect button to register again |
| Second launch of the same build is refused (`이미 연결된 게임 인스턴스`) | the previous process still holds the socket | kill the old game by window title before relaunching |
| The game stops when the window loses focus | not the SDK: it already sets `Application.runInBackground` while connected | check that the game is connected |

## Next

- Add cheats and behaviors so the QA agent can reach states quickly: artel-cheats skill; keep giving per-frame input: artel-behaviors skill.
- Run QA from the terminal: artel-cli skill.

## References

Read these when a step here is not enough; they are the source this skill was written from.

- [SDK repository README](https://github.com/project-artel/artel-sdk/blob/develop/README.md) — install URL, launch arguments
- [Package README](https://github.com/project-artel/artel-sdk/blob/develop/Packages/kr.artel.sdk/README.md) — runtime connection, login flow, input, scanning, reset
- [Protocol](https://github.com/project-artel/artel-sdk/blob/develop/docs/protocol.md) — every action and frame the SDK speaks
- [Architecture](https://github.com/project-artel/artel-sdk/blob/develop/docs/architecture.md) and [ADRs](https://github.com/project-artel/artel-sdk/tree/develop/docs/adr)
