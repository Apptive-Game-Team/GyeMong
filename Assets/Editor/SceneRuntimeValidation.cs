using System;
using System.IO;
using System.Linq;
using GyeMong.EventSystem.Controller;
using GyeMong.InputSystem;
using GyeMong.UISystem.Option;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Util;
using Object = UnityEngine.Object;

public static class SceneRuntimeValidation
{
    private const string Key = "GyeMong.RuntimeValidation";
    private static readonly string[] PreferenceKeys = { "MaxStageId", "CurrentStageId", "TutorialFlag", "GyeMong.Language" };
    private static string[] scenes;
    private static int index;
    private static double readyAt;
    private static double deadline;
    private static bool loaded;
    private static string runtimeError;

    public static void Run()
    {
        Begin(false);
    }

    public static void RunCold() { Begin(true); }

    public static void RunFinal()
    {
        SceneRuntimeMigration.Audit();
        UiDesignValidation.Run();
        RuntimeSettingsFixture.Prepare();
        Begin(true);
    }

    private static void Begin(bool cold)
    {
        if (!Application.isBatchMode) throw new InvalidOperationException("Batch mode only.");
        foreach (var key in PreferenceKeys)
        {
            SessionState.SetBool(Key + key + ".exists", PlayerPrefs.HasKey(key));
            SessionState.SetInt(Key + key, PlayerPrefs.GetInt(key));
        }
        SessionState.SetBool(Key, true);
        SessionState.SetBool(Key + ".cold", cold);
        SessionState.SetInt(Key + ".index", 0);
        EditorSceneManager.OpenScene("Assets/Scenes/TitleScene.unity");
        EditorApplication.EnterPlaymode();
    }

    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        if (!SessionState.GetBool(Key, false)) return;
        var buildScenes = EditorBuildSettings.scenes.Where(s => s.enabled && Path.GetFileNameWithoutExtension(s.path) != "LoadingScene")
            .Select(s => Path.GetFileNameWithoutExtension(s.path)).Distinct();
        bool cold = SessionState.GetBool(Key + ".cold", false);
        scenes = (cold ? buildScenes : buildScenes.Concat(new[] { "TitleScene", "Cinematic", "TitleScene", "Cinematic", "TitleScene" })).ToArray();
        index = cold ? SessionState.GetInt(Key + ".index", 0) : 0;
        SceneManager.sceneLoaded += OnLoaded;
        Application.logMessageReceived += OnLog;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        EditorApplication.update += Tick;
        deadline = EditorApplication.timeSinceStartup + 120;
        Directory.CreateDirectory("Logs/runtime-scenes");
    }

    private static void OnLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "LoadingScene") return;
        try
        {
            Require(SceneTransitionCurtain.IsCovered, "First frame is not covered: " + scene.name);
            var chat = Object.FindObjectOfType<ChatController>();
            if (chat != null)
            {
                foreach (var name in new[] { "BackgroundArea", "ChatImageArea", "CharacterImageArea", "CharacterImageArea2", "ChatWindow" })
                {
                    var image = chat.transform.Find(name).GetComponent<UnityEngine.UI.Image>();
                    Require(!image.enabled || image.color.a == 0, "Default dialogue image visible: " + name);
                }
            }
            Capture(scene.name + "-covered");
            loaded = true;
            readyAt = EditorApplication.timeSinceStartup + 1.0;
            deadline = EditorApplication.timeSinceStartup + 40;
        }
        catch (Exception exception) { Fail(exception); }
    }

    private static void Tick()
    {
        if (!EditorApplication.isPlaying) return;
        try
        {
            if (runtimeError != null) throw new Exception(runtimeError);
            if (EditorApplication.timeSinceStartup > deadline) throw new Exception("Scene initialization timeout.");
            if (!loaded || EditorApplication.timeSinceStartup < readyAt || SceneTransitionCurtain.IsCovered) return;
            Require(Object.FindObjectsOfType<InputManager>(true).Length == 1, "Duplicate input services.");
            Require(Object.FindObjectsOfType<OptionUIToggler>(true).Length == 1, "Duplicate options.");
            Require(Object.FindObjectsOfType<EventSystem>(true).Length == 1, "Duplicate EventSystems.");
            RuntimeSettingsFixture.Verify();
            var scene = SceneManager.GetActiveScene();
            Require(scene.name == scenes[index], "A duplicate request replaced the intended destination.");
            foreach (var canvas in Object.FindObjectsOfType<Canvas>())
                if (canvas.name == "TitleUI") Require(scene.name == "TitleScene" && canvas.gameObject.scene == scene, "Title UI escaped scene ownership.");
            // Dismiss the SDK's project-picker overlay through its own UI action for game captures.
            var sdk = GameObject.Find("Artel Overlay Canvas");
            if (sdk != null)
                foreach (var button in sdk.GetComponentsInChildren<UnityEngine.UI.Button>())
                    if (button.GetComponentInChildren<UnityEngine.UI.Text>()?.text == "나중에") button.onClick.Invoke();
            foreach (var ui in Object.FindObjectsOfType<ChatController>(true)) Require(ui.gameObject.scene == scene, "Dialogue survived its scene.");
            foreach (var ui in Object.FindObjectsOfType<EffectManager>(true)) Require(ui.gameObject.scene == scene, "HUD survived its scene.");
            Capture(scene.name + "-ready");
            Debug.Log("SCENE_RUNTIME_READY: " + scene.name);
            index++;
            if (index >= scenes.Length) { Finish(0); return; }
            if (SessionState.GetBool(Key + ".cold", false))
            {
                SessionState.SetInt(Key + ".index", index);
                loaded = false;
                EditorApplication.ExitPlaymode();
                return;
            }
            OptionUIToggler.Instance.ToggleOption();
            loaded = false;
            SceneLoader.LoadScene(scenes[index], 0);
            SceneLoader.LoadScene("TitleScene", 0);
            Require(SceneLoader.Instance.IsLoading, "Transition lock released early.");
        }
        catch (Exception exception) { Fail(exception); }
    }

    private static void Require(bool condition, string message) { if (!condition) throw new Exception(message); }
    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.EnteredEditMode || !SessionState.GetBool(Key, false) ||
            !SessionState.GetBool(Key + ".cold", false)) return;
        EditorApplication.delayCall += () =>
        {
            int next = SessionState.GetInt(Key + ".index", 0);
            var path = EditorBuildSettings.scenes.First(s => Path.GetFileNameWithoutExtension(s.path) == scenes[next]).path;
            EditorSceneManager.OpenScene(path);
            EditorApplication.EnterPlaymode();
        };
    }
    private static void OnLog(string message, string stack, LogType type)
    {
        if (stack.Contains("CartoonFX.CFXR_WelcomeScreen.CreateGUI")) return;
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert ||
            (message.Contains("DOTWEEN") && message.Contains("missing/null"))) runtimeError = message + "\n" + stack;
    }
    private static void Capture(string name)
    {
        var camera = Camera.main;
        if (camera == null) return;
        Debug.Log($"SCENE_CAMERA: {name}; {camera.gameObject.scene.name}; {camera.transform.position}; canvases=" +
            string.Join(",", Object.FindObjectsOfType<Canvas>().Where(c => c.isRootCanvas).Select(c => c.name + ":" + c.gameObject.scene.name)));
        var canvases = Object.FindObjectsOfType<Canvas>().Where(c => c.isRootCanvas && c.renderMode == RenderMode.ScreenSpaceOverlay).ToArray();
        var originalCameras = canvases.Select(c => c.worldCamera).ToArray();
        var originalDistances = canvases.Select(c => c.planeDistance).ToArray();
        var previousTarget = camera.targetTexture;
        var previousActive = RenderTexture.active;
        var target = new RenderTexture(1920, 1080, 24);
        var image = new Texture2D(1920, 1080, TextureFormat.RGB24, false);
        try
        {
            camera.targetTexture = target;
            foreach (var canvas in canvases)
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = camera;
                canvas.planeDistance = camera.nearClipPlane + 0.1f;
            }
            Canvas.ForceUpdateCanvases();
            UnityEngine.Rendering.RenderPipeline.SubmitRenderRequest(camera,
                new UnityEngine.Rendering.Universal.UniversalRenderPipeline.SingleCameraRequest { destination = target });
            RenderTexture.active = target;
            image.ReadPixels(new Rect(0, 0, 1920, 1080), 0, 0);
            image.Apply();
            File.WriteAllBytes("Logs/runtime-scenes/" + name + ".png", image.EncodeToPNG());
        }
        finally
        {
            camera.targetTexture = previousTarget;
            RenderTexture.active = previousActive;
            for (int i = 0; i < canvases.Length; i++)
            {
                canvases[i].renderMode = RenderMode.ScreenSpaceOverlay;
                canvases[i].worldCamera = originalCameras[i];
                canvases[i].planeDistance = originalDistances[i];
            }
            Object.DestroyImmediate(image);
            Object.DestroyImmediate(target);
        }
    }
    private static void Fail(Exception exception) { Debug.LogException(exception); Finish(1); }
    private static void Finish(int code)
    {
        RuntimeSettingsFixture.Restore();
        foreach (var key in PreferenceKeys)
            if (SessionState.GetBool(Key + key + ".exists", false)) PlayerPrefs.SetInt(key, SessionState.GetInt(Key + key, 0));
            else PlayerPrefs.DeleteKey(key);
        PlayerPrefs.Save();
        SessionState.SetBool(Key, false);
        EditorApplication.update -= Tick;
        SceneManager.sceneLoaded -= OnLoaded;
        Application.logMessageReceived -= OnLog;
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        if (code == 0) Debug.Log("SCENE_RUNTIME_VALIDATION_PASS");
        EditorApplication.Exit(code);
    }
}
