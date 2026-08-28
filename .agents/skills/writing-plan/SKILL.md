---
name: writing-plan
description: Generate a work plan markdown file under .plan/general/ or .plan/issues/ with strict naming conventions. Use when asked to "write a plan" or when starting any non-trivial change. Optionally links plans to GitHub issues.
---

# writing-plan Skill

## Trigger conditions
Use this skill when:
- The user asks for a plan / planning / 작업 플랜 / 실행 계획.
- You are about to implement a non-trivial change and there is no plan file yet.
- You are preparing a PR and need a PR-reviewable plan.

## Inputs (ask if missing)
- **title** (required): Short human-readable title for the plan.
- **github_issue** (optional): GitHub issue reference as `123`, `#123`, or an issue URL.

## Output rules (MUST)

- **Date format:** `YYYY-MM-DD` (must appear in filename and inside file body).
- **Slug generation:**
  - **Translate non-English titles to English summary for the slug.** (e.g., "가격 캐시 추가" -> "add-price-cache")
  - Lowercase everything.
  - Replace spaces/underscores with `-`.
  - Remove special characters.
  - Collapse repeated `-` and trim leading/trailing `-`.
  - Keep reasonably short (<= 50 chars).

### General plan (No GitHub issue)
1. **Ensure directory:** `.plan/general/`
2. **Check collision:**
   - Target: `.plan/general/<YYYY-MM-DD>-<slug>.md`
   - If target exists, append suffix: `-2`, `-3`, etc. (e.g., `...-slug-2.md`)
3. **Create File:** `.plan/general/<YYYY-MM-DD>-<slug>.md`

### GitHub issue plan (GitHub issue provided)
1. **Normalize issue number:**
   - `123` → `123`
   - `#123` → `123`
   - GitHub issue URL → extract the issue number
2. **Validate issue number:** Must contain digits only after normalization.
3. **Optional check:** If `gh` is available and repo context exists, run `gh issue view <number>` to verify the issue. If verification fails due to auth, sandbox, or network issues, continue with the normalized number unless the user explicitly asked for verification.
4. **Ensure directory:** `.plan/issues/`
5. **Check collision:**
   - Target: `.plan/issues/<YYYY-MM-DD>-issue-<number>-<slug>.md`
   - If target exists, append suffix: `-2`, `-3`, etc. (e.g., `...-slug-2.md`)
6. **Create File:** `.plan/issues/<YYYY-MM-DD>-issue-<number>-<slug>.md`

## Plan content (MUST)
1. **Load template:** Read `assets/PLAN_TEMPLATE.md`.
2. **Replace placeholders:**
   - `{{DATE}}` → Current date (YYYY-MM-DD)
   - `{{TITLE}}` → User provided title (Keep original language here)
   - `{{GITHUB_ISSUE_OR_NONE}}` → GitHub issue reference (e.g., `#123`) or string "None"
3. **Fill sections:** Do not delete headers. Add initial thoughts if context is available.

## Return format (in chat)
After creating the plan file, respond with:
- **Created file path** (clickable if possible)
- **3–5 bullet summary** of the plan goal
- **Open questions** (if any context is missing)
