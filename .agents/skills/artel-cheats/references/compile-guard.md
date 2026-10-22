# Keeping the game compiling without the SDK: `ARTEL_GAME_FUNCTIONS`

Every file that uses `[ArtelCheat]`, `[ArtelBehavior]`, `[CheatRange]` or `ArtelBehaviorContext`
must be wrapped, whole, in `#if ARTEL_GAME_FUNCTIONS`. Without the wrap, a project that has no SDK,
or an SDK without the cheat feature (artel-sdk `develop` before #142), fails with dozens of
`error CS0246` and the game stops building.

```csharp
// ARTEL_GAME_FUNCTIONS is defined by the ARTEL SDK (kr.artel.sdk) when it ships cheats.
// Without that SDK the define is absent and this whole file is left out of the compile.
#if ARTEL_GAME_FUNCTIONS
using System.Collections;
using Artel;

public static class MyGameFunctions
{
    // cheats, behaviors and their when: conditions
}
#endif
```

Put the `#if` above the `using` lines and the `#endif` after the last brace. Nothing outside the
wrapped file may call into it; if game code needs a cheat helper, move the helper out of the file
instead of referencing the wrapped class.

**The wrap is the only step on the game side.** Do not add an Editor script, an asmdef or a
`csc.rsp` to the game for this, and do not type the define into Player Settings by hand.

## Who sets the define

The SDK owns it. Its Editor assembly `Artel.GameFunctions.Editor` (`ArtelDefineSync`) runs on every
editor script load:

- adds every define this SDK version supports (today only `ARTEL_GAME_FUNCTIONS`) to the scripting
  define symbols of Standalone, the active build target, and every other build target that already
  has defines;
- removes every define that starts with `ARTEL_` but is not in that list, which a newer SDK may
  have left behind;
- never touches a define that does not start with `ARTEL_`;
- writes Player Settings only when the list actually changes.

When the package is removed, or changes version, through the Package Manager while the editor is
open, it removes every `ARTEL_` define during `PackageManager.Events.registeringPackages`, before the
change is applied. The wrapped file drops out, the next compile succeeds, and the new version (if
any) adds its own defines back.

`ARTEL_` is reserved for the SDK. Never define your own `ARTEL_*` symbol in a game: the SDK deletes
it on the next editor load.

Measured on WordVenture with Unity 2022.3.62f3 in batchmode (each line is one editor run):

| Start | Change | Result |
| --- | --- | --- |
| define absent | SDK with cheats | define added, cheats compiled, 0 errors, 1 run |
| define absent | `develop` SDK or no SDK | wrapped file skipped, 0 errors, 1 run |
| define present | `Client.Remove("kr.artel.sdk")` | define removed, 0 errors, 1 run |
| define present | `Client.Add` of an older SDK version | define removed, 0 errors, 1 run |
| `MY_GAME_FLAG` present | any of the above | left as is |

## Cases the SDK cannot fix

Unity runs no package Editor code while a script fails to compile: in batchmode it stops at
`Scripts have compiler errors.` after compiling, before loading any project assembly, even when
the SDK's own assembly compiled. So once a leftover define has broken the compile, the SDK cannot
remove it. That happens in two cases:

1. The SDK was removed by editing `Packages/manifest.json` with the editor closed, so the removal
   event never fired. Measured: still 81 errors after 2 runs.
2. A committed `ProjectSettings/ProjectSettings.asset` carries `ARTEL_GAME_FUNCTIONS` to a machine
   whose SDK has no cheats (a different `file:` checkout or an unpinned git URL). Measured: 78
   errors, no change on later runs.

The fix is the same: delete `ARTEL_GAME_FUNCTIONS` once in **Project Settings > Player > Other
Settings > Scripting Define Symbols** (or from the `scriptingDefineSymbols` block in
`ProjectSettings/ProjectSettings.asset`), or install the SDK version that ships cheats. Tell the
user this instead of adding game-side code.

Unity writes the define into `ProjectSettings.asset`, so it shows up in `git status` after the
first editor load. Committing it is harmless when `Packages/manifest.json` and
`packages-lock.json` pin the same SDK for everyone; leave it out of the commit otherwise, because
the SDK adds it on each machine anyway.

## Games that already use asmdefs

This is an extra option, not a replacement for the wrap. If the cheat file sits in the game's own
assembly definition, the asmdef can guard the whole assembly without the scripting define: add a
**Version Define** on resource `kr.artel.sdk` whose expression is `0.2.0` (the first SDK version that
ships cheats), defining a game-owned symbol such as `GAME_HAS_ARTEL_CHEATS`; list that symbol in
**Define Constraints**; and reference `Artel.Runtime`. Unity then skips the assembly when the SDK is
missing or too old. Keep that symbol off the `ARTEL_` prefix, which belongs to the SDK.
`Assembly-CSharp` has no asmdef, so for most games the `#if ARTEL_GAME_FUNCTIONS` wrap stays the
way to do it.
