---
name: pair-review
description: >
  Pair review protocol with a critic subagent. Use when the user asks for pair
  review, peer review, critic review, or a code quality pass focused on YAGNI,
  DRY, maintainability, scope control, and implementation tradeoffs during
  planning or coding.
---

# Pair Review

Use this skill to add an independent critic subagent to non-trivial planning or implementation work.

## Goal

Catch design drift early:
- YAGNI violations: speculative features, unused abstractions, premature generalization
- DRY violations: meaningful duplication, copy-pasted logic, split-brain behavior
- Code quality risks: unclear boundaries, hidden state, deep nesting, weak names, hard-to-test code
- Scope creep: changes not required by the task
- Validation gaps: missing edge cases, weak tests, unverified behavior

## Protocol

1. Define the review target.
   - Include the user goal, plan, changed files or diff, constraints, and validation strategy.
   - Keep the packet small enough that the critic can inspect the relevant code independently.
2. Spawn the `pair-review-critic` subagent.
   - Ask it to read the relevant files before judging.
   - Ask for `VERDICT: PASS` or `VERDICT: NONPASS`.
3. Apply the critic feedback deliberately.
   - Fix must-fix issues.
   - Accept or reject should-fix issues with a concrete reason.
   - Do not expand scope only because the critic suggested a larger redesign.
4. Repeat only when needed.
   - Repeat after must-fix changes on complex or risky work.
   - Skip repeat review for small wording or formatting fixes.

## Review Timing

- **Plan review**: before code when the approach has architectural tradeoffs.
- **Mid-implementation review**: when a helper, module boundary, or abstraction is forming.
- **Diff review**: after code changes, before final validation or PR.

## Critic Prompt Template

```text
Use the pair-review-critic role.

TASK:
<user goal and acceptance criteria>

PLAN:
<current plan or implementation approach>

CHANGED FILES / DIFF:
<file list, relevant diff, or paths to read>

CONSTRAINTS:
<architecture rules, project style, tests already run, known tradeoffs>

Focus on YAGNI, DRY, code quality, maintainability, scope control, and validation gaps.
Return VERDICT: PASS or VERDICT: NONPASS. Cite file:line for concrete issues.
```

## Rules

- The primary agent owns the implementation and final judgment.
- The critic is adversarial about quality but not about taste.
- Block only on correctness, maintainability risk, scope creep, or missing validation.
- Do not block on style preferences without a concrete maintainability or correctness impact.
- Prefer the smallest change that preserves clarity and long-term maintainability.
