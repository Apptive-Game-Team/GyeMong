---
name: artel-cheats
description: >-
  Add cheats to a Unity game that uses the ARTEL SDK, so the ARTEL QA agent can reach a test
  precondition in one call instead of playing up to it: go to a scene, unlock a stage, defeat the
  enemies on screen, see the ending, heal. Use when the user asks to "add a cheat", "치트키 만들어줘",
  "씬 이동 치트", "적 다 죽이는 치트", "엔딩 보는 치트", "[ArtelCheat]", or wants the QA agent to stop
  failing with SETUP-FAILED. For routines that keep running every frame, use artel-behaviors.
---

# ARTEL cheats

A cheat is a C# method the QA agent calls once through the SDK to change game state. The agent uses
cheats to reach a test case's **precondition** fast. It must never use one to produce the value a
step verifies, and the QA author can forbid a cheat per test case. So each cheat should **set up a
state**, never fake a result.

Files next to this one:

- [`references/choosing-cheats.md`](references/choosing-cheats.md) — which cheats to make, and which not to
- [`references/reference.md`](references/reference.md) — attribute rules, the wire action, every failure message
- [`references/compile-guard.md`](references/compile-guard.md) — the `#if ARTEL_GAME_FUNCTIONS` wrap, who sets the define, the cases it cannot fix
- [`references/wordventure-example.cs`](references/wordventure-example.cs) — a real implementation (WordVenture)

## Before you start

1. **The SDK must contain the feature.** Cheats ship in `kr.artel.sdk` from the ARTEL-1052 change
   (project-artel/artel-sdk#142). Until that is merged into `develop`, install the package from that
   branch:

   ```text
   https://github.com/project-artel/artel-sdk.git?path=/Packages/kr.artel.sdk#feat/sdk-artelcheat-artelbehavior-등록과-invoke_cheat-start_behavior-action-game_function_catalog-frame-추가-ARTEL-1052
   ```

   Check: `Packages/kr.artel.sdk/Runtime/GameFunctions/ArtelCheatAttribute.cs` and
   `Packages/kr.artel.sdk/Editor/GameFunctions/ArtelDefineSync.cs` exist in the package. If not,
   stop and tell the user: with the wrap below the game still compiles, but no cheat is registered.
   The artel-unity-sdk skill covers installing and building.
2. **Read the game first.** List the scenes in `ProjectSettings/EditorBuildSettings.asset`, then find
   every `SceneManager.LoadScene` call, the save code (`PlayerPrefs` keys), the singletons
   (`static ... Instance`), and the methods the UI buttons call. A good cheat calls those methods.
3. **Decide which cheats to make** with `references/choosing-cheats.md`. Ask the user which test
   cases are slow to set up; that list is better than guessing.
4. Put the cheats in one file, for example `Assets/Scripts/Artel/<Game>GameFunctions.cs`, in an
   assembly that references `Artel.Runtime` (`Assembly-CSharp` does by default), and **wrap the
   whole file in `#if ARTEL_GAME_FUNCTIONS`** (next section).

## Always wrap the file in `#if ARTEL_GAME_FUNCTIONS`

Every file with `[ArtelCheat]`, `[ArtelBehavior]`, `[CheatRange]` or `ArtelBehaviorContext` starts
with `#if ARTEL_GAME_FUNCTIONS` above its `using` lines and ends with `#endif`. Without it, a
teammate or CI machine that has no SDK, or an SDK without cheats, gets dozens of `error CS0246` and
the game stops building.

That wrap is the only step on the game side. The SDK sets the define itself from the version that
ships cheats: it adds `ARTEL_GAME_FUNCTIONS` on every editor load and removes all `ARTEL_` defines
when the package is removed or changes version through the Package Manager. An older SDK or no SDK
never defines it, so the file drops out of the compile.

- Do not add an Editor script, asmdef or `csc.rsp` to the game for this, and do not type the define
  into Player Settings.
- `ARTEL_` is reserved for the SDK. Never define a game symbol with that prefix; the SDK removes
  unknown `ARTEL_` defines.
- Nothing outside the wrapped file may reference its class.
- If the SDK was removed by editing `manifest.json` with the editor closed, or a committed
  `ProjectSettings.asset` brings the define to a machine whose SDK has no cheats, the build breaks
  and the SDK cannot fix it, because Unity runs no package code while scripts fail to compile.
  Tell the user to delete `ARTEL_GAME_FUNCTIONS` once in Player Settings > Scripting Define
  Symbols, or to update the SDK.
- A game whose cheat file sits in its own asmdef can instead use asmdef Version Defines on
  `kr.artel.sdk` plus Define Constraints.

Details and measurements are in [`references/compile-guard.md`](references/compile-guard.md).

## Write a cheat

```csharp
#if ARTEL_GAME_FUNCTIONS
using Artel;

public static class MyGameFunctions
{
    [ArtelCheat("Sets the unlocked stage (0 village .. 4 boss) and saves it. Takes effect on the map screen.")]
    static string UnlockStage([CheatRange(0, 4)] int stage)
    {
        MapMove.StagePosition = stage;          // the field the game itself reads
        SaveLoadController.Instance?.SavePlayData();
        PlayerPrefs.Save();
        return $"stage_position={MapMove.StagePosition}";   // the agent reads this message
    }
}
#endif
```

- **The description is what the agent reads** to decide when to call it. Say what state it leaves
  and where it takes effect.
- **Use an enum for a closed set** (scenes, items). The catalog publishes the values, so the agent
  picks one instead of inventing a name.
- **Put a range on every number** with `[CheatRange(min, max)]`.
- **Return what changed** (`killed=3`, `stage_position=4`). The agent compares it with the screen.
- **Write `when:` for every cheat that only works somewhere**, returning `null` when available and
  the reason otherwise:

  ```csharp
  static string HasLiveEnemy() =>
      TurnBattleSystem.Instance == null ? "not in a battle scene" : null;

  [ArtelCheat("Defeats every slime alive in the current wave with Enemy.Kill().", when: nameof(HasLiveEnemy))]
  static string KillAllSlimes() { ... }
  ```

  The SDK checks it once a second and shows the agent `Cheats now` / `Cheats not now (reason)`, and
  refuses a call with `unavailable: <reason>`. Without it the agent tries the cheat on the title
  screen and your code throws.

The compile-time rules (types, names, instance methods, release builds) are in
`references/reference.md`; a mistake is a compile error from the weaver, not a runtime surprise.

## Three lessons from WordVenture

1. **Recreate what the skipped path would have set.** Loading the battle scene from the title
   skips the map, which is where the game creates `StageDataSingleton` and sets `stagePosition`.
   The `go_to_scene` cheat creates the singleton when it is missing. For every scene a cheat jumps
   to, ask what the previous scene normally leaves behind and set it.
2. **Make the save agree with the scene.** `show_ending` sets `MapMove.StagePosition = 5` (all
   stages cleared) and saves before loading `EndingScene`, because that is the state the real ending
   leaves.
3. **Use the game's own methods.** `Enemy.Kill()` runs the death animation, wave bookkeeping and
   victory check, so killing the last slime still leads to `GameClearScene`. `Destroy(enemy)` would
   leave the wave waiting forever.

## Verify

1. Compile in the editor and check that Player Settings > Scripting Define Symbols now contains
   `ARTEL_GAME_FUNCTIONS` (the SDK adds it). If it is missing, the wrapped file compiled to nothing
   and the catalog will be empty: the SDK is too old. Then make a **development build**; release
   builds contain no cheats.
2. Check the registration is in the build. Names are UTF-16, so use `-el`:

   ```bash
   strings -a <Build>/<Game>_Data/Managed/Assembly-CSharp.dll | grep -c __artel_call_
   strings -a -el <Build>/<Game>_Data/Managed/Assembly-CSharp.dll | grep -E "go_to_scene|kill_all"
   ```

3. With the game connected, check the catalog lists every cheat with its parameters:

   ```bash
   curl -s "$API/api/projects/<p>/game-builds/<b>/game-functions" -H "Authorization: Bearer $TOKEN"
   ```

4. Call each cheat once where it should work and once where it must be refused (wrong scene, bad
   enum, out-of-range number). On a local orchestration server the internal port sends an action
   straight to a connected game:

   ```bash
   curl -s -X POST http://127.0.0.1:8091/internal/action/<instanceId> -H 'Content-Type: application/json' \
     -d '{"type":"ACTION","id":9001,"actions":[{"id":1,"jsonrpc":"2.0","method":"invoke_cheat",
          "params":[{"name":"go_to_scene","args":{"scene":"Battle","stage":0}}]}]}'
   grep -o '"requestId":9001,.*' <orchestration log>
   ```

   Elsewhere, run a short QA scenario whose precondition needs the cheat and read the
   `invoke_cheat` rows in the run log.

WordVenture results (2026-10-16): `go_to_scene Battle` → `loading=TurnBattleScene stage=0`;
`kill_all_slimes` → `killed=1` per wave; `show_ending` → `loading=EndingScene stage_position=5`;
`kill_all_slimes` on the ending screen → `unavailable: not in a battle scene`.

## References

- [Package README](https://github.com/project-artel/artel-sdk/blob/develop/Packages/kr.artel.sdk/README.md), section "Cheats and behaviors for the QA agent"
- [Protocol](https://github.com/project-artel/artel-sdk/blob/develop/docs/protocol.md) and [ADR 0007](https://github.com/project-artel/artel-sdk/blob/develop/docs/adr/0007-game-registered-cheats-and-behaviors.md) (in `develop` once artel-sdk#142 merges)
- [Design and wire contract](https://github.com/project-artel/artel/tree/main/.plan/general) — `2026-10-13-game-registered-cheats-for-the-qa-agent.md`, `2026-10-14-game-functions-wire-contract.md` (in `main` once artel#20 merges)
