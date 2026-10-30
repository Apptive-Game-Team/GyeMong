---
name: artel-behaviors
description: >-
  Add behaviors to a Unity game that uses the ARTEL SDK: routines that keep running inside the game
  every frame (keep attacking, keep casting, keep passing turns, flee) so the ARTEL QA agent can
  start one, keep observing while it plays, and stop it. Use when the user asks for "behavior",
  "기능 추가", "자동 공격", "auto attack for the QA agent", "실시간 게임에서 계속 공격", "[ArtelBehavior]",
  or a real-time game where the agent falls behind between turns. For one-shot state changes, use
  artel-cheats.
---

# ARTEL behaviors

A behavior is an `IEnumerator` method that the SDK runs one step per frame inside the game until it
ends or is stopped. The QA agent starts it with one tool call and keeps thinking and observing while
it plays. It exists because game time keeps running while the agent thinks: in a real-time fight the
character stands still between the agent's turns unless something gives input every frame.

A behavior plays by the game's rules (it calls the same methods the player's input calls), while a
cheat changes state directly. The agent must not use a behavior for a step that verifies the
controls themselves, because a behavior skips the input layer.

Files next to this one:

- [`references/choosing-behaviors.md`](references/choosing-behaviors.md) — which behaviors to make, and which not to
- [`references/lifecycle.md`](references/lifecycle.md) — how a behavior ends, the wire actions, every failure message
- [`references/wordventure-example.cs`](references/wordventure-example.cs) — `auto_cast_spells` and `pass_turns` in WordVenture
- [`../artel-cheats/references/compile-guard.md`](../artel-cheats/references/compile-guard.md) — the `#if ARTEL_GAME_FUNCTIONS` wrap every behavior file needs

## Before you start

1. **The SDK must contain the feature** (project-artel/artel-sdk#142). Until it is merged into
   `develop`, install from its branch; the artel-cheats skill has the exact URL. Check that
   `Packages/kr.artel.sdk/Runtime/GameFunctions/ArtelBehaviorAttribute.cs` exists.
2. **Find the per-frame loop the player drives**: the attack, cast, move or end-turn methods the UI
   calls, the input lock the game uses during animations, and how the game signals that a fight or
   level ended.
3. **Decide which behaviors to make** with `references/choosing-behaviors.md`.
4. **Wrap the whole file in `#if ARTEL_GAME_FUNCTIONS` ... `#endif`**, `using` lines included, the
   same way as cheats. The SDK defines the symbol from the version that ships behaviors and removes
   it when the package is removed, so a project without that SDK still compiles. This is the only
   game-side step: no Editor script, no hand-typed define, and no game symbol may start with
   `ARTEL_`. The cases the SDK cannot fix (SDK removed with the editor closed, a committed define
   on a machine with an older SDK) and the asmdef alternative are in
   [`../artel-cheats/references/compile-guard.md`](../artel-cheats/references/compile-guard.md).

## Write a behavior

```csharp
#if ARTEL_GAME_FUNCTIONS
using System.Collections;
using Artel;

public static class MyGameFunctions
{
    [ArtelBehavior("Casts spells with the hand cards the way the UI does, and ends the turn when no pair is left.", group: "combat")]
    static IEnumerator AutoCastSpells([CheatRange(1, 100)] int maxCasts, ArtelBehaviorContext context)
    {
        var casts = 0;
        try
        {
            while (true)
            {
                if (TurnBattleSystem.Instance == null) { context.Complete($"battle ended after {casts} casts"); yield break; }
                if (InteractionLock.IsLocked) { context.Report("waiting: interaction locked"); yield return null; continue; }
                if (casts >= maxCasts) { context.Complete($"reached maxCasts={maxCasts}"); yield break; }
                // ... one step of play through the same calls the UI makes ...
                casts++;
                context.Report($"cast {casts}");
                yield return null;              // next frame
            }
        }
        finally
        {
            CardManager.Inst?.CancelDrag();     // runs on stop, timeout, scene change
        }
    }
}
#endif
```

- **One loop iteration is one frame.** `yield return null` and `WaitForSeconds` work.
- **End yourself** with `context.Complete(reason)` when the job is done (no enemy left, the scene
  changed, the count reached), and `context.Fail(reason)` when it cannot continue.
- **Always clean up in `finally`.** The SDK disposes the iterator on every end, so `finally` runs on
  stop, timeout, replacement, scene change and run end.
- **Report progress** with `context.Report(text)`; the agent sees the latest line every second.
- **Wait out the game's own locks** (dialogue, animations) instead of fighting them.
- **Use `group:`** for behaviors that would fight over the same thing: starting one stops the running
  one in the same group.
- **Use `when:`** exactly as cheats do; when it turns false the running behavior ends with
  `unavailable`.
- **Say in the description that it calls game code**, so the agent does not use it to verify input.

## Verify

1. Compile, confirm Player Settings > Scripting Define Symbols contains `ARTEL_GAME_FUNCTIONS`,
   make a development build, and check the catalog lists the behavior with its `group`
   and parameters (the artel-cheats skill shows the commands).
2. Start it on a local server and watch it end by itself:

   ```bash
   curl -s -X POST http://127.0.0.1:8091/internal/action/<instanceId> -H 'Content-Type: application/json' \
     -d '{"type":"ACTION","id":9101,"actions":[{"id":1,"jsonrpc":"2.0","method":"start_behavior",
          "params":[{"name":"auto_cast_spells","args":{"maxCasts":20},"maxSeconds":60}]}]}'
   grep -E '"requestId":9101|BEHAVIOR_ENDED' <orchestration log>
   ```

   Expect a `behaviorId` back, then one `BEHAVIOR_ENDED` with a `completed` reason and your detail.
3. Start it again and stop it with `stop_behavior`; confirm the `finally` cleanup left the game
   usable (no card stuck to the cursor, no input lock left on).
4. Start it outside the place it works; it must be refused with `unavailable: <reason>`.

WordVenture result (2026-10-15 QA run): the agent started `auto_cast_spells(maxCasts=20)` in a
battle; it cast 7 spells in 26.3 seconds, defeated every enemy and ended with
`completed: battle scene ended after 7 casts`.

## References

- [Package README](https://github.com/project-artel/artel-sdk/blob/develop/Packages/kr.artel.sdk/README.md), section "Cheats and behaviors for the QA agent"
- [Protocol](https://github.com/project-artel/artel-sdk/blob/develop/docs/protocol.md) and [ADR 0007](https://github.com/project-artel/artel-sdk/blob/develop/docs/adr/0007-game-registered-cheats-and-behaviors.md) (in `develop` once artel-sdk#142 merges)
- [Behavior design](https://github.com/project-artel/artel/tree/main/.plan/general) — `2026-10-13-game-registered-behaviors-the-agent-runs-in-background.md` (in `main` once artel#20 merges)
