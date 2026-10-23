# 2026-10-23 — 스토리와 튜토리얼 넘기기 입력 통일

- Date: 2026-10-23
- GitHub Issue: #487
- Status: Implemented; Play Mode validation pending

## Goal
대사 출력 완료/다음 대사, 스킵 가능한 대기, 튜토리얼 닫기와 게임 오버 재시작을 아무 키(마우스 버튼 포함)로 통일한다.

## Non-goals
전체 스토리 이벤트 취소, 전투 튜토리얼 목표 생략, 일반 오브젝트 상호작용 변경.

## Context / Constraints
Unity 2022.3.34f1 레거시 입력. 기존 입력 버퍼는 1초간 유지된다. 일시정지 팝업은 unscaledTime을 사용한다.
기존 refact/483의 8개 커밋 및 사용자 변경분에 의존하는 작업 트리이므로 현재 HEAD에서 feat/487로 분기한다. 기존 변경분은 커밋하지 않는다.

## Approach (Checklist)
- [x] **Step 0: Recon** — ChatController, SkippableDelayEvent, SkippablePopupWindowEvent 및 번역 확인.
- [x] **Step 1: Implementation** — InputManager에 현재 프레임 입력 소비 메서드 추가. 시작 프레임/중복 소비 방지, 액션 버퍼 제거. 타이핑은 매 프레임 입력을 확인한다. 한/영 안내 갱신.
- [ ] **Step 2: Tests** — 컴파일, 차이 검토. 입력 1회/유지/연타, 타이핑 완료와 다음 대사 분리, 팝업 timeout/일시정지 복원/씬 전환 확인.
- [ ] **Step 3: Rollout / Rollback** — 변경 파일만 커밋. 실행 검증이 남으면 draft PR.

## Validation
- **Commands to run:** git diff --check; 설치된 Unity 2022.3.34f1 batchmode 컴파일.
- **Expected output:** 새 컴파일 오류 없음. 키 1회당 한 단계 진행, 누른 채 유지 시 자동 반복 없음. 전환 커튼이 덮인 동안 진행 없음.

## Risks & Rollback
- **Risks:** Unity 프레임 실행 순서에 따른 입력 재사용. 현재 프레임 소비 표식과 버퍼 초기화로 방어한다. 실제 물리 입력은 Play Mode 확인 필요.
- **Rollback steps:** 이 작업 커밋만 revert.

## Open Questions
- 없음. 스킵은 현재 대사/안내 진행으로 해석하며 전체 이벤트 체인 취소는 포함하지 않는다.

## Verification Results
- 변경 파일의 git diff --check 통과, 번역 JSON 파싱 통과.
- 최초 Unity 2022.3.34f1 배치 컴파일/종료 성공.
- 팝업 입력 격리 보완 후 1차 실행은 다른 Unity 인스턴스의 프로젝트 잠금으로 중단.
- 재실행은 스크립트 컴파일을 완료했고 error CS 없음. 공유 체크아웃에서 다른 작업과 충돌하지 않도록 assembly reload 이후 이 작업의 프로세스만 종료했다. 최종 에디터 전체 실행 성공으로 간주하지 않는다.
- 실제 키/클릭, 타이핑과 다음 대사 분리, 튜토리얼 timeout 및 씬 왕복 Play Mode 검증은 미실행. Draft PR로 유지.
- 공유 체크아웃의 소유 파일 6개를 C:/Users/jys09/.codex/worktrees/any-key-advance/GyeMong으로 이관하고 해시 일치를 검증했다. 공유 폴더의 해당 6개 변경만 복원/제거했으며 다른 작업 파일은 건드리지 않았다.
- 팝업 동안 각 액션 활성 상태를 저장하고 입력을 비활성화한다. finally에서 버퍼를 지우고 원래 상태를 복원하여 Escape가 설정창을 함께 열거나 Enter가 다음 화면까지 진행하는 것을 방지한다.
- 사용자 후속 요청으로 게임 오버 재시작도 같은 입력 소비 메서드로 통일했다. 기존 2초 대기는 유지하고 화면 표시 프레임의 입력은 무시한다. 두 번역 키의 한/영 문구도 변경했다. 이 추가 변경의 Unity 컴파일 및 실제 재시작 검증은 미실행.
