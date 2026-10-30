# 2026-10-30 — ARTEL 브랜치와 회귀 TC 정리

- Date: 2026-10-30
- GitHub Issue: None (자동 승인 검토가 이슈 게시 거절)
- Status: Implementation and registration verified; ready for push

## Goal
현재 작업을 사용자 지정 `artel` 브랜치에 보존하고 SDK·스킬·에디터 가드를 커밋/푸시한다. 기존 치트 6개와 행동 7개 및 등록된 TC 39개를 재사용하고 SDK/재컴파일 회귀 TC를 CLI로 추가한다.

## Non-goals
전투 기능 중복 구현, 기존 TC 중복 등록, 아트 서브모듈 및 자동 생성 에셋 변경, 이슈/PR 게시.

## Context / Constraints
Unity 2022.3.34f1 프로젝트가 에디터에서 열려 있다. 기존 구현과 39개 TC는 이미 커밋 및 서버 등록되어 있다. 현재 설치된 SDK 97c41053731d41a86dd778310811f5f756db6763을 고정한다. 브랜치 이름은 사용자 요청이 일반 이슈 번호 규칙보다 우선한다. 자격증명은 ARTEL_TOKEN 환경변수로만 전달한다.

## Approach (Checklist)
- [x] **Step 0: Recon** 기존 구현, SDK API, 프로젝트 10의 TC 39개와 로컬 네 필드 일치 확인.
- [x] **Step 1: Implementation** artel 브랜치 전환. SDK 커밋 고정, 기존 에디터 가드와 스킬 변경 포함. 회귀 TC 원본 작성.
- [x] **Step 2: Tests** JSON 및 SDK manifest/lock 일치, 기존 DLL의 카탈로그 이름 13개 확인. CLI로 1490–1492 생성 후 개별 재조회하여 네 필드와 폴더 6 일치 확인. Play Mode/빌드는 이번에 미실행.
- [x] **Step 3: Rollout / Rollback** SDK 고정, 에디터 가드, QA 스킬, TC 문서로 커밋 분리. 롤백 경로 기록. origin/artel 푸시 후 로컬/원격 SHA 확인을 최종 전달 조건으로 한다.

## Validation
- **Commands to run:** JSON 파싱, 기존 Assembly-CSharp 카탈로그 문자열 검사, git diff --cached --check, CLI case list/create/show, git ls-remote.
- **Expected output:** 13개 게임 함수 유지, SDK manifest/lock 일치, 신규 TC 필드 일치, 원격 SHA 일치.
- 현재 에디터가 프로젝트를 점유하므로 batchmode Play Mode/빌드를 동시에 실행하지 않는다. 과거 검증 로그와 이번 실행 증거를 구분한다.

## Risks & Rollback
- **Risks:** SDK 교체 후 원격 실행 및 실제 게임 회귀 TC 결과는 별도 검증이 필요하다. TC 등록은 실행 성공을 뜻하지 않는다.
- **Rollback steps:** 이번 커밋을 revert한다. 에디터 가드 제거 시 cs와 meta를 함께 되돌린다. 기존 미커밋 에셋과 서브모듈은 보존한다.

## Open Questions
- 없음. 이슈/PR 생성은 별도 승인 없이는 진행하지 않는다.
