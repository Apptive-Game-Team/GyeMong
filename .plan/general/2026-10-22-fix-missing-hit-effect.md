# 2026-10-22 — 피격 이펙트 누락과 데미지 중단 수정

- Date: 2026-10-22
- GitHub Issue: #476
- Status: Implemented; Unity runtime validation pending

## Goal
피격 이펙트 참조를 복구하고 이펙트가 없어도 단발 공격의 데미지 처리를 유지한다.

## Non-goals
전투 밸런스, 다단히트, 무적 판정, 기타 로그 경고 변경.

## Context / Constraints
- 292f2354에서 필드와 Instantiate 호출을 추가했으나 Player.prefab 연결은 누락됐다.
- ce4365f9에서 연결을 추가했으나 현재 브랜치에는 포함되지 않았다.
- 기존 로컬 변경을 보존한다. fix/476에서 codex/artel-combat-qa를 대상으로 초안 PR을 생성한다.

## Approach (Checklist)
- [x] **Step 0: Recon** 코드, 프리팹, 과거 수정 및 실제 이펙트 GUID/fileID 확인.
- [x] **Step 1: Implementation** Player.prefab 참조 복구 및 HitCollider 이펙트 생성에만 null 검사 적용.
- [ ] **Step 2: Tests** diff 및 YAML 참조 검증, 가능한 경우 Unity 컴파일. Play Mode에서 참조 유무별 단발 공격, 반복 접촉, 다단히트 확인.
- [x] **Step 3: Rollout / Rollback** 변경된 두 파일의 이번 변경만 되돌리면 복구 가능.

## Validation
- **Commands to run:** git diff --check; Unity batchmode compile; GUID/fileID 대조.
- **Expected output:** 이펙트 참조가 실제 에셋을 가리키고, null일 때도 피격 처리 경로가 계속 실행된다.
- **Results:** 두 변경 파일의 git diff --check 통과. GUID 및 root GameObject fileID 대조 통과. 오브젝트 삭제 없음. 기존 다단히트 분기 및 데미지 처리 순서 유지 확인.
- **Pending:** Unity batchmode는 License Client 연결 거부 상태로 컴파일 결과 미확인. Play Mode, 씬 왕복, CI 빌드는 미실행.

## Risks & Rollback
- **Risks:** Unity Play Mode 및 씬 왕복 검증이 필요하다. CI는 원격 접속 복구 후 실행한다.
- **Rollback steps:** HitCollider null 검사와 Player.prefab의 hitEffect 필드 추가만 되돌린다.

## Open Questions
- 없음.
