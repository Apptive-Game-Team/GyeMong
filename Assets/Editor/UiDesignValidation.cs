#if UNITY_EDITOR
using System;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using GyeMong.EventSystem.Controller;
using GyeMong.EventSystem.Event.Chat;
using GyeMong.UISystem;
using GyeMong.UISystem.Option;

public static class UiDesignValidation
{
    private const string Output = "Logs/ui-design";

    public static void RunTutorialInputs()
    {
        if (!Application.isBatchMode) throw new InvalidOperationException("Use batch mode to preserve open scenes.");
        Directory.CreateDirectory(Output);
        var previous = GameLocalization.Language;
        try
        {
            foreach (var language in new[] { GameLanguage.Korean, GameLanguage.English })
            {
                GameLocalization.SetLanguage(language, false);
                foreach (var kind in new[] { "tutorial", "tutorial-skill", "tutorial-move", "tutorial-dash", "tutorial-rebound", "tutorial-plain" })
                    Preview(kind, 1920, 1080);
                Preview("tutorial", 1920, 1200);
            }
            Debug.Log("TUTORIAL_INPUT_VALIDATION_PASS");
        }
        finally { GameLocalization.SetLanguage(previous, false); }
    }

    // Explicit batch entry point. Preview scenes are never saved over game scenes.
    public static void Run()
    {
        if (!Application.isBatchMode)
            throw new InvalidOperationException("Run previews in a separate batchmode editor to preserve open scenes.");
        Directory.CreateDirectory(Output);
        var sources = new System.Collections.Generic.SortedSet<string>();
        foreach (var data in Resources.LoadAll<MultiChatMessageData>("ScriptableObjects/Chat/ChatData"))
            foreach (var chat in data.chatMessages)
                foreach (var line in chat.messages)
                    sources.Add(line);
        File.WriteAllLines(Output + "/dialogue-source.jsonl", System.Linq.Enumerable.Select(sources, s => JsonUtility.ToJson(new SourceLine { source = s })));
        foreach (string source in sources)
            if (!GameLocalization.HasTranslation(source)) throw new Exception("Missing dialogue translation: " + source);
        foreach (string path in new[] {
            "Assets/Resources/Prefabs/UI/ChatCanvas.prefab",
            "Assets/Resources/Prefabs/UI/PopupWindow.prefab",
            "Assets/Resources/Prefabs/InGameManager.prefab",
            "Assets/Resources/Prefabs/Creature/Boss/BossUi/HpBarCanvas.prefab" })
        {
            GameObject root = PrefabUtility.LoadPrefabContents(path);
            try
            {
                foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
                    if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(child.gameObject) != 0)
                        throw new Exception("Missing script: " + path + "/" + child.name);
            }
            finally { PrefabUtility.UnloadPrefabContents(root); }
        }
        var previousLanguage = GameLocalization.Language;
        foreach (var language in new[] { GameLanguage.Korean, GameLanguage.English })
        {
        GameLocalization.SetLanguage(language, false);
        Preview("options", 1920, 1080);
        Preview("controls", 1920, 1080);
        Preview("audio", 1920, 1080);
        Preview("dialogue", 1920, 1080);
        Preview("narration", 1920, 1080);
        Preview("tutorial", 1920, 1080);
        Preview("hud", 1920, 1080);
        Preview("dialogue", 1920, 1200);
        Preview("tutorial", 1920, 1200);
        Preview("boss", 1920, 1080);
        Preview("boss", 1920, 1200);
        Preview("gameover", 1920, 1200);
        }
        GameLocalization.SetLanguage(previousLanguage, false);
        Debug.Log("UI_DESIGN_VALIDATION_PASS");
    }

    [Serializable] private class SourceLine { public string source; }

    private static void Preview(string kind, int width, int height)
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var input = new GameObject("Preview Input").AddComponent<GyeMong.InputSystem.InputManager>();
        input.SetDefaultKey();
        var camera = new GameObject("Preview Camera").AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.16f, 0.22f, 0.19f);
        camera.orthographic = true;
        camera.transform.position = new Vector3(0, 0, -10);
        var texture = new RenderTexture(width, height, 24);
        camera.targetTexture = texture;
        bool tutorial = kind.StartsWith("tutorial");
        bool dialogue = kind == "dialogue" || kind == "narration";
        bool options = kind == "options" || kind == "controls" || kind == "audio";
        string path = dialogue ? "Assets/Resources/Prefabs/UI/ChatCanvas.prefab" : "Assets/Resources/Prefabs/InGameManager.prefab";
        if (kind == "boss") path = "Assets/Resources/Prefabs/Creature/Boss/BossUi/HpBarCanvas.prefab";
        if (options) path = "Assets/Resources/Prefabs/UI/OptionController.prefab";
        var root = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(path));
        var canvas = root.GetComponentInChildren<Canvas>(true);
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = camera;
        canvas.planeDistance = 1;
        if (!options) foreach (Transform child in root.transform)
            child.gameObject.SetActive(kind == "boss" || (dialogue ? child.name != "SpeechBubble" : child.name == (tutorial ? "PopupWindow" : kind == "gameover" ? "GameOverUI" : "PlayerUI")));
        foreach (var text in root.GetComponentsInChildren<TMP_Text>(true))
            if (GameLocalization.HasTranslation(text.text)) LocalizedGameText.Bind(text, text.text);
        if (options)
        {
            canvas.gameObject.SetActive(true);
            foreach (Transform child in canvas.transform) child.gameObject.SetActive(child.name == (kind == "controls" ? "KeyMappingUI" : kind == "audio" ? "SoundControlUI" : "OptionUI"));
            OptionLocalization.Install(root.transform);
        }
        if (dialogue)
        {
            root.GetComponent<ChatController>().SendMessage("Awake");
            root.transform.Find("ChatWindow").GetComponent<Image>().color = Color.black;
            var name = root.transform.Find("NameArea").GetComponent<TMP_Text>();
            name.color = Color.white;
            var message = root.transform.Find("MessageArea").GetComponent<TMP_Text>();
            var original = message.rectTransform.anchoredPosition;
            ChatController.SetSpeaker(ChatSpeakerType.None);
            if (name.enabled || name.text.Length != 0 || message.rectTransform.anchoredPosition.x >= original.x)
                throw new Exception("Narration must hide the speaker and expand left.");
            ChatController.SetSpeaker(ChatSpeakerType.Player);
            var namedPosition = message.rectTransform.anchoredPosition;
            ChatController.SetSpeaker(ChatSpeakerType.None);
            ChatController.SetSpeaker(ChatSpeakerType.Player);
            if (!name.enabled || message.rectTransform.anchoredPosition != namedPosition || name.fontSize != message.fontSize)
                throw new Exception("Named dialogue must have stable geometry and matching text sizes.");
            if (kind == "narration") ChatController.SetSpeaker(ChatSpeakerType.None);
            ChatController.SetBackgroundImage(ChatController.GetBackgroundImageSprite(BackgroundImage.Spring));
            ChatController.SetCharacterImage(null, true);
            ChatController.SetCharacterImage(kind == "narration" ? null : ChatController.GetCharacterImageSprite(ChatSpeakerType.Player), true);
            root.transform.Find("ChatImageArea").GetComponent<Image>().enabled = false;
            message.text = GameLocalization.Text("계절이 바뀌어도, 우리가 걸어온 길은 사라지지 않아.") + "\n" + GameLocalization.Text("숲 너머에서 기다리고 있을게. 준비가 되면 함께 가자.");
            message.color = Color.white;
            foreach (var data in Resources.LoadAll<MultiChatMessageData>("ScriptableObjects/Chat/ChatData"))
            foreach (var chat in data.chatMessages)
            foreach (var line in chat.messages)
                if (message.GetPreferredValues(GameLocalization.Text(line), message.rectTransform.rect.width, 0).y > message.rectTransform.rect.height)
                    throw new Exception("Dialogue line exceeds panel: " + line);
        }
        else if (tutorial)
        {
            var window = root.transform.Find("PopupWindow/Window");
            string title = "기본 공격", content = "마우스 좌클릭을 통해 공격할 수 있다.";
            if (kind == "tutorial-skill") { title = "스치기 시스템 배우기"; content = "좌상단에 늘어난 게이지를 이용해 특수공격(마우스 우클릭)을 사용할 수 있다."; }
            if (kind == "tutorial-move") { title = "플레이어 이동"; content = "W, A, S, D를 눌러서 이동할 수 있다."; }
            if (kind == "tutorial-dash") { title = "돌진"; content = "좌측 Shift키를 눌러 돌진할 수 있다."; }
            if (kind == "tutorial-rebound") input.SetKey(GyeMong.InputSystem.ActionCode.Attack, KeyCode.E);
            if (kind == "tutorial-plain") { title = "슬라임 잡기"; content = "이제 귀여운 슬라임을 잡아보자!"; }
            var controller = root.transform.Find("PopupWindow").GetComponent<PopupWindowController>();
            controller.OpenPopupWindow(title, content);
            var illustration = window.Find("InputIllustration");
            if (illustration == null || illustration.gameObject.activeSelf != (kind != "tutorial-plain"))
                throw new Exception("Incorrect tutorial control visibility: " + kind);
            foreach (var icon in window.GetComponentsInChildren<RawImage>())
                if (icon.texture == null || icon.texture.width == 0) throw new Exception("Missing input illustration: " + icon.name);
            if (kind == "tutorial" || kind == "tutorial-skill")
            {
                float expected = kind == "tutorial" ? 1 : -1;
                if (illustration.Find("Mouse").GetComponent<RawImage>().uvRect.width != expected)
                    throw new Exception("Wrong highlighted mouse button");
            }
            if (kind == "tutorial-rebound" && illustration.Find("Key/Label").GetComponent<TMP_Text>().text != "E")
                throw new Exception("Key rebinding was not reflected");
        }
        else if (kind == "hud")
        {
            if (root.transform.Find("PlayerUI/HpHearts") != null)
                throw new Exception("Legacy hearts must not be present in the HUD.");
            root.transform.Find("PlayerUI/PlayerHpFillArea").GetComponent<Slider>().value = 0.65f;
            root.transform.Find("PlayerUI").GetComponent<Slider>().value = 0.65f;
            root.transform.Find("PlayerUI/DashCoolTime").GetComponent<Slider>().value = 1;
        }
        Canvas.ForceUpdateCanvases();
        if (kind == "boss")
        {
            var bar = root.transform.Find("HpBar");
            var controller = bar.GetComponent<GyeMong.UISystem.Game.BossUI.HpBarController>();
            var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            var type = controller.GetType();
            type.GetField("_rectTransform", flags).SetValue(controller, bar.GetComponent<RectTransform>());
            type.GetField("_curHpBar", flags).SetValue(controller, bar.Find("CurHp").GetComponent<RectTransform>());
            type.GetField("_curShieldBar", flags).SetValue(controller, bar.Find("CurShield").GetComponent<RectTransform>());
            controller.UpdateHp(65, 0);
            float expected = bar.GetComponent<RectTransform>().rect.width * 0.65f;
            if (Mathf.Abs(bar.Find("CurHp").GetComponent<RectTransform>().rect.width - expected) > 0.1f)
                throw new Exception("Boss fill does not match health ratio.");
        }
        foreach (var text in root.GetComponentsInChildren<TMP_Text>()) text.ForceMeshUpdate();
        camera.Render();
        RenderTexture.active = texture;
        var image = new Texture2D(width, height, TextureFormat.RGB24, false);
        image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        image.Apply();
        File.WriteAllBytes($"{Output}/{kind}-{GameLocalization.Language}-{width}x{height}.png", image.EncodeToPNG());
        RenderTexture.active = null;
        camera.targetTexture = null;
        UnityEngine.Object.DestroyImmediate(image);
        UnityEngine.Object.DestroyImmediate(texture);
    }
}
#endif
