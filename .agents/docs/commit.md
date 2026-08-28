# Commit Workflow

## Why

Each commit should explain one coherent change and remain safe to review or revert independently.

## Format

```text
<type>(<optional-scope>): <요약>
```

Types are the set defined in [`branch.md`](branch.md).

Examples from this repository:

```text
fix(webgl): 기본 Unity 템플릿 대신 전용 WebGL 템플릿 사용
ci: 릴리스 시 맥/윈도우/웹 빌드 생성 및 링크 연결
asset(sound): 여름 보스 BGM 교체
balance(elf): 활 공격 데미지 12 → 9
```

Scope는 파일명, 모듈명, 또는 시스템명 중 가장 좁게 특정되는 것을 쓴다.

## Rules

- 요약은 한국어. 50자 이내를 목표로 한다.
- 마침표로 끝내지 않는다.
- 동기가 자명하지 않으면 본문에 왜를 적는다.
- 관련 없는 동작 변경·포매팅·리팩토링을 한 커밋에 섞지 않는다.
- 시크릿, 생성물 노이즈, 로컬 전용 설정을 커밋하지 않는다.
- 에셋을 지울 때는 `.meta` 형제 파일도 같은 커밋에 포함한다.
- `Assets/GyeMong_Art` 핀 변경은 단독 커밋으로 분리한다.

## Body

Add a body when change has non-obvious constraints or tradeoffs:

```text
fix(ui): 씬 전환 후 체력바 미갱신 수정

ChangeListenerCaller 가 리스너를 해제하지 않아 파괴된 UI가 리스트에
남고, 예외가 foreach 를 끊어 뒤 리스너가 갱신되지 않았다. OnDestroy
에서 해제하도록 바꿨다.

Refs #467
```

## Merge Strategy

PR은 squash merge 된다. 최종 커밋 제목이 곧 PR 제목이므로, PR 제목도 이
형식을 따른다.
