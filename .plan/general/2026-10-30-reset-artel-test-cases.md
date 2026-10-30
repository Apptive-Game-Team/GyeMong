# 2026-10-30 — ARTEL 테스트 케이스 전체 교체

- Date: 2026-10-30
- GitHub Issue: None
- Status: Server replacement verified; ready for push

## Goal
사용자 요청에 따라 gyemong 프로젝트 10의 기존 TC 42개를 삭제하고 치트·행동 사용 제한 없는 게임 중심 TC로 교체한다. 기존 전체 TC 시나리오 46은 신규 ID로 다시 연결한다.

## Non-goals
전투 알고리즘 변경, 다른 프로젝트 TC 삭제, 실행하지 않은 QA 성공 판정, 폴더 전체 재구성.

## Context / Constraints
`artel` 브랜치에서 진행. 기존 TC 원본과 원격 JSON/시나리오는 Git에서 제외되는 Logs에 백업한다. 현재 CLI 소스 빌드 복사본을 사용하고 인증은 환경변수로 전달한다. TC와 실행 안내에서 치트 금지 문구를 제거한다.

## Approach (Checklist)
- [x] **Step 0: Recon** 프로젝트 10의 TC 42개 및 폴더 4/6, 시나리오 46 확인.
- [x] **Step 1: Implementation** 기존 TC 42개 삭제 및 0개 조회 확인. 신규 20개(1493–1512)를 폴더 4에 생성하고 시나리오 46 재연결. 게임 함수 설명과 실행 문서의 용도 제한 문구 제거.
- [x] **Step 2: Tests** 신규 TC 20개 개별 재조회에서 네 본문 필드/폴더 일치. 최종 20개, 기존 ID 0개. 시나리오의 신규 참조/행동/허용 안내 20개 모두 일치. 사용 금지 문구 검색 결과 없음. 실제 QA는 미실행.
- [x] **Step 3: Rollout / Rollback** 문서와 등록 기록 갱신 및 백업 완료. 관련 변경만 커밋/푸시하고 SHA 일치를 확인한 뒤 전달한다. 복원은 백업 본문으로 재생성하고 반환 ID로 시나리오를 다시 연결한다.

## Validation
- **Commands to run:** CLI case list/delete/create/show, scenario update/show, git diff --cached --check.
- **Expected output:** 기존 42개 ID 없음, 새 원본과 서버 데이터 일치, 시나리오에서 신규 ID만 참조.
- TC 실행 자체는 이 작업에 포함하지 않는다. 등록 상태는 DRAFT로 남긴다.

## Risks & Rollback
- **Risks:** TC 삭제 후 과거 TC ID는 복원되지 않는다. 기존 QA 이력은 새 TC의 실행 증거가 아니다.
- **Rollback steps:** Logs/artel-cases-before-reset.json 및 artel-scenario-46-before-reset.json, 로컬 원본 백업을 사용한다. 새 ID로 재생성한 TC를 단계에 다시 연결한다.

## Open Questions
- 없음. 사용자가 기존 TC 전체 삭제와 치트 사용 제한 제거를 명시했다.
