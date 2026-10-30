# 2026-10-23 — Play Mode 중 assembly reload 지연 적용

- Date: 2026-10-23
- GitHub Issue: None
- Status: Implemented; isolated Unity validation passed

## Goal
Play Mode 중 재컴파일 후 게임과 SDK의 초기화 상태가 사라져 발생하는 NullReferenceException 방지.

## Non-goals
SDK 버전 교체, 전체 게임의 hot reload 지원, 기존 작업 변경.

## Context / Constraints
Editor.log 6586행에서 재컴파일과 assembly reload가 시작되고 6700행부터 ArtelOverlayController, ArtelManager, StageSelectController 오류가 반복된다. Awake에서 만든 비직렬화 객체가 reload 후 없어지는 공통 원인이다. reload 도중 WebRTC dispose 오류도 있다. 세 곳의 null 검사만으로는 실행 상태를 복구할 수 없다.

## Approach (Checklist)
- [x] **Step 0: Recon** 설치된 SDK dc00688ab2와 Unity 로그 확인.
- [x] **Step 1: Implementation** Editor 전용 guard가 EnteredPlayMode에서 assembly reload를 잠그고 EnteredEditMode에서 해제한다. 진입 시 domain reload와 전역 EditorPrefs는 변경하지 않는다.
- [x] **Step 2: Tests** Unity 2022.3.34f1에서 재생 중 소스 변경, 종료 후 reload, 두 번째 재생 확인.
- [x] **Step 3: Rollout / Rollback** 기존 재생을 종료한 뒤 스크립트를 import하고 다시 재생하는 적용 절차 기록.

## Validation
- **Commands to run:** Unity batchmode에서 별도 검증 프로젝트로 재컴파일 및 play mode 왕복 검증.
- **Expected output:** 재생 중 assembly reload 0회, 종료 후 보류된 reload 실행, 재진입 성공.
- **Result:** 별도 프로젝트에서 guard 없이 `FAIL: assembly was reloaded during play` 재현. guard 적용 후 `PASS: no reload during play, pending reload applied after stop, second play/stop succeeded` 확인. 이 Unity 버전은 LockReloadAssemblies 동안 컴파일도 보류한다.
- **Evidence:** `C:/Users/jys09/Projects/Artel/.tmp/play-mode-reload-validation/`의 `baseline.log`, `verified.log`, `Assets/Editor/ReloadValidation.cs`. Unity가 생성한 guard의 `.meta`를 프로젝트에 함께 추가했다.
- **Not run:** GyeMong 실제 씬과 WebRTC 연결의 수동 재검증. 다른 프로젝트에서 SDK hot reload를 지원하는 수정은 포함하지 않았다.

## Risks & Rollback
- **Risks:** Play Mode 중 코드 변경은 종료 후 반영된다. 이미 상태가 깨진 실행은 재시작이 필요하다. SDK를 사용하는 다른 프로젝트의 hot reload는 이 변경 범위 밖이다.
- **Rollback steps:** Play Mode 종료 후 PlayModeAssemblyReloadGuard.cs와 Unity가 생성한 meta를 함께 제거한다.

## Open Questions
- 없음.
