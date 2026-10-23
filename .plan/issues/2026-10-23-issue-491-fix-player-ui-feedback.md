# 2026-10-23 — 플레이어 표시 및 UI 피드백 수정

- Date: 2026-10-23
- GitHub Issue: #491
- Status: Implemented; Play Mode validation pending

## Goal
정지 플레이어 표시를 복원하고 하트 없는 체력 UI, 균형 잡힌 대화 글자, 실제 조작 키 안내를 적용한다.

## Non-goals
전투 밸런스, 입력 진행 방식, 사용자 기존 에셋 변경 수정.

## Context / Constraints
Unity 2022.3.34f1. 아트는 별도 서브모듈이며 GUID를 변경하지 않는다. 기존 작업 트리 변경을 보존한다.

## Approach (Checklist)
- [x] **Step 0: Recon** 정지 애니메이션 참조, 대화 크기, 체력 프리팹, 팝업 문구 확인.
- [x] **Step 1: Implementation** 누락 스프라이트 참조 복구, 하트 UI 제거, 이름/본문 균형 및 키 표시 수정.
- [ ] **Step 2: Tests** Unity 컴파일, 방향별 대기/이동 전환, 한영 대화 및 재설정 키 안내 확인.
- [ ] **Step 3: Rollout / Rollback** 변경 단위 커밋과 PR, 문제 시 해당 커밋 revert.

## Validation
- Unity 에디터에서 네 방향 스프라이트 로딩 및 1x1 bounds 확인. 수정 스크립트 초기 컴파일 성공.
- 독립 pair-review PASS. 하트 제거 후 잔여 fileID 참조 없음.
- 편집 모드 입력 관리자 생성 검증에서 NullReference 발생 후 임시 오브젝트 검색 제외 플래그 제거. 수정된 검증 전체 재실행과 Play Mode 확인은 미완료.
- 검증 메뉴: Tools/GyeMong/Validate Player UI Feedback. 기존 열린 씬을 바꾸지 않는다.
- **Commands to run:** Unity batchmode 검증 진입점, git diff --check, YAML 참조 검사.
- **Expected output:** 컴파일 오류 없음, 누락 스프라이트 없음, 하트 없음, 동일한 대화 크기와 현재 키 표시.

## Risks & Rollback
- **Risks:** 아트 서브모듈과 메인 저장소의 배포 순서, 대화 긴 이름의 폭.
- **Rollback steps:** 수정 커밋 revert 및 기존 아트 핀 복원.

## Open Questions
- 없음. 피통은 현재 막대 UI를 유지하고 하트를 제거한다.
