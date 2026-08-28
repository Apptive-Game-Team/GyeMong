#!/usr/bin/env bash
# Usage: echo "content" | ~/.agents/library/skills/handoff/handoff.sh
# Saves stdin as a handoff file in <cwd>/.agents/handoffs/

set -euo pipefail

HANDOFFS_DIR=".agents/handoffs"
TIMESTAMP=$(date '+%Y-%m-%d_%H-%M')
FILENAME="${HANDOFFS_DIR}/${TIMESTAMP}.md"
LATEST="${HANDOFFS_DIR}/LATEST.md"

mkdir -p "$HANDOFFS_DIR"

CONTENT=$(cat)

if [[ -z "$CONTENT" ]]; then
  echo "Error: no content on stdin" >&2
  exit 1
fi

printf '%s\n' "$CONTENT" > "$FILENAME"
printf '%s\n' "$CONTENT" > "$LATEST"

echo "Handoff saved to: ${FILENAME}"
