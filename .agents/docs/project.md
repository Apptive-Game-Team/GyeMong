# Project Context

Verified against repository state on 2026-08-28. Agents must re-verify commands
against repository configuration before running them.

## Overview

- Product: **계몽 (GyeMong)** — 2D 탑다운 액션 RPG
- Primary users: 플레이어 (데스크톱/웹 브라우저)
- Core domain: 계절별 스테이지 진행, 보스 패턴 전투, 이벤트 연출
- Runtime environment: **Unity 2022.3.34f1**, URP 14.0.11, 2D Feature 2.0.1

## Architecture

- Entry point: `Assets/Scenes/TitleScene.unity` (빌드 세팅 0번). 총 13개 씬 등록.
- Main modules (`Assets/Scripts/GyeMong/`):
  - `GameSystem/` — 플레이어, 몹/보스 상태머신, 공격, 맵/포탈
  - `UISystem/` — 인게임 HUD, 옵션, 타이틀, 소셜(로그인/마이페이지)
  - `EventSystem/` — 컷신·대사 등 이벤트 연출
  - `SoundSystem/` — BGM/SFX 재생, 오브젝트 풀
  - `InputSystem/` — 키 바인딩 (`InputManager`, `ActionCode`)
  - `DataSystem/` — `DataManager`, `DataBase`, `KeyValue`
  - `Assets/Scripts/Util/` — `SingletonObject`, `ChangeListener`, `ObjectPool`
- Dependency direction: `UISystem` → `GameSystem` (리스너 등록). 역방향 참조 없음.
- External systems: `Assets/Scripts/GyeMong/UISystem/Social/` 이 `UnityWebRequest`
  로 로그인/회원가입 서버와 통신.
- Persistent data: `PlayerPrefs` (스테이지 진행도 등). 세이브 파일 포맷 없음.
- Art/audio: `Assets/GyeMong_Art` **Git 서브모듈** (`Apptive-Game-Team/GyeMong_Art`).

## Commands

이 프로젝트에는 CLI 테스트/린트 파이프라인이 없다. 아래가 검증된 전부다.

| Purpose | Command |
|---|---|
| Clone | `git clone --recurse-submodules https://github.com/Apptive-Game-Team/GyeMong.git` |
| Sync submodule | `git submodule update --init --recursive` |
| Run locally | Unity Hub → 2022.3.34f1 로 프로젝트 열기 → `TitleScene` 재생 |
| Format | TODO (설정 없음) |
| Lint | TODO (설정 없음) |
| Type-check | Unity 에디터 컴파일 (별도 명령 없음) |
| Unit tests | TODO — 프로젝트 테스트 어셈블리 없음 (`.agents/docs/testing.md` 참고) |
| Integration tests | TODO — 없음 |
| Build (WebGL) | `.github/workflows/deploy.yml` — `main` push 시 자동, GitHub Pages 배포 |
| Build (데스크톱) | `.github/workflows/release.yml` — 릴리스 발행 시 macOS/Windows/WebGL |

CI는 `game-ci/unity-builder@v4` 를 쓰고 `UNITY_LICENSE` / `UNITY_EMAIL` /
`UNITY_PASSWORD` / `GYE_MONG_PLAY_TOKEN` 시크릿에 의존한다.

## Constraints

- Supported platforms: WebGL (GitHub Pages), Windows x64, macOS
- Compatibility: Unity 버전 고정 (2022.3.34f1). CI 워크플로 3곳에 하드코딩되어
  있으므로 올릴 때 함께 바꿔야 한다.
- WebGL 빌드는 `Assets/WebGLTemplates` 의 전용 템플릿을 쓴다 (#466).
- Performance: 리포 용량이 CI 디스크 한계에 근접한다. `deploy.yml` 이
  `free-disk-space` 액션을 먼저 돌린다. 대용량 에셋 추가 시 주의.
- Security: 서버 통신 코드에 자격증명을 하드코딩하지 않는다.

## Ownership

- Maintainers: Monolong, Gimlocal, YunseongJeong, Jinwook700, hwanginseop
- Designers: cheese1006, choiyangjin1
- Sensitive modules:
  - `Assets/Scripts/Util/ChangeListener/` — 모든 HUD가 의존. 변경 시 전 UI 영향.
  - `Assets/Scripts/GyeMong/GameSystem/Creature/Player/PlayerCharacter.cs` — 전투 밸런스 직결
  - `.github/workflows/` — 릴리스·배포 파이프라인
  - `Assets/GyeMong_Art` 서브모듈 핀 — 바꾸면 전원 재동기화 필요
- Changes requiring explicit review: 서브모듈 핀 변경, CI 워크플로, 히스토리 재작성
