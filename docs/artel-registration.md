# ARTEL 등록 기록

2026-10-30, stage 환경의 `gyemong` 프로젝트(ID 10).

## 현재 구성

사용자 요청에 따라 기존 TC 42개(ID 1411–1449, 1490–1492)를 모두 삭제하고 게임 QA TC 20개(ID 1493–1512)로 교체했다.

- TC 폴더: 기존 `Artel 전투 QA` (ID 4)
- 전체 시나리오: ID 46, `게임 QA (20개, 치트·행동 자유 사용)`
- 원본: [artel-test-cases.json](artel-test-cases.json)
- 읽기용 목록: [artel-test-cases.md](artel-test-cases.md)
- 시나리오 단계: [artel-scenario-steps.json](artel-scenario-steps.json)

모든 TC와 시나리오 단계에서 등록된 치트·행동을 자유롭게 사용한다. 스테이지 이동·해금·회복·자원 보충·피해·처치 사용에 제한을 두지 않는다. 게임 함수 카탈로그의 처치·진행·공격 설명에 있던 테스트 용도 제한 문구도 제거했다.

## TC 범위

| ID | 항목 |
|---|---|
| 1493 | Slime 진입과 전투 화면 |
| 1494 | 접근과 공격 |
| 1495 | 연속 공격·콤보 |
| 1496 | 스킬 |
| 1497 | 플레이어 회복 |
| 1498 | 자원·적 HP HUD |
| 1499 | 슬라임 페이즈·분열 적 정리 |
| 1500 | 도주·대시 |
| 1501 | 거리 유지·스킬 |
| 1502 | 선회 |
| 1503 | 후퇴·회복 |
| 1504 | 행동 교체·중단 |
| 1505 | 옵션 메뉴와 전투 재개 |
| 1506 | 해금·씬 왕복 |
| 1507–1512 | Elf, Shadow, Golem, NagaRogue, NagaWarrior, Wanderer 전투 진행 |

## 검증

CLI로 기존 TC 삭제 후 프로젝트 목록이 0개임을 확인하고 새 TC를 생성했다. 신규 20개를 각각 `case show`로 재조회해 scene/step/precondition/expectedValue/folderId가 원본과 일치함을 검사했다. 최종 목록 20개에 삭제한 ID가 없고, 시나리오 46의 20개 단계가 신규 ID·행동·자유 사용 안내와 일치함을 확인했다.

TC 상태는 모두 DRAFT이며 이번 작업에서 게임 QA를 실행하지 않았다. 전투 동작 결과는 신규 TC 실행으로 확인해야 한다.

## 백업과 복원

삭제 전 서버 TC: `Logs/artel-cases-before-reset.json`. 시나리오: `Logs/artel-scenario-46-before-reset.json`. 기존 로컬 원본: `Logs/artel-tc-reset-backup/`. 실행 응답과 재조회 결과도 Logs에 있으며 Git에는 포함하지 않는다. 과거 문서는 Git 이력에도 남아 있다.

TC 복원이 필요하면 백업의 scene/step/precondition/expectedValue/folderId로 다시 생성하고 반환된 새 ID를 시나리오에 연결한다. 삭제한 ID 자체는 재사용하지 않는다.

## CLI

배포된 `@project-artel/cli@alpha`(0.0.1-alpha.1)에 폴더 명령이 없어 기존 CLI 소스의 빌드 복사본 `Logs/artel-cli-source/dist/cli.js`를 사용했다. 인증은 ARTEL_TOKEN 환경변수로 전달했다. 등록된 TC를 다시 `case create`하면 중복되므로 변경 시 조회·수정을 사용한다.
