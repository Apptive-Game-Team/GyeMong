# Which behaviors to make

A behavior earns its place when a test needs **input every frame or every turn for longer than one
agent turn**, and that input is not what the test verifies. If one call can produce the state, make a
cheat instead.

## Make these first

| Kind | Why | Example |
|---|---|---|
| **Keep fighting** | Real-time and turn-based fights outlast the agent's turn; the character stands still otherwise | `auto_cast_spells(maxCasts)` (WordVenture), `auto_attack(range)` |
| **Keep passing time** | Tests about cooldowns, day/night, turn counts, timers | `pass_turns(turns)` (WordVenture) |
| **Survive** | Dodge, flee, block while the agent watches something else on screen | `flee()`, `keep_blocking()` |
| **Keep moving** | Walk to a point, patrol, follow a path the agent should not steer frame by frame | `walk_to(spawn_point)` |
| **Keep a resource up** | Drink potions, reload, recharge so a long test does not fail for an unrelated reason | `keep_health_above(percent)` |

## Do not make these

- **A behavior that wins by itself when winning is what the test checks.** Keep the decisions with
  the agent: a behavior gives input, the agent judges.
- **A behavior that drives the controls under test.** A behavior calls game code, so it cannot catch
  a broken button binding. Tests of the controls use real input.
- **A behavior that never ends.** Every behavior needs at least one `Complete` condition (job done,
  scene changed, count reached). The SDK also stops it at `maxSeconds` (default 30, at most 300).
- **Two behaviors that move the same thing without a shared `group`.** They will fight; put them in
  one group so starting one stops the other.

## For each behavior, write down

1. What it does each frame, through which game methods.
2. When it is done (`Complete`) and when it gives up (`Fail`).
3. What it must undo when stopped early (the `finally` block).
4. Which lock or state it must wait for.
5. Its group, and where it cannot run (`when:`).
6. Which test cases must not use it, so the user can exclude it in the test case's `functions` field.
