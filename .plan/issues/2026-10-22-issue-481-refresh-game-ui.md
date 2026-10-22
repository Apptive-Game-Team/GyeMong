# 2026-10-22 — Figma 기반 게임 UI 개편

- Date: 2026-10-22
- GitHub Issue: #481
- Status: UI 및 한영 번역 구현; Unity 렌더 및 타이틀/스토리 Play Mode 검증 통과

## Goal
Figma 좌우 대화창(36:9), 튜토리얼(60:21), 전투 HUD(87:2)의 여백·위계·색상을 Unity UI에 적용한다. 사용자 정정에 따라 화자는 왼쪽, 본문은 오른쪽으로 표시한다. 화자가 없는 대사는 이름 칸을 숨기고 본문 영역을 확장한다. 이후 한국어·영어 전환과 선택 저장을 추가한다.

## Non-goals
월드맵, 로그인, 전투 밸런스, 기존 로딩 수정 변경.

## Context / Constraints
Unity 2022.3.34f1 / uGUI / TMP. 배경과 캐릭터는 실제 게임 콘텐츠를 계속 사용한다.
기존 변경 및 아트 서브모듈 GUID를 보존한다. 선행 PR #480이 병합된 codex/artel-combat-qa에서 feat/481을 분기했다. 검증 당시 fix/479 HEAD와 기준 브랜치의 트리가 동일함을 확인했다. PR은 codex/artel-combat-qa를 대상으로 하여 UI 변경만 포함한다.

## Approach (Checklist)
- [x] **Step 0: Recon** Figma와 공용 프리팹/컨트롤러 확인.
- [x] **Step 1: Implementation** ChatCanvas, PopupWindow, InGameManager, HpBarCanvas 레이아웃 정리. 기존 이벤트 연결 유지. 여름 씬 4곳의 팝업 X 위치 override를 0으로 수정.
- [x] **Step 1a: Correction** Frame 36:9 기준 사각 검정 패널/흰 테두리와 화자 좌측·본문 우측 배치. None 및 미등록 화자 이름 숨김, 본문 확장, 빈 얼굴 목록 안전 처리.
- [x] **Step 1b: Localization** 원문 대사 149종, 화자, 튜토리얼, 설정/키/음량, 타이틀 버튼, 게임오버 번역. Resources JSON, 한국어 fallback, 언어 버튼과 PlayerPrefs 저장. 진행 중 대사/팝업 언어 갱신 및 누적 대사 페이지 넘김. 원본 한국어 에셋 보존.
- [x] **Step 2: Tests** YAML 내부 참조 검사, Unity 컴파일, 16:9 및 16:10 레이아웃 확인.
- [ ] **Step 3: Rollout / Rollback** UI 관련 변경만 검토·커밋. 기존 작업은 제외.

## Validation
- **Commands to run:** git diff --check; Unity 2022.3.34f1 batchmode compile.
- **Expected output:** 참조 누락 및 컴파일 오류 없음. 대화와 팝업 열기/닫기, 체력/보호막 표시 유지.
- **Passed:** 변경 프리팹 4개의 fileID 중복/내부 참조 검사, 변경 파일 `git diff --check`.
- **Passed:** Unity 2022.3.34f1 Roslyn 및 기존 Bee `Assembly-CSharp.rsp` / `Assembly-CSharp-Editor.rsp`로 런타임과 에디터 코드 컴파일. 출력만 `Logs/ui-design/`로 변경하여 열린 에디터의 어셈블리를 덮어쓰지 않음. 두 컴파일 모두 exit 0.
- **Passed:** 사용자 저장/종료 승인 후 기존 에디터를 정상 종료. Unity 배치 렌더 최종 실행 exit 0 및 UI_DESIGN_VALIDATION_PASS 확인. 대화창·튜토리얼·보스 바 1920×1080 / 1920×1200, 플레이어 HUD 1920×1080, 게임오버 1920×1200 스크린샷 8장 확인. 보스 체력 65%의 실제 RectTransform 폭 검사 통과. 프리팹 Missing Script 없음. 로그: Logs/ui-design-final.log. 검증 이미지는 게임 배경을 제외한 UI 단독 미리보기이며 실제 Play Mode 조작/씬 왕복은 미실행.
- **Reproduce:** 에디터 종료 후 Unity 2022.3.34f1에 `-batchmode -force-d3d11 -disableManagedDebugger -projectPath <repo> -executeMethod UiDesignValidation.Run -quit -logFile Logs/ui-design-validation.log` 전달. 스크린샷은 `Logs/ui-design/`에 생성. 실제 게임의 대화, 팝업, 체력·보호막 및 씬 왕복은 별도 Play Mode 검증 필요.
- **Correction validation:** Logs/ui-design-horizontal.log exit 0 / UI_DESIGN_VALIDATION_PASS. 화자→None→화자 전환 시 위치 복원, 실제 게임 배경/캐릭터로 좌우 대화창 및 내레이션 렌더 확인.
- **Localization validation:** Logs/ui-localization.log UI_DESIGN_VALIDATION_PASS. 대사 149종 번역 누락 검사, 두 언어의 단일 대사 높이 검사, 16:9·16:10 렌더 검증. 영어 설정/튜토리얼/대화 및 한국어 게임오버 이미지 확인.
- **Implementation:** 번역은 표시 시점에 적용한다. 씬 로드 시 기존 TMP 텍스트를 바인딩하고 런타임 팝업은 명시적으로 바인딩한다. 설정의 이미지 글자는 TMP로 교체하고 타이틀은 한국어 이미지 원본과 영어 텍스트를 전환한다. 추가 패키지/아트 서브모듈 변경 없음.
- **Play Mode passed:** Logs/ui-playmode-final.log / UI_PLAYMODE_VALIDATION_PASS. 실제 설정 열기 경로로 버튼을 눌러 영어 변경/PlayerPrefs 저장 확인. Title → Cinematic → Title → Cinematic 왕복에서 영어 화자, None 이름 숨김/본문 확장/기존 위치 복원을 확인. 테스트 후 기존 언어 설정 복원. 에디터용 CartoonFX.CFXR_WelcomeScreen.CreateGUI의 기존 NullReferenceException이 별도로 기록됨.
- **Final checks:** 번역 카탈로그 236개 키 중복 없음, 양쪽 언어 값 누락 없음. 4개 변경 프리팹 내부 fileID 참조 검사와 변경 소스 diff --check 통과. 키/음량 메뉴 렌더 추가, 한글 키 이름은 자동 축소로 한 줄 표시. 게임오버 문자를 흰색으로 변경해 어두운 배경 가독성 확보.
- **Remaining validation:** 실제 전투 중 HUD/보스 보호막 조작, WebGL 빌드는 미실행. 이번 Play Mode 검증은 타이틀/스토리 및 설정에 한정한다. UI 변경만 feat/481에 커밋하여 Draft PR로 게시한다. 기존 렌더 설정·폰트·아틀라스·아트 서브모듈 변경은 제외한다.

## Risks & Rollback
- **Risks:** 씬의 프리팹 override가 새 배치를 덮어쓸 수 있음. 긴 대사와 게임오버 UI의 부모 변경 영향 확인 필요.
- **Review:** 씬 override 검색 완료. 여름 4개 씬의 Window X=300 override만 수정. GameOverUI를 캔버스 직속으로 이동해 HUD 앵커에 의존하지 않도록 수정. 직렬화 참조는 유지했고 16:10 중앙 배치를 렌더로 확인. 부모-자식 참조 상호 일치 검사 통과.
- **Rollback steps:** 이번 변경 파일만 원복. 서브모듈 핀과 기존 작업은 유지.

## Open Questions
- 기존 배경과 캐릭터는 실제 게임 데이터로 표시한다. Frame 3의 사각 검정 패널/흰 테두리는 uGUI로 표현한다.
- 한영 전환: 설정에서 언어 선택, PlayerPrefs 저장, UI/튜토리얼/화자/대사 번역. 한국어 원문 유지 및 번역 누락 시 한국어 fallback.
