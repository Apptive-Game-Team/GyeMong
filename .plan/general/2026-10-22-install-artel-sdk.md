# 2026-10-22 — Artel SDK 및 프로젝트 치트 스킬 설치

- Date: 2026-10-22
- GitHub Issue: None
- Status: Installed; account connection pending

## Goal
현재 Unity 프로젝트에 공식 Artel SDK와 프로젝트 범위의 artel-cheats 스킬을 설치한다.

## Non-goals
게임 치트 구현, CLI/MCP 설치, 전역 스킬 설치, GitHub 게시.

## Context / Constraints
Unity 2022.3.34f1. 기존 ProjectSettings.asset 수정은 보존한다.
SDK는 에디터/개발 빌드에서 자동 부트스트랩하므로 씬 수정이 필요 없다.
사용자 로그인과 프로젝트 선택은 실행 시 확인한다.

## Approach (Checklist)
- [x] **Step 0: Recon** 공식 안내, 패키지 호환성, develop의 치트 지원 파일 확인.
- [x] **Step 1: Implementation** manifest에 공식 Git URL 추가; .agents/skills/artel-cheats 및 참조 파일 4개 설치.
- [x] **Step 2: Tests** Unity 패키지 해석 및 컴파일 성공(exit 0). SDK 0.2.0, lock hash e0e05b1a00194388907731d04c6b415d9b313d6f. SDK가 ARTEL_GAME_FUNCTIONS 심볼 자동 추가. JSON 및 git diff --check 통과.
- [ ] **Step 3: Rollout / Rollback** 사용자가 Unity Play Mode에서 로그인하고 프로젝트를 선택해야 연결 확인 가능. 런타임 연결은 미검증.

## Validation
- **Commands to run:** Unity 2022.3.34f1 batchmode import/compile, manifest JSON parse, git diff --check.
- **Expected output:** SDK 패키지 해석 성공, 컴파일 오류 없음, 스킬 참조 파일 존재.
- **Result:** Logs/artel-sdk-setup.log에 Exiting batchmode successfully now 및 return code 0 확인. 기존 코드 경고와 SDK 분석 경고는 있음.

## Risks & Rollback
- **Risks:** 네트워크/라이선스 또는 기존 프로젝트 오류로 Unity 검증이 차단될 수 있다. 최초 연결에는 사용자 계정 로그인이 필요하다.
- **Rollback steps:** 추가한 manifest 항목 및 스킬 폴더를 제거하고 Unity가 lockfile을 재생성하도록 한다. SDK가 추가한 ARTEL_GAME_FUNCTIONS 심볼이 남았는지 확인한다.

## Open Questions
- 실행 시 연결할 Artel 프로젝트는 로그인 후 선택한다.
