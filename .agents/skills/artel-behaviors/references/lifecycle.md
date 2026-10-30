# Behavior lifecycle and reference

## How a behavior ends

The SDK sends one `BEHAVIOR_ENDED` frame with one of these reasons, and runs the `finally` block in
every case.

| Ends because | Reason |
|---|---|
| the agent stopped it | `stopped` |
| the next test case does not allow it | `not_allowed` |
| it returned or called `context.Complete` | `completed` + detail |
| it called `context.Fail` or threw | `failed` + detail |
| `maxSeconds` passed (default 30, at most 300; the agent can extend it) | `timeout` |
| another behavior in the same `group` started | `replaced` |
| its object was destroyed or its scene unloaded | `target_gone` |
| its `when` method turned false | `unavailable` + reason |
| `reset_game`, run end, or the connection closed | `run_ended` |

- At most 4 behaviors run at once.
- A behavior advances with game time, so pausing the game pauses it.
- A behavior may run across steps and test cases; the agent sees it in every observation until it
  ends.

## Attribute rules

Same as cheats (parameter types `int`, `float`, `bool`, `string`, `enum`; `[CheatRange]`; snake_case
names; instance methods on a `MonoBehaviour`; `when:`), plus:

- The return type must be `IEnumerator`.
- `ArtelBehaviorContext` may be the last parameter; the agent does not see it.
- `group:` names a set of mutually exclusive behaviors.
- `context.Report(text)` keeps the latest line, at most 200 characters.

## Wire

| Action | Params | Answer |
|---|---|---|
| `start_behavior` | `{ "name", "args", "target", "maxSeconds" }` | `{ "behaviorId": 7, "report": null }` |
| `stop_behavior` | `{ "behaviorId": 7 }` or `{ "all": true }` | `{ "stopped": [7] }` |
| `extend_behavior` | `{ "behaviorId": 7, "seconds": 60 }` | `{ "maxSeconds": 90 }` |

Running behaviors ride on every `PULSE` as `behaviors: [{ behaviorId, name, target, seconds, maxSeconds, report }]`.

## Failure messages the agent sees

| Message | Cause |
|---|---|
| `behaviors are not available in this build` | release build |
| `unknown behavior: <name>` | not registered |
| `bad argument <param>: <why>` | wrong type, out of range, unknown enum value; also `maxSeconds` outside 1–300 |
| `unavailable: <reason>` | the `when` method said no |
| `too many behaviors running: <names>` | 4 already running |
| `unknown behavior id: <id>` | stopping or extending one that already ended |
| `behavior not allowed in this run: <name>` | the run or the test case does not allow it (server side) |
