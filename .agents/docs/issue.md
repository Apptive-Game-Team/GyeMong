# Issue Workflow

## Why

An issue defines the problem and acceptance boundary. It should not prescribe implementation before investigation.

## Ready Criteria

An issue is ready when it has:
- clear problem statement
- user or system impact
- acceptance criteria
- known constraints
- explicit non-goals when scope could expand
- dependencies or blockers

## Issue Template

```markdown
## Problem

## Impact

## Acceptance Criteria
- [ ]

## Constraints

## Non-goals

## Validation Notes
```

## Safe Creation

Write generated issue content to a Markdown file before invoking GitHub CLI.
Pass the file with `--body-file`; do not interpolate multiline content into
`--body`. This prevents shell quoting, command substitution, and newline damage.

Use a temporary file unless the repository requires the issue draft to be
tracked:

```bash
gh issue create --title "<title>" --body-file /tmp/issue-body.md
```

After creation, read the remote issue back with `gh issue view` and confirm the
title and body match the source file. Fix the remote issue before continuing if
content is missing, truncated, or malformed. Remove temporary files after
successful verification.

## Lifecycle

1. Create or refine issue.
2. Confirm dependencies and priority.
3. Mark in progress only when active work starts.
4. Link branch, plan, and PR.
5. Update scope changes in issue before implementing them.
6. Close only after acceptance criteria and required validation pass.

## Sizing

Split issue when it contains multiple independently releasable outcomes or requires unrelated ownership areas.

Do not split tightly coupled steps that cannot provide value or validation independently.
