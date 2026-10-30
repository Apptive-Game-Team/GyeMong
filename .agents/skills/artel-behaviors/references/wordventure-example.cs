// ARTEL_GAME_FUNCTIONS 는 cheat 기능이 들어간 ARTEL SDK(kr.artel.sdk)가 설치되어 있을 때 SDK 가 켜는 define 이다.
// SDK 가 없거나 cheat 기능이 없는 예전 SDK 이면 define 이 없어서 이 파일 전체가 compile 에서 빠진다.
#if ARTEL_GAME_FUNCTIONS
using System.Collections;
using Artel;
using Battle.Turns;
using Cards;
using Combat.Enemies;
using Combat.Spells;
using Combat.Stage;
using Combat.UI;
using Core;
using Map;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ARTEL QA agent 가 부르는 치트와 behavior(ARTEL-1052). 에디터와 개발 빌드에서만 등록되고, 출시 빌드에서는
/// SDK 가 아무것도 만들지 않는다. 모든 호출은 UI 가 부르는 것과 같은 게임 메서드를 지난다.
/// </summary>
public static class WordVentureGameFunctions
{
    // ---- when: conditions (null = available, otherwise the reason) ----

    static string InBattle() => TurnBattleSystem.Instance != null ? null : "not in a battle scene";

    static string HasPlayer() => Player.PlayerInt() != null ? null : "no player in this scene";

    static string HasLiveEnemy()
    {
        if (TurnBattleSystem.Instance == null) return "not in a battle scene";
        foreach (var enemy in Object.FindObjectsOfType<Enemy>())
            if (enemy.IsAlive) return null;
        return "no live enemy";
    }

    // ---- cheats ----

    [ArtelCheat("Sets the unlocked stage (MapMove.StagePosition, 0 village .. 4 boss) and saves it to PlayerPrefs. Takes effect on the map screen.")]
    static string UnlockStage([CheatRange(0, 4)] int stage)
    {
        MapMove.StagePosition = stage;
        if (SaveLoadController.Instance != null) SaveLoadController.Instance.SavePlayData();
        PlayerPrefs.Save();
        return $"stage_position={MapMove.StagePosition}";
    }

    [ArtelCheat("Heals the player by amount through Player.TakeHit(-amount). Health can go above 100.", when: nameof(HasPlayer))]
    static string HealPlayer([CheatRange(1, 100)] int amount)
    {
        Player.PlayerInt().TakeHit(-amount);
        return $"healed={amount}";
    }

    [ArtelCheat("Defeats every slime alive in the current wave with Enemy.Kill(), the same death path a spell takes. Every enemy in WordVenture is a slime.", when: nameof(HasLiveEnemy))]
    static string KillAllSlimes()
    {
        var count = 0;
        foreach (var enemy in Object.FindObjectsOfType<Enemy>())
        {
            if (!enemy.IsAlive) continue;
            enemy.Kill();
            count++;
        }
        return $"killed={count}";
    }

    // 씬 이름을 enum 으로 받는다. agent 가 문자열을 지어내지 않고 catalog 의 values 에서 고른다.
    public enum GameScene { Title, Story, Map, Battle, GameClear, GameOver, Ending }

    [ArtelCheat("Loads a scene directly. Battle and GameClear use stage (0 village .. 4 boss) and unlock up to it, the state the map sets before entering a battle.")]
    static string GoToScene(GameScene scene, [CheatRange(0, 4)] int stage = 0)
    {
        if (scene == GameScene.Battle || scene == GameScene.GameClear)
        {
            EnsureStageData().stagePosition = stage;
            if (MapMove.StagePosition < stage) UnlockStage(stage);
        }
        var name = SceneName(scene);
        SceneManager.LoadScene(name);
        return $"loading={name} stage={stage}";
    }

    [ArtelCheat("Marks all five stages cleared, saves, and loads EndingScene, the scene the game shows after the boss stage.")]
    static string ShowEnding()
    {
        // 보스(4)를 깬 뒤 BattleWaveController 가 StagePosition 을 5 로 올리고 GameClearController 가 엔딩을 연다.
        MapMove.StagePosition = 5;
        EnsureStageData().stagePosition = 4;
        if (SaveLoadController.Instance != null) SaveLoadController.Instance.SavePlayData();
        PlayerPrefs.Save();
        SceneManager.LoadScene("EndingScene");
        return "loading=EndingScene stage_position=5";
    }

    static string SceneName(GameScene scene) => scene switch
    {
        GameScene.Title => "TitleScene",
        GameScene.Story => "StoryScene",
        GameScene.Map => "MapScene",
        GameScene.Battle => "TurnBattleScene",
        GameScene.GameClear => "GameClearScene",
        GameScene.GameOver => "GameOverScene",
        _ => "EndingScene",
    };

    // StageDataSingleton 은 맵을 지나야 생긴다. 타이틀에서 전투로 바로 갈 때는 여기서 만든다(Awake 가 Instance 를 잡는다).
    static StageDataSingleton EnsureStageData()
    {
        if (StageDataSingleton.Instance == null)
            new GameObject("StageDataSingleton (cheat)").AddComponent<StageDataSingleton>();
        return StageDataSingleton.Instance;
    }

    // ---- behaviors ----

    [ArtelBehavior("Casts spells with the hand cards the way the UI does (combine, then pick a live enemy), and ends the turn when no pair is left. Completes when no enemy has been alive for 3 seconds.", group: "combat")]
    static IEnumerator AutoCastSpells([CheatRange(1, 100)] int maxCasts, ArtelBehaviorContext context)
    {
        var casts = 0;
        var noEnemySeconds = 0f;
        try
        {
            while (true)
            {
                var battle = TurnBattleSystem.Instance;
                var zone = CombineZone.Instance;
                if (battle == null || zone == null)
                {
                    context.Complete($"battle scene ended after {casts} casts");
                    yield break;
                }

                if (InteractionLock.IsLocked)
                {
                    context.Report("waiting: interaction locked");
                    yield return null;
                    continue;
                }

                if (zone.IsAwaitingTarget)
                {
                    foreach (var enemy in Object.FindObjectsOfType<Enemy>())
                    {
                        var selectable = enemy.GetComponent<SelectableObject>();
                        if (!enemy.IsAlive || selectable == null || !selectable.GetSelectable()) continue;
                        zone.SetTarget(selectable);
                        casts++;
                        context.Report($"cast {casts} at {enemy.name}");
                        break;
                    }
                    yield return null;
                    continue;
                }

                if (zone.IsCasting || SpellObj.HasActiveSpells)
                {
                    yield return null;
                    continue;
                }

                var anyAlive = false;
                foreach (var enemy in Object.FindObjectsOfType<Enemy>())
                    if (enemy.IsAlive) { anyAlive = true; break; }
                noEnemySeconds = anyAlive ? 0f : noEnemySeconds + Time.deltaTime;
                if (noEnemySeconds > 3f)
                {
                    context.Complete($"no enemy left after {casts} casts");
                    yield break;
                }
                if (casts >= maxCasts)
                {
                    context.Complete($"reached maxCasts={maxCasts}");
                    yield break;
                }

                Card spell = null, type = null;
                if (CardManager.Inst != null)
                {
                    foreach (var card in CardManager.Inst.HandCards)
                    {
                        if (card == null || !card.gameObject.activeInHierarchy) continue;
                        if (spell == null && card.CompareTag("Spell")) spell = card;
                        else if (type == null && card.CompareTag("MagicType")) type = card;
                    }
                }

                if (spell != null && type != null)
                {
                    zone.AddCard(spell.gameObject);
                    zone.AddCard(type.gameObject);
                    zone.OnButtonClick();
                    context.Report($"combining {spell.word.name} + {type.word.name}");
                }
                else
                {
                    battle.TurnEndButton();
                    context.Report("no spell pair in hand: ending turn");
                }
                yield return null;
            }
        }
        finally
        {
            if (CardManager.Inst != null) CardManager.Inst.CancelDrag();
        }
    }

    [ArtelBehavior("Presses the end-turn button each time it is the player's turn until turns have passed.", group: "combat")]
    static IEnumerator PassTurns([CheatRange(1, 50)] int turns, ArtelBehaviorContext context)
    {
        var passed = 0;
        var battle = TurnBattleSystem.Instance;
        if (battle == null)
        {
            context.Fail("not in a battle scene");
            yield break;
        }

        System.Action onEnded = () => passed++;
        battle.PlayerTurnEnded += onEnded;
        try
        {
            while (passed < turns)
            {
                if (TurnBattleSystem.Instance == null)
                {
                    context.Complete($"battle scene ended after {passed} turns");
                    yield break;
                }
                battle.TurnEndButton();
                context.Report($"turns passed={passed}/{turns}");
                yield return null;
            }
            context.Complete($"passed {passed} turns");
        }
        finally
        {
            if (battle != null) battle.PlayerTurnEnded -= onEnded;
        }
    }
}
#endif
