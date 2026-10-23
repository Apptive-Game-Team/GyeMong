# 씬 런타임 구성

빌드 등록 씬의 SceneRuntime 오브젝트는 SceneRuntimeInstaller를 가진다. 공통 GameManager와 OptionController는 존재하지 않을 때만 생성하고 HUD와 ChatCanvas는 씬마다 생성한다. 기존 EventSystem은 유지한다. StageSelect는 전용 StageSelectRuntime variant를 사용한다.

새 씬에서는 맵/플레이어/연출을 씬에 배치하고 설치 컴포넌트의 프리팹 필드를 지정한다. 런타임 서비스 인스턴스를 씬에 중복 배치하지 않는다. 씬 객체가 런타임 생성 서비스에 직렬화 참조를 갖지 않도록 한다. 전환은 SceneLoader.LoadScene을 통해 요청한다. 단일 씬 전환을 기준으로 검증했으며 additive 로딩은 검증하지 않았다.

SceneTransitionCurtain이 초기 렌더를 가리고 카메라와 UI 준비 후 입력과 화면을 연다. 영속성은 루트 서비스가 소유하며 자식 싱글턴이 부모 씬 UI를 영속화하면 안 된다.

## 검증

Unity 2022.3.34f1에서 프로젝트를 닫고 batchmode로 실행한다. Audit은 nographics와 quit 사용 가능하다. 렌더/Play Mode 검증에는 force-d3d11을 사용하고 quit을 전달하지 않는다(검증기가 종료한다).

- `-executeMethod SceneRuntimeMigration.Audit`: 빌드 등록 씬 설치 구성 확인
- `-executeMethod SceneRuntimeValidation.Run`: 순차 전환 및 타이틀/스토리 왕복
- `-executeMethod SceneRuntimeValidation.RunCold`: 각 씬 독립 Play Mode 시작
- `-executeMethod SceneRuntimeValidation.RunFinal`: audit + 한영 UI 렌더 + 독립 시작 + 키/음량 보존

결과는 지정한 logFile과 Logs/runtime-scenes에 남는다. RunFinal은 Assets/Database의 키/음량 파일을 잠시 대체한 뒤 복원한다. 원본 파일과 속성은 Library/RuntimeSettingsFixtureBackup.json에 먼저 백업한다. 비정상 종료 후에는 `-executeMethod RuntimeSettingsFixture.Restore`로 복구하며, 백업이 남은 상태에서는 새 fixture를 만들지 않는다. 진행도와 언어 키도 정상 종료 시 복원한다. 기존 CartoonFX WelcomeScreen 에디터 예외만 제외하며 게임 런타임 오류는 실패로 처리한다.

## 범위와 롤백

LoadingScene을 제외한 빌드 등록 11개 씬을 이전했다. 비등록 테스트/레거시 씬은 이전하지 않았다. SpringBeach의 데이터 없는 missing MonoBehaviour만 제거했고 유효 Light2D는 유지했다. 마이그레이션은 외부 참조와 지원하지 않는 프리팹 override가 있으면 저장 전에 실패한다. 되돌릴 때는 씬, StageSelect variant 및 런타임 코드 변경을 함께 되돌린다.

## 저장과 진행도

저장 codec은 `GYEMONG1:` prefix 뒤에 IV와 AES payload를 저장한다. 기존 에디터 평문 JSON도 읽으며 다음 저장 때 플랫폼 포맷을 적용한다. 기존 코드의 15바이트 키는 유효한 AES 키가 아니었으므로 해당 키로 생성된 정상 암호문 호환을 가정하지 않는다. 내장 키 기반 암호화는 클라이언트 난독화이며 보안 경계가 아니다. 손상된 파일은 로드 시 덮어쓰지 않고 기본값으로 복구한다.

스테이지 해금은 기존 진행도보다 낮아지지 않으며 마지막 노드 범위로 제한한다. 새 게임은 별도의 진행도 초기화 경로를 사용한다. 시네마틱 종료는 진행도 3개 키만 초기화하고 언어·로그인은 유지한다.

## 전투와 대화 수명

상태 전환은 StateMachineMob의 공통 종료/시작 경로를 사용한다. 스턴 중에는 이전 상태 Update가 실행되지 않는다. 파생 상태는 currentState를 직접 종료하지 않고 StopCurrentState를 사용한다. 첫 yield 전에 다음 상태로 전환해도 활성 코루틴 핸들이 유지된다.

HUD는 원래 등록한 caller에서 파괴 시 리스너를 해제한다. 알림 중 등록한 리스너는 다음 알림부터 호출하며, 알림 도중 제거된 리스너는 건너뛴다.

StageManager 입장/클리어 연출은 SceneLoader가 실행한다. 외부 씬 전환을 수락하면 진행 중 연출과 반복 대화음을 취소한다. 자체 포탈·클리어 이동은 전환 전에 자신의 핸들을 해제한다.

추가 검증 진입점:
- `RuntimeQualityValidation.RunData`: 임시 디렉터리의 저장 왕복·손상 복구 및 진행도 경계
- `ArtelCombatValidation.Run`: 전투 자원/행동, 보호막, 상태 수명, 대화 취소와 재입장, 스턴 중 페이즈·사망 경로
- `RuntimeQualityValidation.BuildWebGL`: 저장 검사 후 배포 없는 로컬 WebGL 빌드

전투 검증의 긴 스테이지 진입 대사는 테스트 wrapper에서만 배속한다. 이는 상태/전환 회귀 검사이며 정상 속도의 대사 UX 검증을 대신하지 않는다.

ARTEL SDK는 WebGL 플랫폼 경계를 수정한 커밋 `dc00688ab2cf79d2d4e71dd2ef9fc5e838c5dfef`에 고정한다. SDK PR은 project-artel/artel-sdk#160이다. WebGL 플레이어에는 WebRTC 기반 SDK를 포함하지 않고, WebGL을 대상으로 설정한 에디터에서는 SDK를 사용할 수 있다. 게임의 QA 코드도 같은 플랫폼 경계를 사용한다.
