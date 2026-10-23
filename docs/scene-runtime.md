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

결과는 지정한 logFile과 Logs/runtime-scenes에 남는다. RunFinal은 Assets/Database의 키/음량 파일을 잠시 대체한 뒤 복원한다. 백업은 Unity SessionState에 있으므로 **원본 파일을 별도로 백업하고 실행하며 프로세스를 강제 종료하지 않는다**. 진행도와 언어 키도 정상 종료 시 복원한다. 기존 CartoonFX WelcomeScreen 에디터 예외만 제외하며 게임 런타임 오류는 실패로 처리한다.

## 범위와 롤백

LoadingScene을 제외한 빌드 등록 11개 씬을 이전했다. 비등록 테스트/레거시 씬과 SpringBeach의 기존 missing script는 변경하지 않았다. 마이그레이션은 외부 참조와 지원하지 않는 프리팹 override가 있으면 저장 전에 실패한다. 되돌릴 때는 씬, StageSelect variant 및 런타임 코드 변경을 함께 되돌린다.
