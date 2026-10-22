# 2026-10-22 — Artel 전투 치트와 지속 행동

- Date: 2026-10-22
- GitHub Issue: None (자동 승인 검토가 외부 게시 거부; 로컬 초안 보존)
- Status: Implemented and locally validated; external publication not authorized

## Goal
스테이지 변경, 플레이어 회복/자원, 적 체력 N 감소/처치 치트와 지속 공격·추적·도주·거리 유지·선회·스킬·회복 행동 제공.

## Non-goals
밸런스 수정, 무적 자동 전투, 완전 경로 탐색, 실제 입력 장치 테스트 대체.

## Context / Constraints
Unity 2022.3.34f1, SDK 0.2.0. 기존 설치 변경 보존. 전체 QA 파일에 ARTEL_GAME_FUNCTIONS 및 개발 빌드 가드. 입력 관리자 전역 변경 없이 플레이어의 기존 전투 처리 재사용. 씬/프리팹 변경 없음.

## Approach (Checklist)
- [x] **Step 0: Recon** 플레이어 입력, 보스 피해/페이즈, StageInfo 진입, 공식 행동 생명주기 확인.
- [x] **Step 1: Implementation** 플레이어 partial QA 입력/자원 브리지, 치트 6개·행동 7개 카탈로그, 사용법. 독립 크리틱 리뷰 PASS.
- [x] **Step 2: Tests** Unity 컴파일/카탈로그 13개 확인 및 Play Mode 검증 PASS. Windows Development Build 성공(exit 0), 빌드 DLL에도 13개 등록 확인.
- [x] **Step 3: Rollout / Rollback** 미검증 항목과 롤백 기록. 사용자 요청에 따라 `codex/artel-combat-qa` 로컬 브랜치에서 관련 변경을 커밋하여 관리. GitHub 이슈/PR과 원격 게시 없음.

## Validation
- **Commands to run:** Unity batchmode compile/development build; 로컬 검증 스크립트로 게임 함수 호출 및 상태 검증; git diff --check.
- **Expected output:** 컴파일 오류 없음, 함수 등록, 입력 정리, 게임 잠금/쿨다운 유지.
- **Play Mode result:** Logs/artel-combat-validation.txt PASS. 슬라임→골렘→방랑자 이동, HP 설정/회복, 실제 연속 공격 상태, 도주 이동, 중단 입력 해제, 골렘 방패 보존 HP 감소, 방랑자 방향 방어 우회 및 처치 확인. 진행 PlayerPrefs 복원.
- **Known runtime logs:** 기존 HitCollider.hitEffect 미할당 및 CameraManager.Awake null 참조 발생. 이번 기능 상태 검증은 통과했지만 전체 콘솔 오류 0건은 아님. 서버 원격 호출/CI/WebGL 및 모든 행동의 장시간 실전 동작은 미검증.

## Risks & Rollback
- **Risks:** 다중 페이즈 보스·분열 적의 처치 완료는 기존 사망 연출 뒤 확인해야 함. 직선/벽 회피 이동은 복잡한 미로를 해결하지 않음. 스테이지 치트는 진행 저장값을 변경함.
- **Rollback steps:** 추가 QA 파일/.meta와 PlayerCharacter의 입력 훅만 되돌림. 기존 SDK 설치는 유지.

## Open Questions
- 서버 연결은 이전 턴부터 미확인. 로컬 검증과 서버 검증을 구분한다.
