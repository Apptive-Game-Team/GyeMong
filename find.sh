#!/bin/bash
# search_managed_reference.sh
# Unity 프로젝트의 Assets 폴더 아래에서 특정 ManagedReference ID가 포함된 파일 찾기

SEARCH_ID="<<<<<<<<"
SEARCH_DIR="./Assets"

echo "🔍 Searching for ManagedReference ID: $SEARCH_ID in $SEARCH_DIR ..."
echo

# .unity, .prefab, .asset 같은 YAML 파일에서만 검사
grep -R --include=\*.unity --include=\*.prefab --include=\*.meta --include=\*.asset "$SEARCH_ID" "$SEARCH_DIR" | while read -r line ; do
    echo "Found in: $line"
done

echo
echo "✅ Search complete."
