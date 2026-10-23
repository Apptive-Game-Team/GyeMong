using System;
using System.Linq;
using GyeMong.InputSystem;
using GyeMong.UISystem;
using UnityEditor;
using UnityEngine;
using GyeMong.EventSystem.Controller;
using GyeMong.EventSystem.Event.Chat;
using TMPro;
using UnityEngine.UI;

public static class PlayerUiFeedbackValidation
{
    private const string Sheet = "Assets/GyeMong_Art/Art/Player/sheet2_walkfour.png";
    private const string Idle = "Assets/GyeMong_Art/Animator/Player/player_idle_anim/";

    // Explicit one-time asset repair; never runs during editor startup or gameplay.
    public static void RepairIdle()
    {
        var importer = (TextureImporter)AssetImporter.GetAtPath(Sheet);
        importer.GetSourceTextureWidthAndHeight(out int width, out int height);
        float cell = width / 4f;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.spritePixelsPerUnit = cell;
        importer.filterMode = FilterMode.Point;
        string[] directions = { "down", "right", "up", "left" };
        importer.spritesheet = directions.Select((direction, column) => new SpriteMetaData
        {
            name = "idle_" + direction,
            rect = new Rect(column * cell, height - cell, cell, cell),
            alignment = (int)SpriteAlignment.Center,
            pivot = new Vector2(0.5f, 0.5f)
        }).ToArray();
        importer.SaveAndReimport();
        var sprites = AssetDatabase.LoadAllAssetsAtPath(Sheet).OfType<Sprite>().ToArray();
        foreach (string direction in directions)
        {
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(Idle + "player_" + direction + "_idle.anim");
            var binding = AnimationUtility.GetObjectReferenceCurveBindings(clip).Single();
            AnimationUtility.SetObjectReferenceCurve(clip, binding, new[] {
                new ObjectReferenceKeyframe { time = 0, value = sprites.Single(s => s.name == "idle_" + direction) }
            });
            EditorUtility.SetDirty(clip);
        }
        var controller = AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>("Assets/GyeMong_Art/Animator/Player/Player.controller");
        var idle = controller.layers[0].stateMachine.states.Single(s => s.state.name == "player_idle").state.motion as UnityEditor.Animations.BlendTree;
        var children = idle.children;
        for (int i = 0; i < children.Length; i++)
        {
            var position = children[i].position;
            string direction = position.x > 0 ? "right" : position.x < 0 ? "left" : position.y > 0 ? "up" : "down";
            children[i].motion = AssetDatabase.LoadAssetAtPath<AnimationClip>(Idle + "player_" + direction + "_idle.anim");
        }
        idle.children = children;
        EditorUtility.SetDirty(idle);
        AssetDatabase.SaveAssets();
        Run();
    }

    [MenuItem("Tools/GyeMong/Validate Player UI Feedback")]
    public static void Run()
    {
        foreach (string direction in new[] { "down", "right", "up", "left" })
        {
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(Idle + "player_" + direction + "_idle.anim");
            var binding = AnimationUtility.GetObjectReferenceCurveBindings(clip).Single();
            var sprite = AnimationUtility.GetObjectReferenceCurve(clip, binding).Single().value as Sprite;
            if (sprite == null || sprite.bounds.size.x <= 0) throw new Exception("Missing idle sprite: " + direction);
            Debug.Log("FEEDBACK_CHECK: idle " + direction + " " + sprite.name + " " + sprite.bounds.size);
        }
        var input = InputManager.Instance;
        GameObject inputHost = null;
        if (input == null)
        {
            inputHost = new GameObject("Feedback validation input");
            input = inputHost.AddComponent<InputManager>();
        }
        var originalBindings = new System.Collections.Generic.Dictionary<ActionCode, KeyCode>(input.GetKeyActions());
        if (originalBindings.Count == 0) input.SetDefaultKey();
        var originalKey = input.GetKeyActions()[ActionCode.Dash];
        var originalLanguage = GameLocalization.Language;
        try
        {
            input.SetKey(ActionCode.Dash, KeyCode.K);
            foreach (var language in new[] { GameLanguage.Korean, GameLanguage.English })
            {
                GameLocalization.SetLanguage(language, false);
                string text = GameLocalization.Text("좌측 Shift키를 눌러 돌진할 수 있다.");
                if (!text.Contains("[K]") || text.Contains("Shift")) throw new Exception("Stale tutorial key: " + text);
            }
        }
        finally
        {
            input.SetKey(ActionCode.Dash, originalKey);
            input.GetKeyActions().Clear();
            foreach (var binding in originalBindings) input.GetKeyActions().Add(binding.Key, binding.Value);
            GameLocalization.SetLanguage(originalLanguage, false);
            if (inputHost != null) UnityEngine.Object.DestroyImmediate(inputHost);
        }
        Debug.Log("FEEDBACK_CHECK: rebound tutorial keys passed");
        var hud = PrefabUtility.LoadPrefabContents("Assets/Resources/Prefabs/InGameManager.prefab");
        try
        {
            if (hud.transform.Find("PlayerUI/HpHearts") != null) throw new Exception("Legacy hearts remain");
            if (hud.transform.Find("PlayerUI/PlayerHpFillArea").GetComponent<Slider>().fillRect == null)
                throw new Exception("Missing health fill");
        }
        finally { PrefabUtility.UnloadPrefabContents(hud); }
        var chat = PrefabUtility.LoadPrefabContents("Assets/Resources/Prefabs/UI/ChatCanvas.prefab");
        try
        {
            var controller = chat.GetComponent<ChatController>();
            typeof(ChatController).GetMethod("Awake", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(controller, null);
            var name = chat.transform.Find("NameArea").GetComponent<TMP_Text>();
            var message = chat.transform.Find("MessageArea").GetComponent<TMP_Text>();
            foreach (var language in new[] { GameLanguage.Korean, GameLanguage.English })
            {
                GameLocalization.SetLanguage(language, false);
                foreach (ChatSpeakerType speaker in Enum.GetValues(typeof(ChatSpeakerType)))
                {
                    ChatController.SetSpeaker(speaker);
                    if (name.fontSize != message.fontSize) throw new Exception("Mismatched dialogue size");
                    if (name.enabled && message.rectTransform.anchoredPosition.x < name.rectTransform.anchoredPosition.x + name.rectTransform.rect.width)
                        throw new Exception("Overlapping name and message");
                    var position = message.rectTransform.anchoredPosition;
                    ChatController.SetSpeaker(ChatSpeakerType.None);
                    ChatController.SetSpeaker(speaker);
                    if (message.rectTransform.anchoredPosition != position) throw new Exception("Dialogue layout drifts");
                }
            }
        }
        finally
        {
            GameLocalization.SetLanguage(originalLanguage, false);
            PrefabUtility.UnloadPrefabContents(chat);
        }
        Debug.Log("FEEDBACK_CHECK: health bar and bilingual speaker layout passed");
        if (Application.isBatchMode) UiDesignValidation.Run();
    }
}
