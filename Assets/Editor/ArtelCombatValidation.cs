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
    private static string runtimeError;
    private static readonly string[] SaveKeys = { "TutorialFlag", "CurrentStageId", "MaxStageId" };

    static ArtelCombatValidation()
    {
        EditorApplication.playModeStateChanged += state =>
        {
            if (state == PlayModeStateChange.EnteredPlayMode && SessionState.GetBool(RunningKey, false))
            {
                started = EditorApplication.timeSinceStartup;
                Application.logMessageReceived += (message, stack, type) =>
                {
                    if ((type == LogType.Error || type == LogType.Exception || type == LogType.Assert) &&
                        !stack.Contains("UnityEditor.Android.TargetScanWorker") && !stack.Contains("CartoonFX.CFXR_WelcomeScreen"))
                        runtimeError = message + "\n" + stack;
                };
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
            if (runtimeError != null) throw new InvalidOperationException("Runtime error: " + runtimeError);
            if (EditorApplication.timeSinceStartup - started > 480) throw new TimeoutException("Validation exceeded 480 seconds.");
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
        RuntimeQualityValidation.RunCombatChecks(player);
        yield return RuntimeQualityValidation.RunStateTransitionChecks();
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

        var golem = enemy as GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Boss;
        if (golem != null)
        {
            int phase = golem.CurrentPhase;
            var stun = golem.Stun(10);
            stun.MoveNext();
            GyeMongGameFunctions.DamageEnemies(golem.CurrentHp);
            Check(golem.CurrentPhase == phase + 1 && golem.CurrentHp > 0, "Golem changes phase while stunned");
            (stun as IDisposable)?.Dispose();
            yield return Ready();
        }

        yield return GyeMongGameFunctions.GoToStage(CombatStage.Wanderer);
        Check(SceneManager.GetActiveScene().name == "Wanderer", "stage Wanderer entered");
        yield return Ready();
        enemy = Object.FindObjectsOfType<Mob>().Where(m => m.CurrentHp > 1).OrderBy(m => (m.transform.position - SceneContext.Character.transform.position).sqrMagnitude).First();
        hp = enemy.CurrentHp;
        GyeMongGameFunctions.DamageEnemies(1);
        Check(Mathf.Approximately(enemy.CurrentHp, hp - 1), "Wanderer HP damage bypasses directional defense");
        yield return GyeMongGameFunctions.DefeatEnemies();
        Check(enemy == null || !enemy.isActiveAndEnabled || enemy.CurrentHp <= 0, "defeat enemy");
        while (Util.SceneLoader.Instance.IsLoading) yield return null;
        yield return RuntimeQualityValidation.RunStageCancellationChecks();
        yield return GyeMongGameFunctions.GoToStage(CombatStage.Slime);
        Check(SceneManager.GetActiveScene().name == "SpringBeach", "stage entry works after cancelled dialogue");
        while (Util.SceneLoader.Instance.IsLoading) yield return null;
        foreach (var stage in new[] { CombatStage.Elf, CombatStage.Shadow, CombatStage.NagaRogue, CombatStage.NagaWarrior })
        {
            yield return EnterStageWithAcceleratedIntro(stage);
            yield return Ready();
            var enemies = Object.FindObjectsOfType<GyeMong.GameSystem.Creature.Mob.StateMachineMob.StateMachineMob>();
            Check(enemies.Length > 0, "combat scene has state-machine enemies: " + stage);
            foreach (var mob in enemies) mob.StartCoroutine(mob.Stun(0.1f));
            yield return Wait(0.5f);
            Check(SceneContext.Character != null && SceneContext.Character.CurrentHp > 0, "stun and resume completed: " + stage);
        }
        while (Util.SceneLoader.Instance.IsLoading) yield return null;
        yield return ValidateSandworm();
    }

    private static IEnumerator EnterStageWithAcceleratedIntro(CombatStage stage)
    {
        var entry = GyeMongGameFunctions.GoToStage(stage);
        try
        {
            while (entry.MoveNext())
            {
                // Long authored intros exceed the cheat's interactive timeout without input.
                // Run the actual dialogue and transition, accelerating only this test.
                Time.timeScale = 20;
                yield return entry.Current;
            }
        }
        finally
        {
            (entry as IDisposable)?.Dispose();
            Time.timeScale = 1;
        }
    }

    private static IEnumerator ValidateSandworm()
    {
        var loading = EditorSceneManager.LoadSceneAsyncInPlayMode("Assets/Scenes/Summer/SummerSandWorm.unity",
            new LoadSceneParameters(LoadSceneMode.Single));
        while (!loading.isDone) yield return null;
        yield return Wait(1);
        var boss = Object.FindObjectOfType<GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Sandworm.Sandworm>(true);
        Check(boss != null, "legacy Sandworm scene contains its boss");
        for (var parent = boss.transform; parent != null; parent = parent.parent) parent.gameObject.SetActive(true);
        boss.enabled = true;
        yield return Wait(1);
        if (SceneContext.Character != null) SceneContext.Character.isInvincible = true;
        // Set up the final phase, then exercise the normal damage/death dispatch while stunned.
        typeof(GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Boss)
            .GetField("currentPhase", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(boss, 1);
        boss.StartCoroutine(boss.Stun(30));
        boss.OnAttacked(boss.CurrentHp + 1);
        Check(!boss.GetComponent<Collider2D>().enabled, "Sandworm death while stunned disables collision");
        yield return Wait(0.5f);
        Util.SceneLoader.LoadScene("TitleScene", 0);
        while (Util.SceneLoader.Instance.IsLoading) yield return null;
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
