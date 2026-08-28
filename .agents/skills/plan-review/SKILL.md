---
name: plan-review
description: >
  Plan review protocol. Use before non-trivial implementation when the user asks
  for plan review, review-plan, peer plan review, multi-model plan review,
  fast/medium/heavy review, or wants ambiguity and overengineering checked before
  coding. Runs fast and medium first-pass reviews, then a heavy second-pass
  verification, then folds findings back into the plan.
---

# Plan Review

Use this skill before non-trivial implementation when plan quality matters.

## Goal

Improve plans before code:
- fast reviewer catches ambiguity and missing decisions
- medium reviewer catches overengineering, YAGNI, weak boundaries, and DRY risk
- heavy reviewer verifies the revised plan and tradeoffs
- primary agent reflects accepted findings into the final plan

## Protocol

1. Build the review packet.
   - Task goal and acceptance criteria
   - Current plan
   - Relevant files, modules, APIs, schemas, or diffs
   - Constraints, risk points, and validation strategy
2. First pass: run fast and medium reviews.
   - Prefer parallel execution when subagents are available.
   - Fast review: ambiguity, missing details, unclear validation.
   - Medium review: overengineering, YAGNI, DRY, maintainability.
3. Revise the plan.
   - Apply all `must-fix` findings.
   - Apply useful `should-fix` findings.
   - Keep rejected suggestions in a short "Rejected feedback" note with reasons.
4. Second pass: run heavy review.
   - Give it the original task, original plan, first-pass findings, and revised plan.
   - It checks whether the revised plan is implementable and appropriately scoped.
5. Finalize.
   - If heavy review passes, use the revised plan for implementation.
   - If heavy review fails, update the plan and repeat heavy review once.
   - If it still fails, stop and report the blocker.

## Reviewer Tiers

Choose reviewers by role and available capability. Do not make one provider or
model name mandatory.

- **fast**: ambiguity, missing decisions, hidden assumptions, validation gaps.
  Prefer a fast/cheap reviewer with low or medium reasoning.
- **medium**: overengineering, YAGNI, DRY, coupling, maintainability. Prefer a
  strong everyday coding reviewer with medium reasoning.
- **heavy**: final gate, tradeoff validation, blocker check. Prefer the
  strongest available reviewer with high/deep reasoning.

If the environment supports explicit model or thinking-depth selection, map
tiers to the closest available options. Examples only:
- Codex/OpenAI-style: fast = small or everyday model with low reasoning; medium
  = everyday coding model with medium reasoning; heavy = strongest model with
  high/xhigh reasoning.
- Claude-style: fast = Haiku/light thinking; medium = Sonnet/standard thinking;
  heavy = Opus/deep thinking.
- Single-model environments: run the same model three times with distinct roles
  and increasing scrutiny.

If subagents are unavailable, self-review sequentially in fast, medium, then
heavy roles. Keep the role prompts separate.

## Prompt Templates

### Fast First Pass

```text
Review this implementation plan as the fast reviewer.

Focus only on ambiguity, missing decisions, hidden assumptions, unclear ownership,
edge cases, and validation gaps.

Return FAST REVIEW: PASS or FAST REVIEW: NONPASS.
For NONPASS, list must-fix/should-fix/question findings with concrete plan changes.
```

### Medium First Pass

```text
Review this implementation plan as the medium reviewer.

Focus only on overengineering, YAGNI, speculative abstractions, unnecessary
module splits, DRY risks, coupling, and maintainability.

Return MEDIUM REVIEW: PASS or MEDIUM REVIEW: NONPASS.
For NONPASS, list must-fix/should-fix/question findings with concrete plan changes.
```

### Heavy Second Pass

```text
Review this implementation plan as the heavy reviewer.

You receive the original task, original plan, fast findings, medium findings,
and revised plan. Verify that the revised plan reflects the right feedback,
rejects bad feedback for clear reasons, solves the task, and has a credible
validation strategy.

Return HEAVY REVIEW: PASS or HEAVY REVIEW: NONPASS.
For NONPASS, list only blockers with concrete plan changes.
```

## Rules

- The primary agent owns the final plan.
- Treat vague plans as failures.
- Model/provider names are examples, not requirements.
- Do not add abstractions only to satisfy DRY.
- Do not expand scope during review.
- Prefer explicit, localized, testable implementation steps.
- Heavy review is the final gate before implementation.
