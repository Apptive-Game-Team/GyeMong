# 2026-10-23 — 런타임 후속 품질 검증과 결함 수정

- Date: 2026-10-23
- GitHub Issue: #483, #484, #489
- Status: Implemented and validated; PR #490

## Goal
기존 리팩토링 이후 남은 저장·전투·씬 결함을 근거 기반으로 수정하고 실제 WebGL 빌드까지 검증한다. 문제가 없다는 근거 없이 전체 완벽을 선언하지 않는다.

## Non-goals
전투 밸런스 재설계, 아트 서브모듈 수정, 쓰지 않는 씬 일괄 삭제, 배포 실행.

## Context / Constraints
PR #485 병합 이후 후속 수정은 fix/489에서 별도 PR로 진행한다. 최신 통합 브랜치 codex/artel-combat-qa의 #482/#485/#488 변경을 보존했다. 관련 없는 dirty 에셋은 보존한다. Hindsight는 ENOTFOUND로 조회 불가. 기존 15바이트 AES 키로는 유효한 암호문을 만들 수 없으므로 없는 레거시 암호문 호환을 주장하지 않는다.

## Approach (Checklist)
- [x] **Step 0: Recon** 저장 경로·키·오류 처리와 빌드 모듈 확인
- [x] **Step 1: Implementation** codec을 버전 prefix와 SHA256 파생 키 및 랜덤 IV로 수정. 평문 JSON은 읽기 호환. 손상 데이터는 원본을 보존하고 기본값으로 로드. 에디터 폴더 생성과 유효 키 항목 검증. 보스/플레이어는 독립 감사로 확인한 결함만 수정. 씬 missing script는 원인 식별 후 데이터 없는 컴포넌트만 제거.
- [x] **Step 2: Tests** 임시 디렉터리에서 codec/저장 왕복과 손상·빈 값·유효하지 않은 키 검사. 전투 회귀 검사. 씬 audit과 반복 로드. 배포 없는 로컬 WebGL 빌드와 브라우저 smoke.
- [x] **Step 3: Rollout / Rollback** 후속 커밋과 PR #490 게시. 새 저장 포맷 적용 후 롤백은 데이터 백업과 함께 수행.

## Validation
- **Commands to run:** Unity batchmode의 프로젝트 전용 검증/BuildPipeline 진입점
- **Expected output:** 저장 왕복·오류 복구·씬 전환 PASS와 WebGL BuildReport Succeeded. 미실행 항목은 명시.

## Risks & Rollback
- **Risks:** 클라이언트 내 키는 보안 비밀이 아니며 암호화는 변조 방지를 보장하지 않는다. 새 포맷은 구버전 앱에서 읽히지 않는다. 기존 설정을 테스트로 덮어쓰지 않는다.
- **Rollback steps:** 신규 저장은 별도 백업 후 코드 롤백. 씬·런타임 수정은 함께 되돌린다.

## Open Questions
- 전투 고정 피해 1은 의도일 수 있으므로 그대로 유지한다. 실증되지 않은 밸런스 수정은 하지 않는다.

## Findings and implementation
- 저장: 15바이트 AES 키 오류를 SHA256 파생 키·버전 prefix·랜덤 IV로 수정. 기존 평문 JSON 읽기 호환, 손상 원본 보존 및 기본값 복구, 잘못된 키 항목 무시. 신규 에디터 저장 디렉터리 자동 생성.
- 플레이어: 보호막 초과 흡수 시 HP 중복 차감 수정, 이동 잠금의 이전 입력/속도 초기화, 대시/공격 tween의 오브젝트 수명 연결.
- 상태머신: 종료/핸들 해제를 공통화하고 스턴 Update와 중복 Exit를 차단. 첫 yield 이전 내부 전환의 코루틴 핸들 덮어쓰기도 방지. 파생 보스·ShadowOfHero까지 적용.
- HUD: 중복 리스너 등록 방지, 해제 API, 콜백 중 추가/제거 정책과 HUD 파괴 시 해제.
- 진행도: 시네마틱 종료 시 언어/로그인 설정 보존, 해금 진행도 단조 증가 및 범위 보정, StageManager 동기 코루틴 완료 플래그 고착/중복 클리어 수정.
- 씬 취소: SceneLoader가 외부 전환 수락 시 입장/클리어 대화와 반복음을 취소. 자체 포탈/클리어 전환은 사전 핸들 해제로 구분.
- 씬 자산: SpringBeach의 데이터 없는 missing MonoBehaviour 1030449218과 해당 component 참조만 제거. 정상 Light2D 유지. 23개 씬 GUID 정적 감사는 Unity 내장 스크립트 참조 외 누락 없음.
- 검증 설정 백업: SessionState만 쓰던 파일 백업을 Library의 디스크 manifest로 변경, 비정상 종료 복구 진입점 제공.

## Evidence so far
- runtime-data-quality.log: codec Unicode 왕복·랜덤 IV·평문 호환·손상 복구 PASS.
- runtime-combat-quality2.log: 보호막 0/.5/1/2, 이동 잠금, 리스너 변경, 스턴 Update/Exit, 가중치 fallback, 동기 상태 전환과 실제 Slime/Golem/Wanderer 경로 PASS.
- runtime-webgl-quality.log: 저장 및 진행도 검사 PASS 후 ARTEL SDK WebRTC 참조 CS0234/CS0246로 WebGL 빌드 실패. 게임 이슈 #489. 사용자 승인으로 ARTEL 작업에 수정 요청 전달.
- runtime-combat-quality7.log / artel-combat-validation.txt: 최신 통합 #488과 수정 SDK를 적용한 7개 전투 씬 진입·스턴 복귀, Golem 스턴 페이즈, 입장/클리어 대화 취소·음향 정리·재입장, 레거시 Sandworm 스턴 중 사망 처리 PASS. 긴 입장 대사는 테스트 wrapper에서 배속한다. quality5/6에서 발견한 대사 제한시간 및 비동기 로드 대기 문제는 검증 코드에서 수정했다.
- ARTEL SDK #160: SDK 별도 프로젝트 WebGL release/development와 CodeGen 22/22 PASS. 게임은 구현 SHA dc00688ab2cf79d2d4e71dd2ef9fc5e838c5dfef에 고정했고 게임 QA의 WebGL 플랫폼 경계도 일치시켰다.
- runtime-final-quality.log: 씬 설치 audit, 한영 UI 24개 렌더, 11개 씬 독립 시작, 저장 키/음량 보존 PASS. Library/RuntimeSettingsFixtureBackup.json 제거와 임시 설정 파일 복원 확인. 새 캡처에서 이름/본문 배치, 무화자 내레이션, 튜토리얼 계속 안내를 확인했다.
- runtime-webgl-quality2.log: 전체 게임 WebGL release BuildResult.Succeeded, RUNTIME_WEBGL_BUILD_PASS 및 exit 0. `.wasm` 36,662,489바이트 생성. 타깃 전환 도중 이전 패키지 경로의 캐시 컴파일 오류가 출력됐지만 Unity가 갱신 후 정상 빌드 완료.
- 별도 headless Chrome smoke: 실제 빌드 타이틀→설정→음량 변경/저장→영어 전환→새로고침 후 언어·음량 유지→새 게임 스토리 진입 및 무화자 영어 대사 표시 확인. webgl-browser-console.log에서 console error/pageerror/암호화 예외 없음. sound length 미로드 로그는 남으며 모든 오디오의 청취 검증은 하지 않았다.
- 에디터 씬 ready 캡처 일부는 ARTEL 등록 overlay가 가린다. 구조/초기화 검사는 PASS이며 실제 릴리스 화면은 위 Chrome 검사로 별도 확인했다. 모든 보스 패턴/전체 클리어·사망 및 물리 포털 조작 완주는 검증 범위가 아니다.
- 독립 크리틱: 취소 경로 수정 후 코드 PASS, 실행 결과 별도 확인 필요.
- 동일 체크아웃의 #487 작업은 사용자 승인으로 별도 worktree에 이관 완료. 해당 변경은 이 PR에 포함하지 않는다.
