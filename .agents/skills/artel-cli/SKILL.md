---
name: artel-cli
description: >-
  Install, sign in and use the ARTEL CLI (`artel`, npm @project-artel/cli) to create test cases and
  scenarios, launch a game build with the SDK, and run the ARTEL QA agent from a terminal or CI.
  Use when the user asks to "install the artel CLI", "CLI 셋팅", "artel auth login", "run QA from the
  terminal", "artel qa run", "start the game for QA", or to point the CLI at a local server.
---

# ARTEL CLI

The CLI is the npm package `@project-artel/cli`; it installs the command `artel`.

## 1. Install

Node.js 22.12.0 or newer is required. Check with `node --version`; if it is missing or older, stop
and ask the user before installing Node.

```bash
npm install -g @project-artel/cli@alpha
artel --version
```

Always use the `@alpha` tag; `latest` points at an outdated build. If the global install fails with
a permission error, do not use `sudo`; use `npx @project-artel/cli@alpha <command>` instead.

## 2. Sign in

Two ways; the first one found wins.

| Way | When | How |
|---|---|---|
| `ARTEL_TOKEN` | CI, or no browser | a CLI token issued in the console's account settings, exported as `ARTEL_TOKEN` |
| `artel auth login` | a person at a machine | opens the console in a browser and stores the token in `~/.artel/credentials.json` |

```bash
artel auth login
artel auth status        # which credential is used; prints a fingerprint, never the token
```

The default target is the stage deployment (API `https://stage-orch.artel.kr`, console
`https://home.stage.artel.kr`). For another server pass both addresses, or set
`ARTEL_API_BASE_URL` and `ARTEL_CONSOLE_BASE_URL`:

```bash
artel auth login --api-url http://localhost:8080 --console-url http://localhost:5173
```

Never print a token or paste it into a chat; pass it through the environment.

## 3. The QA loop

```bash
artel project list                                         # project ids
artel game start --build ./Build/Game.exe --project <p>    # launch signed in, wait for registration
artel game list --project <p>                              # instance ids for qa run
artel case create --project <p> --file cases.json --json   # test cases
artel scenario create --project <p> --title "Smoke" --steps steps.json --json
artel run create --project <p> --name smoke --json         # a test run (a set of scenarios)
artel run scenarios <run id> --project <p> --set <scenario ids>
artel qa run --test-run <run id> --instance <instance id>  # start and watch to a verdict
artel qa show <qa run id>
artel issue list --project <p>
```

Every command that reports a result takes `--json`; keys never disappear and unknown values are
`null`. `artel <group> --help` lists the rest.

### Test case folders

Before creating test cases, inspect the project's folders with
`artel case folder list --project <p> --json`. Follow the user's requested classification first,
then the project's existing folder structure. When neither provides a classification, group new
test cases by player-facing feature, such as Combat, Inventory, or Quests.

Reuse matching folders. Create a missing feature folder only when needed with
`artel case folder create --project <p> --name <feature> --json`; add a subfeature folder with
`--parent <folder id>` only when it helps organize the cases. Use the project's naming language.
If a case's classification is unclear, use the existing unclassified folder, or leave it without
a folder if none exists. Do not move existing test cases or rename, move, or restructure existing
folders unless the user requests that work.

Set each new case's `folderId` to the selected folder's returned id. For a file whose cases all
belong in one folder, pass `--folder <folder id>` to `artel case create`; an explicit per-case
`folderId` takes precedence over `--folder`. When leaving unclassified cases without a folder,
omit both their `folderId` and the command's `--folder`.

### File shapes that trip people up

`cases.json` (one object or an array):

```json
[{ "scene": "Map_scene", "step": "Enter the boss stage", "precondition": "The character is on the boss node",
   "expectedValue": "The boss battle scene loads" }]
```

`steps.json` for `scenario create`: `case_id` is a **number**, the database id from `case create`
(a string is rejected with `scenario_invalid_steps`).

```json
[{ "action": "Enter the boss stage from the map", "case_id": 1, "hint": null, "input": null }]
```

### Run options worth knowing

| Flag | What it sets |
|---|---|
| `--model <id>` | the model (`artel qa models` lists ids) |
| `--prompt-version <v>` | pin the agent prompt version |
| `--content-map-mode on\|frozen\|off` | how much of the content map the run reads and writes; use `frozen` to measure |
| `--knowledge-mode learning\|frozen\|off` | the same for the knowledge store |
| `--label <name>` | the experiment name only, never the arm |
| `--force` | take over a game instance another run is using |
| `--no-wait` | start and return |

Cheats and behaviors registered in the game are on by default for a run whose build has them; see
the artel-cheats and artel-behaviors skills.

## Troubleshooting

| Symptom | Cause |
|---|---|
| `game_registration_timeout` | the build is not a development build, or another process holds the instance (see artel-unity-sdk) |
| `sdk_disconnected` on `qa run` | the game is not running or not connected; `artel game list` |
| `upstream_unavailable` on `qa run` | the server cannot reach the agent server |
| Two instances with the same id when launching several builds at once | serialize `artel game start` (for example with `flock`) and run the QA runs in parallel |

## References

- [CLI README](https://github.com/project-artel/artel-cli/blob/main/README.md) — every command group and flag
- [artel.kr/cli.md](https://artel.kr/cli.md) — step-by-step install instructions written for agents
- `artel <group> --help` — the installed version's exact flags
