# Cheat reference

## Attribute rules (checked by the SDK weaver at compile time)

| Rule | Detail |
|---|---|
| Parameter types | `int`, `float`, `bool`, `string`, or an `enum` |
| Ranges and defaults | `[CheatRange(min, max)]`; a C# default value makes the parameter optional |
| Return type | `void`, `string` (the message the agent reads), or `IEnumerator` for a cheat that waits (a scene load) |
| Name | the method name in snake_case (`UnlockStage` → `unlock_stage`); override with `name:`; duplicates are an error |
| Static or instance | instance methods must be on a `MonoBehaviour`; with several live instances the agent passes a `target` selector |
| `when:` | a parameterless method on the same type returning `bool` or `string` (`null` = available, otherwise the reason); checked once a second and on every call |
| Release builds | nothing is generated; the attribute is inert and calls are refused |
| Runtime registration | `ArtelCheats.Register(name, description, params, handler)` / `Unregister` for cheats built from data |

Registration is generated code that calls the methods directly, so there is no reflection and
managed stripping keeps the methods.

## Wire

The agent's call reaches the game as one `ACTION` item:

```json
{ "method": "invoke_cheat", "params": [{ "name": "go_to_scene", "args": { "scene": "Battle", "stage": 0 }, "target": null }] }
```

The game answers with an `ACTION_RESULT` item whose `returnValue.message` is the string the cheat
returned. On connect the SDK sends `GAME_FUNCTION_CATALOG`, which lists every cheat with its
parameters, ranges and enum values; every `PULSE` carries which cheats are available now.

## Failure messages the agent sees

| Message | Cause |
|---|---|
| `cheats are not available in this build` | release build |
| `unknown cheat: <name>` | not registered (typo, or the file is not compiled) |
| `bad argument <param>: <why>` | wrong type, out of range, unknown enum value |
| `<name> is static and takes no target` | a target was passed to a static cheat |
| `unavailable: <reason>` | the `when` method said no |
| `no active <Component>` | instance cheat with no live object |
| `<n> <Component> objects; pass target as one of: ...` | instance cheat with several live objects and no target |
| `Nothing at selector: <selector>` / `<Component> is not on <selector>` | the target does not resolve |
| `cheat not allowed in this run: <name>` | the run or the test case does not allow it (server side) |
| `<ExceptionType>: <message>` | the cheat threw; fix the cheat |
| `<name> did not finish within 60 seconds` | an `IEnumerator` cheat never ended |
