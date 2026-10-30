# ARTEL 등록 기록

2026-10-30, stage 환경의 `gyemong` 프로젝트(ID 10).

## 게임 함수

치트 6개와 지속 행동 7개는 기존 커밋 `e057a911` 및 후속 수정에 구현되어 있다. 사용법은 [전투 QA](artel-combat.md)를 참고한다. 이번에는 중복 구현 없이 SDK를 현재 설치된 `97c41053731d41a86dd778310811f5f756db6763`으로 고정했다.

현재 로컬 `Library/ScriptAssemblies/Assembly-CSharp.dll`에서 13개 카탈로그 이름을 확인했다. manifest와 lock의 SDK URL/해시가 일치하며 SDK 패키지에 치트·행동 attribute와 define 동기화 코드가 존재한다. 이는 서버 카탈로그 조회나 실제 호출 성공을 대신하지 않는다.

## 테스트 케이스

기존 [전투 TC 원본](artel-test-cases.json) 39개(ID 1411–1449)는 서버 목록과 scene/step/precondition/expectedValue가 모두 일치했다. 중복 등록하거나 기존 분류를 변경하지 않았다.

이번에 [회귀 TC 원본](artel-regression-cases.json) 3개를 `ARTEL 연동 회귀` 폴더(ID 6)에 CLI로 생성했다.

| ID | 항목 | 상태 |
|---|---|---|
| 1490 | 고정 SDK 연결·치트·행동 카탈로그 | DRAFT |
| 1491 | Play Mode 재컴파일 지연 및 재진입 | DRAFT |
| 1492 | 행동 교체와 전투 씬 왕복 | DRAFT |

각 ID를 `case show`로 다시 조회해 네 필드와 folderId가 원본과 일치함을 확인했다. 총 42개이며 신규 TC는 등록만 완료했고 실행하지 않았다.

배포된 `@project-artel/cli@alpha`(0.0.1-alpha.1)는 폴더 명령이 없어서 프로젝트 외부의 기존 CLI 소스를 `Logs/artel-cli-source`에 복사해 빌드하고 사용했다. 해당 복사본과 실행 응답은 Git에서 제외되는 Logs에 있다. 인증은 ARTEL_TOKEN 환경변수를 사용했다.

## 검증 한계

GyeMong 프로젝트가 Unity 에디터에서 열려 있어 별도의 batchmode Play Mode·빌드를 실행하지 않았다. 기존 검증 기록은 [전투 QA](artel-combat.md)와 [에디터 가드 계획](../.plan/general/2026-10-23-defer-play-mode-assembly-reload.md)에 있다. 현재 Editor.log에는 과거 패키지 해석 중 CS0006과 이후 재컴파일/reload 기록 및 GUI 경고가 있어 전체 콘솔 오류 0건으로 보고하지 않는다.

## 재등록 시 주의

원본 JSON을 다시 `case create`하면 중복된다. 이번에 생성한 ID 1490–1492를 조회·수정하고, 추가 생성 전 프로젝트 목록을 비교한다.
