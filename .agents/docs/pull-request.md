# Pull Request Workflow

## Why

PR should let reviewer understand intent, verify evidence, and identify risk without reconstructing development history.

## Before Opening

- Confirm issue acceptance criteria.
- Update plan to reflect final implementation.
- Review full diff against default branch.
- Remove debug code and unrelated churn.
- Run required validation.
- Confirm migrations, configuration, and rollback needs.

## Title

Use Conventional Commit format:

```text
<type>(<optional-scope>): <imperative summary>
```

## Body Template

```markdown
## Why

## What Changed

## Validation
- [ ] Command or manual check

## Risks

## Rollback

Closes #123
```

## Safe Creation

Write generated PR content to a Markdown file before invoking GitHub CLI. Pass
the file with `--body-file`; do not interpolate multiline content into `--body`.
This prevents shell quoting, command substitution, and newline damage.

Use a temporary file unless the repository requires the PR draft to be tracked:

```bash
gh pr create --title "<title>" --body-file /tmp/pr-body.md
```

After creation, read the remote PR back with `gh pr view` and confirm the title
and body match the source file. Fix the remote PR before reporting completion if
content is missing, truncated, or malformed. Remove temporary files after
successful verification.

## Review Rules

- Keep PR focused on one coherent outcome.
- Mark draft while known required work remains.
- Respond to each actionable review comment.
- Resolve threads only after change or explicit agreement.
- Add new commits during review when history clarity matters.
- Squash only when repository policy prefers a single final commit.

## Merge Criteria

- acceptance criteria satisfied
- required checks pass
- review approvals complete
- unresolved risks explicitly accepted
- deployment or migration order documented
