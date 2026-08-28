# Development Workflow

## Why

Small, explicit steps reduce regressions and make review, rollback, and handoff predictable.

## Work Classification

Trivial work:
- documentation typo
- isolated formatting fix
- deterministic one-line configuration change

Non-trivial work:
- behavior change
- bug fix requiring investigation
- dependency or schema change
- cross-module refactor
- user-facing workflow change

Non-trivial work requires a concise plan. Use the `writing-plan` skill when it
is installed.

## End-to-End Flow

1. Confirm goal, scope, acceptance criteria, and non-goals.
2. Read project context, relevant code, tests, and recent changes.
3. Link or create an issue. Non-trivial development branches require one.
4. Create a branch using the issue number.
5. Write a concise implementation plan; use `writing-plan` when installed.
6. Identify architecture impact, tradeoffs, risks, and rollback.
7. Implement the smallest coherent change.
8. Follow `testing.md`; use an installed testing skill when available.
9. Review the complete diff for scope, correctness, and accidental churn.
10. Commit coherent units using the commit convention.
11. Open a PR with evidence and explicit remaining risk.
12. Address review without hiding unresolved concerns.

## Change Rules

- Preserve existing architecture unless the task requires changing it.
- Keep unrelated cleanup out of the change.
- Add abstractions only when they remove demonstrated complexity or match an established pattern.
- Keep migrations backward-compatible when practical.
- Prefer reversible rollout for high-risk behavior.

## Stop Conditions

Pause and surface the problem when:
- requirements conflict
- destructive action lacks approval
- required credentials or external access are unavailable
- validation reveals an unrelated pre-existing failure that blocks confidence
- scope expands beyond the agreed issue or plan

Do not silently guess through high-impact ambiguity.
