#if UNITY_EDITOR && ARTEL_GAME_FUNCTIONS
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Artel;
using GyeMong.GameSystem.Creature.Mob;
using GyeMong.GameSystem.Creature.Player;
using GyeMong.QA;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

// Explicitly invoked validation only; never starts a test on a normal editor launch.
[InitializeOnLoad]
public static class ArtelCombatValidation
{
    private const string RunningKey = "GyeMong.QaValidation.Running";
    private const string ReportPath = "Logs/artel-combat-validation.txt";
    private static readonly Stack<IEnumerator> Steps = new Stack<IEnumerator>();
    private static int lastFrame = -1;
    private static double started;
    private static readonly string[] SaveKeys = { "TutorialFlag", "CurrentStageId", "MaxStageId" };

    static ArtelCombatValidation()
    {
        EditorApplication.playModeStateChanged += state =>
        {
            if (state == PlayModeStateChange.EnteredPlayMode && SessionState.GetBool(RunningKey, false))
            {
                started = EditorApplication.timeSinceStartup;
                Steps.Push(Validate());
                EditorApplication.update += Tick;
            }
        };
    }

    public static void Run()
    {
        if (!Application.isBatchMode) throw new InvalidOperationException("Run validation in a separate batchmode editor.");
        Directory.CreateDirectory("Logs");
        File.WriteAllText(ReportPath, "Artel combat validation\n");
        foreach (string key in SaveKeys)
        {
            SessionState.SetBool(RunningKey + key + "exists", PlayerPrefs.HasKey(key));
            SessionState.SetInt(RunningKey + key, PlayerPrefs.GetInt(key));
        }
        SessionState.SetBool(RunningKey, true);
        EditorSceneManager.OpenScene("Assets/Scenes/TitleScene.unity");
        EditorApplication.EnterPlaymode();
    }

    private static void Tick()
    {
        try
        {
            if (EditorApplication.timeSinceStartup - started > 240) throw new TimeoutException("Validation exceeded 240 seconds.");
            if (!EditorApplication.isPlaying || Time.frameCount == lastFrame) return;
            lastFrame = Time.frameCount;
            while (Steps.Count > 0)
            {
                var step = Steps.Peek();
                if (!step.MoveNext()) { (step as IDisposable)?.Dispose(); Steps.Pop(); continue; }
                if (step.Current is IEnumerator child) { Steps.Push(child); continue; }
                return;
            }
            Finish(null);
        }
        catch (Exception error) { Finish(error); }
    }

    private static void Finish(Exception error)
    {
        EditorApplication.update -= Tick;
        while (Steps.Count > 0) (Steps.Pop() as IDisposable)?.Dispose();
        foreach (string key in SaveKeys)
        {
            if (SessionState.GetBool(RunningKey + key + "exists", false)) PlayerPrefs.SetInt(key, SessionState.GetInt(RunningKey + key, 0));
            else PlayerPrefs.DeleteKey(key);
        }
        PlayerPrefs.Save();
        SessionState.SetBool(RunningKey, false);
        File.AppendAllText(ReportPath, error == null ? "PASS\n" : "FAIL: " + error + "\n");
        EditorApplication.Exit(error == null ? 0 : 1);
    }

    private static void Check(bool condition, string name)
    {
        if (!condition) throw new InvalidOperationException(name);
        File.AppendAllText(ReportPath, "OK: " + name + "\n");
    }

    private static IEnumerator Wait(float seconds)
    {
        float until = Time.realtimeSinceStartup + seconds;
        while (Time.realtimeSinceStartup < until) yield return null;
    }

    private static IEnumerator Ready()
    {
        float until = Time.realtimeSinceStartup + 80;
        if (SceneContext.Character != null) SceneContext.Character.isInvincible = true;
        yield return Wait(0.5f);
        Time.timeScale = 5;
        try
        {
            while (SceneContext.Character == null || SceneContext.Character.isControlled)
            {
                if (SceneContext.Character != null) SceneContext.Character.isInvincible = true;
                if (Time.realtimeSinceStartup > until) throw new TimeoutException("Player control did not become ready.");
                yield return null;
            }
        }
        finally { Time.timeScale = 1; }
        SceneContext.Character.isInvincible = true;
        yield return Wait(0.5f);
    }

    private static bool HasInjectedInput(PlayerCharacter player) => (bool)typeof(PlayerCharacter)
        .GetField("_qaInputActive", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(player);

    private static IEnumerator Validate()
    {
        yield return Wait(1);
        var registryType = typeof(ArtelCheatAttribute).Assembly.GetType("Artel.GameFunctionRegistry", true);
        var registry = registryType.GetProperty("Shared", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        foreach (string kind in new[] { "Cheats", "Behaviors" })
        {
            var functions = ((IEnumerable)registryType.GetProperty(kind).GetValue(registry)).Cast<object>().ToArray();
            string[] expected = kind == "Cheats"
                ? new[] { "heal_player", "set_player_resources", "damage_enemies", "defeat_enemies", "unlock_stages", "go_to_stage" }
                : new[] { "keep_attacking", "chase_and_attack", "keep_fleeing", "keep_distance", "circle_enemy", "keep_using_skill", "retreat_and_heal" };
            var names = functions.Select(f => (string)f.GetType().GetField("Name").GetValue(f)).ToArray();
            foreach (string name in expected) Check(names.Contains(name), "registered " + name);
            if (kind == "Behaviors")
                foreach (var function in functions.Where(f => expected.Contains((string)f.GetType().GetField("Name").GetValue(f))))
                    Check((string)function.GetType().GetField("Group").GetValue(function) == "player_combat", "combat group");
        }

        yield return GyeMongGameFunctions.GoToStage(CombatStage.Slime);
        Check(SceneManager.GetActiveScene().name == "SpringBeach", "stage Slime entered");
        yield return Ready();
        var player = SceneContext.Character;
        GyeMongGameFunctions.SetPlayerResources(50, 100, 0);
        Check(Mathf.Approximately(player.CurrentHp, player.stat.HealthMax / 2), "resource HP setup");
        GyeMongGameFunctions.HealPlayer();
        Check(Mathf.Approximately(player.CurrentHp, player.stat.HealthMax), "heal player");

        var attack = GyeMongGameFunctions.KeepAttacking(new ArtelBehaviorContext());
        bool attacked = false;
        float end = Time.realtimeSinceStartup + 2;
        try
        {
            while (Time.realtimeSinceStartup < end && attack.MoveNext())
            {
                attacked |= player.isAttacking;
                yield return null;
            }
        }
        finally { (attack as IDisposable)?.Dispose(); }
        Check(attacked, "continuous attack entered normal attack state");
        Check(!HasInjectedInput(player), "attack cancellation releases input");

        var flee = GyeMongGameFunctions.KeepFleeing(20, false, new ArtelBehaviorContext());
        Vector3 start = player.transform.position;
        end = Time.realtimeSinceStartup + 2;
        try { while (Time.realtimeSinceStartup < end && flee.MoveNext()) yield return null; }
        finally { (flee as IDisposable)?.Dispose(); }
        Check(Vector3.Distance(start, player.transform.position) > 0.1f, "flee moved player");
        Check(!HasInjectedInput(player), "flee cancellation releases input");

        yield return GyeMongGameFunctions.GoToStage(CombatStage.Golem);
        Check(SceneManager.GetActiveScene().name == "SpringGolemTemple", "stage Golem entered");
        yield return Ready();
        var enemy = Object.FindObjectsOfType<Mob>().Where(m => m.CurrentHp > 1).OrderBy(m => (m.transform.position - SceneContext.Character.transform.position).sqrMagnitude).First();
        float hp = enemy.CurrentHp;
        enemy.currentShield = 100;
        GyeMongGameFunctions.DamageEnemies(1);
        Check(Mathf.Approximately(enemy.CurrentHp, hp - 1) && Mathf.Approximately(enemy.CurrentShield, 100), "HP damage bypasses/preserves shield");

        yield return GyeMongGameFunctions.GoToStage(CombatStage.Wanderer);
        Check(SceneManager.GetActiveScene().name == "Wanderer", "stage Wanderer entered");
        yield return Ready();
        enemy = Object.FindObjectsOfType<Mob>().Where(m => m.CurrentHp > 1).OrderBy(m => (m.transform.position - SceneContext.Character.transform.position).sqrMagnitude).First();
        hp = enemy.CurrentHp;
        GyeMongGameFunctions.DamageEnemies(1);
        Check(Mathf.Approximately(enemy.CurrentHp, hp - 1), "Wanderer HP damage bypasses directional defense");
        yield return GyeMongGameFunctions.DefeatEnemies();
        Check(enemy == null || !enemy.isActiveAndEnabled || enemy.CurrentHp <= 0, "defeat enemy");
    }

    public static void BuildDevelopment()
    {
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).Distinct().ToArray(),
            locationPathName = "Build/ArtelQA/GyeMong.exe",
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.Development
        });
        if (report.summary.result != BuildResult.Succeeded) throw new InvalidOperationException("Development build failed: " + report.summary.result);
    }
}
#endif
