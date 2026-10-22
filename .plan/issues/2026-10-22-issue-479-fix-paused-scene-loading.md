# 2026-10-22 — 팝업 일시정지로 인한 로딩 멈춤 수정

- Date: 2026-10-22
- GitHub Issue: #479
- Status: Implemented; Play Mode validation pending

## Goal
팝업의 Enter 입력이 스테이지 진입으로 전달되지 않고, 일시정지 중에도 로딩이 완료된다.

## Non-goals
공용 싱글턴과 입력 시스템 전반의 재설계, 기존 사용자 에셋 변경.

## Context / Constraints
팝업은 Canvas 하위 오브젝트다. 씬 로컬 싱글턴으로 관리하여 자식 DontDestroyOnLoad 경고를 제거한다.
InputManager.GetKeyDown은 입력을 소비하므로 팝업이 열려 있는 동안 스테이지 입력을 막는다.

## Approach (Checklist)
- [x] **Step 0: Recon**: 로그와 팝업/입력/씬 전환 경로 확인.
- [x] **Step 1: Implementation**: 팝업 상태와 비활성화 시 시간 복구, 실제 시간 타이머, Enter 닫기, 스테이지 입력 차단, 로더의 실제 시간 대기와 시간 복구.
- [ ] **Step 2: Tests**: 최초 튜토리얼, Enter/Space 닫기, 자동 닫기, 일시정지 중 씬 전환, 왕복 전환 확인.
- [x] **Step 3: Rollout / Rollback**: 수정 파일만 반영하고 문제가 있으면 해당 변경만 되돌린다.

## Validation
- **Commands to run:** git diff --check; 가능한 로컬 C# 컴파일/동작 검증; Unity Play Mode.
- **Expected output:** 경고 없이 팝업이 닫히고, 별도 Enter 입력으로 다음 씬 진입. 시간 배율 0에서도 로딩 완료 후 1.
- **Results:** 독립 pair-review PASS. Unity 생성 `Library/Bee/artifacts/1900b0aE.dag/Assembly-CSharp.rsp`와 .NET SDK 8.0.426 Roslyn으로 전체 Assembly-CSharp 컴파일 성공(exit 0). 출력은 `Logs/popup-loading-validation/`에 격리했다. 기존 코드 경고가 있으며 수정 파일의 컴파일 오류는 없다.
- **Limits:** Unity batchmode는 같은 프로젝트가 이미 열려 있어 거부됨(`HandleProjectAlreadyOpenInAnotherInstance`). 기존 에디터를 종료하지 않았다. Play Mode 왕복 및 CI 빌드는 미실행.

## Risks & Rollback
- **Risks:** 씬 로컬 팝업 참조와 코루틴 취소, 입력 버퍼가 닫힌 뒤 남는 경우를 점검해야 한다.
- **Rollback steps:** 이번 작업의 코드 변경만 되돌린다. 기존 사용자 변경은 보존한다.

## Open Questions
- Play Mode 왕복 검증 필요. PR 대상: codex/artel-combat-qa, 작업 브랜치: fix/479.
