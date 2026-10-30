# Artel 전투 QA

실행 절차·기대 결과·결과 기록은 [테스트 케이스 20개](artel-test-cases.md)를 참고한다. 모든 TC에서 등록된 치트와 행동을 자유롭게 사용할 수 있다. 스테이지 이동·해금·회복·자원 보충·피해·처치에 사용 제한을 두지 않는다.

2026-10-30 기존 TC 전체 교체와 CLI 등록 현황은 [등록 기록](artel-registration.md)을 참고한다.

Unity 에디터 또는 Development Build에서 Artel에 연결하면 아래 함수가 카탈로그에 등록된다. 일반 릴리스와 SDK 없는 프로젝트에는 QA 코드가 포함되지 않는다.

## 치트

| 이름 | 인자 | 동작 |
|---|---|---|
| `go_to_stage` | `stage`: Slime, Elf, Shadow, Golem, NagaRogue, NagaWarrior, Wanderer | 선택 화면을 거쳐 정상 StageManager 진입. 보스 클리어 후 진행 처리 유지. 대화·인트로는 정상 실행 |
| `unlock_stages` | 없음 | 전투 스테이지 모두 해금 후 선택 화면 열기 |
| `heal_player` | `amount=10000` (1–10000) | 살아 있는 플레이어 회복, 최대 체력으로 제한 |
| `set_player_resources` | `healthPercent=100` (1–100), `gaugePercent=100` (0–100), `shield=0` (0–1000) | 현재 HP·게이지·방어막 설정 및 HUD 갱신 |
| `damage_enemies` | `amount` (0.1–1000000), `selection=Nearest` 또는 All | 방패·방랑자 정면 방어를 우회해 현재 페이즈 HP에서 N 감소. 일반 사망/페이즈 전환 실행 |
| `defeat_enemies` | `selection=Nearest` 또는 All | 정상 사망 경로로 처치. 페이즈 연출이 끝난 뒤 다음 페이즈 처리. All은 분열로 생긴 적도 재탐색. 최대 45초 |

`go_to_stage`는 저장된 현재/최대 스테이지와 튜토리얼 완료 값을 변경한다. 스테이지는 실제 StageInfo가 있고 빌드에 등록된 전투 7개로 제한했다. Cinematic은 저장값 전체를 지우는 기존 코드가 있어 이동 대상으로 노출하지 않는다. Sandworm은 빌드 씬과 데이터가 일치하지 않아 노출하지 않는다.

피해 치트의 양은 현재 페이즈 HP로 제한된다. 페이즈 전환 직후 HP가 다시 차는 것은 게임 동작이다. 살아남은 동일 페이즈 적의 방패는 보존한다. 처치 이후 승리 연출과 저장은 게임이 담당한다. 전투를 진행할 때 필요에 따라 피해·처치 치트를 사용한다.

## 지속 행동

| 이름 | 필수 인자 | 동작 |
|---|---|---|
| `keep_attacking` | 없음 | 제자리에서 가장 가까운 적을 조준하며 연속 공격·콤보 |
| `chase_and_attack` | `attackDistance` (0.5–5) | 적에게 접근 후 거리 내에서 연속 공격 |
| `keep_fleeing` | `safeDistance` (2–20), `useDash` (bool) | 주변 적들의 반대 방향으로 도주, 안전 거리에서 대기·재감시. 선택적으로 대시 |
| `keep_distance` | `distance` (2–15) | 가까우면 후퇴, 멀면 접근하고 게이지가 있으면 스킬 사용 |
| `circle_enemy` | `radius` (1–15) | 적을 조준하며 시계 방향 선회 |
| `keep_using_skill` | 없음 | 이동하지 않고 게이지가 허용하는 동안 비충전 스킬 반복 |
| `retreat_and_heal` | `safeDistance` (2–15) | 안전 거리 확보 후 게임의 회복 키를 반복 사용. 완전 회복 시 완료, 게이지 부족 시 실패 |

모두 `player_combat` 그룹이므로 새 행동을 시작하면 기존 행동이 정리된다. 공격·이동·스킬·회복은 플레이어의 기존 처리와 입력 잠금, 쿨다운, 소모 자원을 따른다. 필요하면 행동 실행 도중에도 치트로 HP·게이지를 보충할 수 있다.

행동 종료/교체/제한시간/연결 종료 시 주입한 입력을 해제한다. 이미 시작한 공격·대시 연출은 게임 규칙대로 마친다. 플레이어 사망·씬 변경·적 없음에서 종료한다(회복은 적이 없어도 가능). 대화나 조작 잠금 중에는 대기한다. 이동은 8방향 벽 감지로, 복잡한 미로 경로 탐색이나 모든 투사체의 회피를 보장하지 않는다.

호출 예시:

```json
{"method":"start_behavior","params":[{"name":"keep_fleeing","args":{"safeDistance":7,"useDash":true},"maxSeconds":60}]}
```

```json
{"method":"invoke_cheat","params":[{"name":"damage_enemies","args":{"amount":50,"selection":"Nearest"}}]}
```

중단은 SDK의 `stop_behavior`에 `behaviorId` 또는 `all:true`를 전달한다. 기본 실행 제한은 30초이며 SDK의 `maxSeconds`/`extend_behavior`로 조정한다.

## 검증 실행

프로젝트를 연 에디터를 닫고 Unity 2022.3.34f1에서 실행:

```powershell
& $unity -batchmode -projectPath $project -executeMethod ArtelCombatValidation.Run -logFile "$project/Logs/artel-combat-playmode.log"
& $unity -batchmode -quit -projectPath $project -executeMethod ArtelCombatValidation.BuildDevelopment -logFile "$project/Logs/artel-combat-build.log"
```

Play Mode 검증은 진행 관련 PlayerPrefs 3개를 백업/복원하며 `Logs/artel-combat-validation.txt`에 결과를 남긴다. 개발 빌드 출력은 `Build/ArtelQA/GyeMong.exe`이다. 강제 종료/에디터 크래시 시에는 복원 코드가 실행되지 않을 수 있다.

2026-10-22 검증: Unity 컴파일, 13개 카탈로그 등록, 슬라임→골렘→방랑자 이동, HP 설정/회복, 연속 공격·도주 및 중단 입력 해제, 골렘 방패 보존 HP 감소, 방랑자 방어 우회/처치 PASS. Windows 개발 빌드 성공 및 최종 DLL의 13개 함수 포함 확인. 인트로 대기 중에만 테스트 시간 배율을 높였다.

실행 중 기존 `HitCollider.hitEffect` 미할당과 `CameraManager.Awake` null 참조 로그가 관찰됐다. 전체 콘솔 오류 0건, 원격 서버 호출, CI/WebGL, 모든 행동의 장시간 실전 검증은 완료하지 않았다.
