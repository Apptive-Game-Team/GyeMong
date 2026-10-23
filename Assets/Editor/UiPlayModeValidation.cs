#if UNITY_EDITOR
using System;
using GyeMong.EventSystem.Controller;
using GyeMong.EventSystem.Event.Chat;
using GyeMong.UISystem;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class UiPlayModeValidation
{
    private const string Key = "GyeMong.UiPlayValidation";
    private static double nextCheck;
    private static int step;

    public static void Run()
    {
        if (!Application.isBatchMode) throw new Exception("Batch mode only.");
        SessionState.SetBool(Key, true);
        SessionState.SetInt(Key + ".saved", PlayerPrefs.GetInt(GameLocalization.PreferenceKey, -1));
        EditorSceneManager.OpenScene("Assets/Scenes/TitleScene.unity");
        EditorApplication.EnterPlaymode();
    }

    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        if (!SessionState.GetBool(Key, false)) return;
        EditorApplication.update -= Tick;
        EditorApplication.update += Tick;
        nextCheck = EditorApplication.timeSinceStartup + 12;
    }

    private static void Tick()
    {
        if (!EditorApplication.isPlaying || EditorApplication.timeSinceStartup < nextCheck) return;
        try
        {
            if (step == 0 || step == 2)
            {
                var option = UnityEngine.Object.FindObjectOfType<GyeMong.UISystem.Option.OptionUI>(true);
                if (option == null) throw new Exception("Missing options in title scene.");
                var panel = option.transform.Find("Canvas/OptionUI");
                float previousScale = Time.timeScale;
                UnityEngine.Object.FindObjectOfType<GyeMong.UISystem.Option.OptionUIToggler>(true).ToggleOption();
                if (Time.timeScale != 0 || !GyeMong.GameSystem.GameTime.IsPaused)
                    throw new Exception("Options must pause gameplay.");
                var button = panel.Find("LanguageButton").GetComponent<Button>();
                GameLocalization.SetLanguage(GameLanguage.Korean, false);
                button.onClick.Invoke();
                if (GameLocalization.Language != GameLanguage.English || PlayerPrefs.GetInt(GameLocalization.PreferenceKey) != 1)
                    throw new Exception("Language button did not save English.");
                var label = panel.Find("ResumeButton/Localized Label").GetComponent<TMP_Text>();
                if (label.text != "Resume") throw new Exception("Active options did not refresh.");
                UnityEngine.Object.FindObjectOfType<GyeMong.UISystem.Option.OptionUIToggler>(true).ToggleOption();
                if (Time.timeScale != previousScale || GyeMong.GameSystem.GameTime.IsPaused)
                    throw new Exception("Closing options must restore gameplay time.");
                SceneManager.LoadScene("Cinematic");
            }
            else
            {
                var chat = UnityEngine.Object.FindObjectOfType<ChatController>(true);
                if (chat == null) throw new Exception("Missing story dialogue canvas.");
                var name = chat.transform.Find("NameArea").GetComponent<TMP_Text>();
                var message = chat.transform.Find("MessageArea").GetComponent<TMP_Text>();
                ChatController.SetSpeaker(ChatSpeakerType.Player);
                float namedX = message.rectTransform.anchoredPosition.x;
                if (name.text != "Player :") throw new Exception("Speaker did not retain English across scene load.");
                ChatController.SetSpeaker(ChatSpeakerType.None);
                if (name.enabled || name.text.Length != 0 || message.rectTransform.anchoredPosition.x >= namedX)
                    throw new Exception("Story narration layout is incorrect.");
                ChatController.SetSpeaker(ChatSpeakerType.Player);
                if (message.rectTransform.anchoredPosition.x != namedX) throw new Exception("Speaker layout drifted.");
                if (step == 3)
                {
                    Finish(0);
                    return;
                }
                SceneManager.LoadScene("TitleScene");
            }
            step++;
            nextCheck = EditorApplication.timeSinceStartup + 10;
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            Finish(1);
        }
    }

    private static void Finish(int code)
    {
        int saved = SessionState.GetInt(Key + ".saved", -1);
        if (saved < 0) PlayerPrefs.DeleteKey(GameLocalization.PreferenceKey);
        else PlayerPrefs.SetInt(GameLocalization.PreferenceKey, saved);
        PlayerPrefs.Save();
        SessionState.SetBool(Key, false);
        EditorApplication.update -= Tick;
        if (code == 0) Debug.Log("UI_PLAYMODE_VALIDATION_PASS: Title -> Cinematic -> Title -> Cinematic; language persistence and narration.");
        EditorApplication.Exit(code);
    }
}
#endif
