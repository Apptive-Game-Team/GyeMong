# Which cheats to make

A cheat earns its place when it removes play the QA agent would otherwise repeat before a test can
even start. Start from the test cases, not from the game: ask the user which preconditions take the
longest to reach, or read the test cases' `precondition` fields and count how often each state
appears.

## Make these first

| Kind | Why | WordVenture example |
|---|---|---|
| **Go to a scene** | Almost every precondition names a scene. One enum-parameter cheat covers all of them | `go_to_scene(scene, stage)` |
| **Unlock progress** | Later stages, levels and chapters otherwise need the whole game played | `unlock_stage(stage)` |
| **Reach the end** | Endings, credits and post-game screens are the slowest states to reach | `show_ending()` |
| **Clear the current obstacle** | Lets a test move past a fight or puzzle that is not what it tests | `kill_all_slimes()` |
| **Survive** | Keeps a long test from dying of an unrelated cause | `heal_player(amount)` |
| **Give a resource** | Gold, items, cards, currency that a shop or crafting test needs | (WordVenture has none) |
| **Set a flag** | Tutorial seen, quest done, dialogue read — the switches that change what a screen shows | (tutorial flag would fit) |

## Do not make these

- **A cheat that produces a verified result.** "Win the battle" as one call makes every "the
  battle can be won" test meaningless. Prefer the obstacle-clearing form (defeat what is on screen)
  and let the game's own victory logic run.
- **A cheat that bypasses the code under test.** If the save system is being tested, a cheat that
  writes `PlayerPrefs` directly hides its bugs. Call the save method instead.
- **A cheat with free-form input** (an arbitrary scene name string, a raw `PlayerPrefs` key). The
  agent will invent values. Use an enum or a range.
- **Many near-identical cheats** (`go_to_title`, `go_to_map`, ...). One cheat with an enum
  parameter is easier for the agent to choose from.

## For each cheat, write down

1. The state it leaves, in one sentence. This becomes the attribute description.
2. What the normal path to that state also sets (singletons, statics, save keys) — the cheat must
   set those too.
3. Where it cannot work. This becomes the `when:` method.
4. What it returns, as `key=value`.
5. Which test cases must not use it (those that verify the state the cheat produces). Tell the user
   so they can set the test case's `functions` field to exclude it.
