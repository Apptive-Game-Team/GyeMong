# Project Agent Instructions

## Scope and Precedence

This file is the repository-level entrypoint for coding agents.

Read `.agents/docs/project.md` before non-trivial work. Repository-specific
commands, constraints, and narrower instructions take precedence over these
template defaults.

## Project Workflow

For non-trivial work, follow:

- `.agents/docs/workflow.md`
- `.agents/docs/testing.md`

For tracked Git work, follow:

- `.agents/docs/issue.md`
- `.agents/docs/branch.md`
- `.agents/docs/commit.md`
- `.agents/docs/pull-request.md`

Use project-local skills when installed and applicable. Skill instructions
define their own triggers, formats, and output paths.

Installed in this repository:

| Path | Purpose |
|---|---|
| `.agents/skills/writing-plan/` | 비-사소한 작업 전 계획 문서 작성 |
| `.agents/skills/plan-review/` | 구현 전 계획 리뷰 |
| `.agents/skills/pair-review/` | 크리틱 서브에이전트와 페어 리뷰 |
| `.agents/skills/handoff/` | 세션 핸드오프 문서 작성 |
| `.agents/agents/developer.md` | 계획→구현→리뷰→PR 전 과정 에이전트 |
| `.agents/agents/pair-review-critic.md` | 독립 크리틱 서브에이전트 |

## Unity Constraints

This is a Unity 2022.3.34f1 (URP, 2D) project. The following override generic
guidance:

- **Never hand-edit `.meta` files' `guid` fields.** A GUID change breaks every
  scene, prefab, and asset reference pointing at that file.
- **Deleting an asset requires deleting its `.meta` sibling**, and removing any
  scene/prefab reference to it. Leftover references become missing-script or
  missing-reference errors at runtime.
- **Editing `.unity` / `.prefab` YAML by hand is allowed but must be verified.**
  After editing, confirm no `fileID` in the file points at a removed object.
- **`Assets/GyeMong_Art` is a Git submodule** (`Apptive-Game-Team/GyeMong_Art`).
  Art and audio changes belong in that repository; the main repository only
  records the pinned commit. Always check out with `--recurse-submodules`.
- Assembly definitions exist only inside third-party packages. Project scripts
  compile into the default assembly, so deleting a script that siblings
  reference breaks the whole compile.

## Communication

Issues, commits, and pull requests in this repository are written in Korean.
Match that. Code identifiers and comments stay in English unless surrounding
code says otherwise.
