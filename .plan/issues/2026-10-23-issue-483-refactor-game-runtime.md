# 2026-10-23 — 게임 시스템 및 씬 초기화 리팩토링

- Date: 2026-10-23
- GitHub Issue: #483
- Status: Implemented; draft PR pending build validation

## Goal
자체 코드 전체(262개 C# 파일)를 모듈 단위로 점검해 책임과 중복을 정리한다. 반복 배치된 실행용 프리팹을 공통 부트스트랩과 씬 초기화 코드로 대체하고 첫 로드 시 기본 UI/미초기화 카메라가 노출되는 프레임을 제거한다.

## Non-goals
서드파티·아트·맵/연출 콘텐츠 재작성, 밸런스와 세이브 형식 변경, 새 DI 프레임워크 도입. 코드 위치만 바꾸기 위한 전체 이름/네임스페이스 변경.

## Context / Constraints
- Unity 2022.3.34f1. UI PR #482의 feat/481에서 후속 refact/483 분기. 기존 미커밋 에셋 변경은 보존한다.
- 빌드 세팅은 13행/고유 씬 12개. SummerOpeningScene 중복 행은 이 작업에서 별도 변경하지 않는다.
- 대부분 씬에 GameManager/OptionController/ChatCanvas/InGameManager가 반복 배치돼 있다. StageSelect의 GameManager 설정 override는 원본과 비교한 뒤 이전한다.
- 기존 MonoBehaviour 파일/GUID/직렬화 필드/공개 호출은 유지한다. 공통 서비스에 대한 씬 내부 직렬화 참조는 삭제 전 검사한다.
- Hindsight 조회/이니셔티브 기록은 ENOTFOUND home으로 사용 불가. 코드와 씬을 직접 확인했다.

## Approach (Checklist)
- [x] **Step 0: Recon** 모듈 크기·공통 프리팹·씬 로드 경로·override 조사.
- [x] **Step 1: Implementation — scenes** 공통 게임/설정 초기화(기존 씬 EventSystem 유지, 없을 때 생성), 씬별 UI 설치, 준비 전 화면 가림과 통일된 로드 경로. 씬 전환/직접 씬 실행 모두 지원. 에디터 마이그레이션은 참조 검사 후 공통 서비스 인스턴스를 제거하고 설치 설정만 남긴다. 카메라/플레이어/월드 배치는 유지한다.
- [x] **Step 1: Implementation — modules** 입력 초기화/키 상태 책임, 데이터 직렬화·암호화 책임, 사운드 초기화 중복, 대화 데이터 조회와 화면 책임, UI 텍스트 생성 중복을 정리한다. 플레이어 클래스는 직렬화 계약을 유지하는 partial 파일로 응집된 메서드를 분리한다. 공통 객체/리스너 생명주기는 실제 사용처를 함께 수정한다.
- [x] **Step 2: Tests** 컴파일, 씬 마이그레이션 참조 검사, 고유 빌드 씬 설치 구성 검사, 타이틀/스토리/스테이지/전투 씬 왕복, 첫 프레임 가림/기본 UI 비노출, 기존 한영 UI 렌더 회귀 검사. 이동한 전투 메서드는 본문 동일성 검사.
- [ ] **Step 3: Rollout / Rollback** 책임별 커밋 후 UI PR과 별도 후속 PR 게시. 실제 실행하지 못한 검증은 명시한다.

## Validation
- **Commands to run:** Unity batchmode migration/audit; 기존 UiDesignValidation.Run / UiPlayModeValidation.Run; 추가 실행 생명주기 검증; git diff --check.
- **Expected output:** 오류 없는 컴파일, 원본 씬 콘텐츠/직렬화 참조 보존, 공통 서비스 하나씩 생성, 목적지 준비 전 화면 비노출, 초기화 후 정상 화면, 기존 게임 동작과 번역 유지.

## Risks & Rollback
- **Risks:** 런타임 생성 시 Awake/Start 순서, 씬에서 서비스로 향하는 직렬화 참조, 씬별 override, 영속 객체의 오래된 씬 참조, 테스트 실행 중 진행도 변경.
- **Rollback steps:** 코드와 씬 마이그레이션을 함께 되돌린다. 테스트는 진행도/언어 키를 보존하며 새 게임의 전체 PlayerPrefs 삭제 경로를 실행하지 않는다.

## Open Questions
- 스코프는 사용자 확인으로 게임 프로젝트 전체 및 씬 로딩까지 포함한다. 미확인 씬 전용 설정은 삭제하지 않고 조사 후 적용한다.

## Final implementation and evidence
- 빌드 등록 고유 12개 중 LoadingScene을 제외한 11개 씬을 Unity API로 이전했다. 비등록 레거시/테스트 씬은 이전하지 않았다.
- StageSelect의 축소 HUD는 StageSelectRuntime variant로 보존했다. 월드/플레이어/카메라 배치와 기존 EventSystem 설정을 유지했다.
- 공통 서비스는 한 번 생성하고 HUD/대화는 씬 소유로 생성한다. 로딩 가림막, 카메라 준비, 입력 차단을 함께 적용했다. 자식 싱글턴 때문에 TitleUI 전체가 영속화되는 문제도 수정했다.
- 플레이어 Movement/Combat/Events, 저장 codec, 씬 번역 및 라벨 생성 책임을 분리했다. 사운드/풀/입력/옵션 수명과 초기화를 수정했다. 전체 보스 클래스 분리는 하지 않았다.
- runtime-play5.log: 16회 순차/왕복 시작 PASS. runtime-final.log: 씬 구성 audit, 한영 UI 24개 렌더, 11개 씬 독립 시작 및 저장된 F2 키/음량 유지 PASS. 임시 설정 파일 복원 확인.
- 플레이어 이동 메서드 본문 동일성, 삭제 씬 객체 fileID 잔여 참조 검사 PASS. 하트 30개 패널 내부 배치 캡처 확인. 독립 크리틱 PASS.
- 기존 CartoonFX 에디터 WelcomeScreen 예외만 테스트에서 명시적으로 제외한다. 기존 SpringBeach missing script는 보존한다.
- WebGL/CI 빌드와 전체 보스 전투/승리/사망/물리 포털 조작은 미검증이므로 PR은 draft로 게시한다. 기존 AES 15바이트 키 문제는 저장 형식을 바꾸지 않기 위해 별도 이슈로 분리한다.
