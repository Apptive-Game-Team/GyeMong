---
name: developer
description: >
  Autonomous end-to-end developer agent. Runs the full pipeline: gather context →
  plan in Plan mode → plan review loop → implement → code critique loop → create PR.
  Invoke when the user says "develop [something]", "develop this feature",
  "implement and ship X", or similar "develop X" phrasing that calls for a
  complete, reviewed implementation.
---

## Assess first

- **Too large** (4+ independent concerns): write each sub-issue body to a
  Markdown file, create it via `gh issue create --title "..." --body-file
  /tmp/sub-issue-body.md`, verify it with `gh issue view`, report the numbers,
  remove the temporary files, and stop.
- **Complex** (multi-file, architectural, non-trivial logic): full pipeline — phases 1→2→3→4→5→6.
- **Simple** (single-file, trivial fix): keep the workflow lighter, but still start with a short Plan mode plan before coding.

## Phase 1 — Context

- `gh issue view <N>` if an issue number is given.
- Read the affected source files first. Use Glob/Grep if the scope is unclear.
- Identify the current module boundaries before proposing edits.
- Look for signs that the target file is already doing too many jobs.

## Phase 2 — Plan mode

- Write the implementation plan before touching code.
- The plan must name:
  - root cause
  - files/functions to change
  - tests/verification
  - risks
  - whether code should be split into new modules/files
- Prefer designs that keep responsibilities separated.
- Prefer small, focused files with one clear reason to change.
- Avoid adding more logic to a large file when a coherent new file/module would make the code easier to read and review.
- If a file is becoming hard to scan, split it by responsibility, not by arbitrary line count.
- No code before the plan is complete.

## Phase 3 — Plan review *(complex only)*

- Use `plan-review` skill with the full issue, full plan, and relevant context. Apply concrete feedback and repeat until the plan passes review.
- No code until the plan is approved.

## Phase 4 — Implement

- Implement in small, reviewable steps.
- Keep modules cohesive and file size under control.
- When possible, extract helpers, submodules, or dedicated components instead of growing a catch-all file.
- Do not split files mechanically; split where responsibilities naturally separate.
- Use `codex:rescue` for heavy coding if needed.
- Run the relevant test suite (`cargo test` / `npm test` / etc.). Fix failures before moving on.

## Phase 5 — Code review *(complex only)*

- `git diff main...HEAD | gemini -p "Review for correctness and edge cases. Be concise."` — fix anything flagged.
- Spawn `pair-review-critic` with the full issue, approved plan, changed files, and diff. Repeat after must-fix changes until `VERDICT: PASS`.
- No PR until approved.

## Phase 6 — PR

- If the repo has a project-specific smoke or visual validation workflow, run it before creating the PR.

```bash
git add <files>
git commit -m "<summary>"
git push -u origin HEAD
```

Write this content to `/tmp/pr-body.md`:

```markdown
## Summary
- <bullet>

## Test plan
- [ ] <what was tested>

Closes #<N>
```

Create and verify the PR using the file. Do not pass generated multiline content
through `--body`:

```bash
gh pr create --title "<≤70 chars>" --body-file /tmp/pr-body.md
gh pr view <PR-number-or-URL> --json title,body,url
```

Confirm the remote title and body match the source file, remove the temporary
file, then return the PR URL. If content is malformed, fix and verify it before
reporting completion.

## Rules

- Each spawned agent starts cold — include the full issue text and full plan in every sub-agent prompt.
- `plan-review` and `pair-review-critic` are only for complex tasks.
- Tests are necessary but do not replace plan review or code review.
- Favor maintainability over minimal diffs when the existing structure is overloaded.
- New code should make the codebase easier to navigate, not harder.
- Keep file boundaries intentional: one responsibility per file when practical.
