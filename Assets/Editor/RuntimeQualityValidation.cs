using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob;
using GyeMong.GameSystem.Creature.Player;
using Util.ChangeListener;
using GyeMong.GameSystem.Map.Stage.Select;
using GyeMong.GameSystem.Map.Stage;
using GyeMong.GameSystem.Map.Stage.ScriptableObject;
using GyeMong.EventSystem.Controller;
using GyeMong.EventSystem.Event.Chat;
using UnityEngine.SceneManagement;
using Util;
using System.Linq;
using System.Reflection;
using GyeMong.DataSystem;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class RuntimeQualityValidation
{
    private static void Check(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
        Debug.Log("QUALITY_CHECK: " + message);
    }

    public static void RunData()
    {
        var codec = typeof(DataManager).Assembly.GetType("GyeMong.DataSystem.SavedSectionCodec");
        string Encode(string value) => (string)codec.GetMethod("Encrypt").Invoke(null, new object[] { value });
        string Decode(string value) => (string)codec.GetMethod("Decrypt").Invoke(null, new object[] { value });
        const string json = "{\"masterVolume\":0.37,\"note\":\"한글 English\"}";
        string encrypted = Encode(json);
        Check(Decode(encrypted) == json, "release codec Unicode round trip");
        Check(Encode(json) != encrypted, "fresh IV per save");
        Check(Decode(json) == json, "legacy editor JSON readable");
        foreach (var invalid in new[] { "", "not base64", "GYEMONG1:AA==" })
        {
            bool rejected = false;
            try { Decode(invalid); } catch (TargetInvocationException) { rejected = true; }
            Check(rejected, "invalid payload rejected");
        }
        string root = Path.Combine(Path.GetTempPath(), "GyeMong-validation-" + Guid.NewGuid().ToString("N"));
        var host = new GameObject("Data validation");
        try
        {
            var manager = host.AddComponent<DataManager>();
            typeof(DataManager).GetField("savePath", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(manager, root);
            manager.SaveSection(new SoundData { masterVolume = 0.37f }, "SoundData");
            Check(Mathf.Approximately(manager.LoadSection<SoundData>("SoundData").masterVolume, 0.37f), "new directory save and load");
            string path = Path.Combine(root, "SoundData.json");
            File.WriteAllText(path, encrypted);
            Check(Mathf.Approximately(manager.LoadSection<SoundData>("SoundData").masterVolume, 0.37f), "encrypted file readable in editor");
            File.WriteAllText(path, "broken");
            Check(Mathf.Approximately(manager.LoadSection<SoundData>("SoundData").masterVolume, 0.5f), "corrupt file falls back to defaults");
            Check(File.ReadAllText(path) == "broken", "corrupt original preserved");
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(host);
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }
        const string key = "MaxStageId";
        bool hadProgress = PlayerPrefs.HasKey(key);
        int originalProgress = PlayerPrefs.GetInt(key);
        try
        {
            PlayerPrefs.SetInt(key, 7);
            StageSelectPage.SaveProgress(2);
            Check(PlayerPrefs.GetInt(key) == 7, "replaying an earlier stage preserves unlocks");
            StageSelectPage.SaveProgress(8);
            Check(PlayerPrefs.GetInt(key) == 7, "last-stage progress stays in range");
            PlayerPrefs.SetInt(key, -100);
            StageSelectPage.SaveProgress(-1);
            Check(PlayerPrefs.GetInt(key) == 0, "invalid negative progress recovers");
        }
        finally
        {
            if (hadProgress) PlayerPrefs.SetInt(key, originalProgress); else PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }
        Debug.Log("RUNTIME_DATA_VALIDATION_PASS");
    }

    private sealed class Listener : IChangeListener<float>
    {
        public Action Callback;
        public int Calls;
        public void OnChanged(float value) { Calls++; Callback?.Invoke(); }
    }
    private sealed class Caller : ChangeListenerCaller<Listener, float> { }
    private sealed class CountingState : StateMachineMob.BaseState
    {
        public int Exits, Updates, Ticks;
        public override int GetWeight() => 1;
        public override IEnumerator StateCoroutine() { while (true) { Ticks++; yield return null; } }
        public override void OnStateExit() { Exits++; }
        public override void OnStateUpdate() { Updates++; }
    }

    private sealed class ImmediateTransition : StateMachineMob.BaseState
    {
        public StateMachineMob.BaseState Next;
        public override int GetWeight() => 1;
        public override IEnumerator StateCoroutine() { mob.ChangeState(Next); yield break; }
    }

    public static IEnumerator RunStateTransitionChecks()
    {
        var host = new GameObject("Immediate state transition validation");
        try
        {
            var mob = host.AddComponent<StateMachineMob>();
            var next = new CountingState { mob = mob };
            mob.ChangeState(new ImmediateTransition { mob = mob, Next = next });
            yield return null;
            var stun = mob.Stun(1);
            stun.MoveNext();
            int ticks = next.Ticks;
            yield return null;
            yield return null;
            Check(next.Ticks == ticks && next.Exits == 1, "synchronous transition retains the new coroutine handle");
            (stun as IDisposable)?.Dispose();
        }
        finally { UnityEngine.Object.Destroy(host); }
    }

    public static void RunCombatChecks(PlayerCharacter player)
    {
        var caller = new Caller();
        var first = new Listener();
        var second = new Listener();
        first.Callback = () => { caller.RemoveListener(first); caller.AddListener(second); };
        caller.AddListener(first);
        caller.AddListener(first);
        caller.Call(1);
        Check(first.Calls == 1 && second.Calls == 0, "listener mutation and duplicate registration");
        caller.Call(2);
        Check(first.Calls == 1 && second.Calls == 1, "listener removal and next notification");

        var health = typeof(PlayerCharacter).GetField("curHealth", BindingFlags.NonPublic | BindingFlags.Instance);
        float originalHp = player.CurrentHp, originalShield = player.curShield;
        bool invincible = player.isInvincible;
        var gauge = typeof(PlayerCharacter).GetField("curSkillGauge", BindingFlags.NonPublic | BindingFlags.Instance);
        var canMove = typeof(PlayerCharacter).GetField("canMove", BindingFlags.NonPublic | BindingFlags.Instance);
        float originalGauge = (float)gauge.GetValue(player);
        bool originalCanMove = (bool)canMove.GetValue(player);
        try
        {
            foreach (float shield in new[] { 0f, 0.5f, 1f, 2f })
            {
                health.SetValue(player, 10f);
                player.curShield = shield;
                player.isInvincible = false;
                player.TakeDamage(1);
                Check(Mathf.Approximately(player.CurrentHp, 10f - Mathf.Max(0, 1 - shield)), "shield absorbs HP damage: " + shield);
                Check(Mathf.Approximately(player.curShield, Mathf.Max(0, shield - 1)), "shield remainder: " + shield);
            }
            typeof(PlayerCharacter).GetField("movement", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(player, Vector2.right);
            player.playerRb.velocity = Vector2.right;
            player.SetPlayerMove(false);
            Check(player.Movement == Vector2.zero && player.Velocity == Vector2.zero, "movement lock clears stale motion");
        }
        finally
        {
            health.SetValue(player, originalHp);
            player.curShield = originalShield;
            player.isInvincible = invincible;
            player.SetPlayerMove(originalCanMove);
            gauge.SetValue(player, originalGauge);
            player.changeListenerCaller.CallSkillGaugeChangeListeners(originalGauge);
            player.changeListenerCaller.CallHpChangeListeners(originalHp);
            player.changeListenerCaller.CallShieldChangeListeners(originalShield);
        }

        var host = new GameObject("State lifecycle validation");
        try
        {
            var mob = host.AddComponent<StateMachineMob>();
            var state = new CountingState { mob = mob };
            mob.ChangeState(state);
            var stun = mob.Stun(1);
            stun.MoveNext();
            typeof(StateMachineMob).GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(mob, null);
            Check(state.Exits == 1 && state.Updates == 0, "stun exits once and stops updates");
            mob.ChangeState(new Dictionary<Type, int>());
            Check(state.Exits == 1, "empty weighted fallback does not exit twice");
            (stun as IDisposable)?.Dispose();
        }
        finally { UnityEngine.Object.Destroy(host); }
        Debug.Log("RUNTIME_COMBAT_REGRESSION_PASS");
    }

    public static IEnumerator RunStageCancellationChecks()
    {
        var stage = ScriptableObject.CreateInstance<StageInfo>();
        var chat = ScriptableObject.CreateInstance<MultiChatMessageData>();
        chat.chatMessages = new List<MultiChatMessageData.MultiChatMessage>
        {
            new MultiChatMessageData.MultiChatMessage
            {
                speakerName = ChatSpeakerType.None,
                messages = new List<string> { new string('x', 2000) }
            }
        };
        stage.beforeScript = new List<MultiChatMessageData> { chat };
        stage.afterScript = new List<MultiChatMessageData> { chat };
        try
        {
            foreach (bool clearing in new[] { false, true })
            {
                SceneLoader.LoadScene("StageSelectPage", 0);
                while (SceneLoader.Instance.IsLoading) yield return null;
                if (clearing) StageManager.ClearStage(SceneLoader.Instance);
                else StageManager.EnterStage(stage, SceneLoader.Instance);
                float until = Time.realtimeSinceStartup + 1;
                while (Time.realtimeSinceStartup < until) yield return null;
                SceneLoader.LoadScene("TitleScene", 0);
                while (SceneLoader.Instance.IsLoading) yield return null;
                var flag = typeof(StageManager).GetField(clearing ? "_clearing" : "_entering", BindingFlags.NonPublic | BindingFlags.Static);
                Check(!(bool)flag.GetValue(null), "stage dialogue cancellation releases transition flag");
                var sounds = (ICollection)typeof(ChatController).GetField("dialogueSounds", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
                Check(sounds.Count == 0, "stage dialogue cancellation stops looping sounds");
                until = Time.realtimeSinceStartup + 1;
                while (Time.realtimeSinceStartup < until) yield return null;
                Check(SceneManager.GetActiveScene().name == "TitleScene", "cancelled script cannot redirect from title");
            }
        }
        finally
        {
            StageManager.CancelTransitions(SceneLoader.Instance);
            UnityEngine.Object.Destroy(stage);
            UnityEngine.Object.Destroy(chat);
        }
    }

    public static void BuildWebGL()
    {
        if (!Application.isBatchMode) throw new InvalidOperationException("Batch mode only.");
        RunData();
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).Distinct().ToArray(),
            locationPathName = "Build/RuntimeValidationWebGL",
            target = BuildTarget.WebGL,
            options = BuildOptions.None
        });
        if (report.summary.result != BuildResult.Succeeded)
            throw new Exception("WebGL validation build failed: " + report.summary.result);
        Debug.Log("RUNTIME_WEBGL_BUILD_PASS");
    }
}
