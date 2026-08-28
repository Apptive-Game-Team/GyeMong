# Testing Workflow

## Why

Validation depth should match behavior risk and blast radius.

## Reality of This Project

이 리포에는 **프로젝트용 테스트 어셈블리가 없다.** `com.unity.test-framework`
패키지는 설치되어 있지만 `.asmdef` 로 정의된 테스트는 서드파티 패키지 것뿐이다.
CLI 린터·포매터도 없다.

따라서 검증은 다음 세 가지로 이루어진다:

1. **컴파일 확인** — Unity 에디터에서 콘솔 에러 0건
2. **Play Mode 수동 검증** — 변경된 동작을 실제로 재생해서 확인
3. **CI 빌드** — `main` push 시 WebGL 빌드가 통과

없는 테스트 명령을 지어내지 않는다.

## Strategy

1. 변경 대상 씬을 특정한다. 대부분 `Assets/Scenes/` 하위 13개 등록 씬 중 하나다.
2. 현재 실패를 먼저 재현하거나 기준 동작을 기록한다.
3. 최소 범위로 구현한다.
4. Unity 에디터에서 컴파일 에러·경고를 확인한다.
5. 해당 씬을 Play Mode 로 실행해 변경 동작을 확인한다.
6. 씬 전환이 얽힌 변경은 **씬을 최소 2회 이상 오가며** 확인한다. 이 프로젝트는
   싱글턴·리스너가 씬 간 유지되므로 첫 진입에서만 되는 버그가 흔하다.
7. 공유 계약(리스너, 싱글턴, `SceneContext`)을 건드렸으면 영향받는 HUD 전체를
   확인한다.

## Risk-Based Coverage

- Low risk (문서, 주석, 단일 상수): 컴파일 확인
- Medium risk (한 시스템 내 동작 변경): 해당 씬 Play Mode 검증
- High risk (플레이어/전투/씬 전환/서브모듈 핀/CI): 씬 전환 왕복 검증 + CI 빌드
  통과 확인 + 롤백 방법 명시

## Unity-Specific Checks

프리팹·씬 YAML을 직접 편집했으면 반드시:
- 제거한 오브젝트의 `fileID` 를 가리키는 잔여 참조가 없는지 확인
- 삭제한 스크립트의 `guid` 가 어떤 씬·프리팹에도 남아있지 않은지 확인
- 에디터에서 해당 프리팹을 열어 missing script 경고가 없는지 확인

에셋을 삭제했으면:
- `.meta` 형제 파일도 함께 지웠는지 확인
- 빈 폴더의 고아 `.meta` 가 남지 않았는지 확인

## Rules

- Test observable behavior, not private implementation details.
- 검증 명령을 지어내지 않는다. 프로젝트 설정에서 확인된 것만 쓴다.
- 실행하지 않은 검증을 했다고 보고하지 않는다.
- 테스트 어셈블리가 도입되면 이 문서를 갱신한다.

## Reporting

PR과 핸드오프에는 아래를 적는다:
- 실행한 검증 (어느 씬, 어떤 조작)
- 결과
- 건너뛴 검증과 이유
- 남은 리스크
