# Branch Workflow

## Why

Predictable branch names expose intent and issue linkage without relying on local context.

## Naming

```text
<type>/<issue-number>
```

Examples from this repository:

```text
feat/449
fix/459
asset/452
refact/405
```

Allowed types (README 커밋 규칙과 동일한 집합):

| Type | 설명 |
|---|---|
| `feat` | 새로운 기능 |
| `fix` | 결함 수정 |
| `refact` | 동작 유지 구조 개선 |
| `balance` | 밸런스·수치 조정 |
| `comment` | 주석 추가/오타 수정 (동작 변경 없음) |
| `asset` | 이미지·사운드 등 게임 리소스 |
| `art` | 아트 작업 |
| `effect` | 이펙트·연출 |
| `docs` | 문서 |
| `rename` | 파일·폴더명 변경 또는 이동 |
| `remove` | 불필요한 코드·파일 삭제 |
| `perf` | 측정된 성능 개선 |
| `test` | 테스트 전용 변경 |
| `build` | 빌드·의존성 변경 |
| `ci` | 자동화 변경 |
| `chore` | 위에 해당하지 않는 유지보수 |

Issue number rules:
- digits only
- must reference the primary issue for the branch
- create or identify the issue before creating the branch

이슈 번호가 없는 소규모 작업은 `<type>/<짧은-설명>` 을 쓴다 (예: `fix/webgl-template`).
비-사소한 개발 작업은 이슈를 먼저 만든다.

## Lifecycle

- Branch from `main`.
- Keep one primary issue per branch.
- Sync with `main` before final validation when divergence matters.
- Never force-push a shared branch without coordination.
- Delete branch after merge when no follow-up work depends on it.

## Submodule Note

`Assets/GyeMong_Art` 변경이 필요한 작업은 서브모듈 리포에 별도 브랜치·PR을 만들고,
메인 리포 브랜치에서는 핀 갱신만 커밋한다. 서브모듈 핀을 바꾼 브랜치는 그 핀이
서브모듈 원격에 올라간 뒤에만 푸시한다.
