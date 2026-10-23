using System;
using System.IO;
using GyeMong.DataSystem;
using GyeMong.InputSystem;
using GyeMong.SoundSystem;
using UnityEditor;
using UnityEngine;

internal static class RuntimeSettingsFixture
{
    private const string Key = "GyeMong.SettingsFixture";
    private static readonly string[] Files = { "KeyMappingData.json", "SoundData.json" };

    public static void Prepare()
    {
        if (SessionState.GetBool(Key, false)) throw new InvalidOperationException("Restore the previous settings fixture first.");
        SessionState.SetBool(Key + ".directory", Directory.Exists("Assets/Database"));
        SessionState.SetBool(Key + ".directoryMeta", File.Exists("Assets/Database.meta"));
        Directory.CreateDirectory("Assets/Database");
        foreach (var name in Files)
            foreach (var suffix in new[] { "", ".meta" })
            {
                string path = "Assets/Database/" + name + suffix;
                bool exists = File.Exists(path);
                SessionState.SetBool(Key + path, exists);
                if (!exists) continue;
                SessionState.SetString(Key + path + ".data", Convert.ToBase64String(File.ReadAllBytes(path)));
                SessionState.SetInt(Key + path + ".attributes", (int)File.GetAttributes(path));
                File.SetAttributes(path, FileAttributes.Normal);
            }
        SessionState.SetBool(Key, true);
        var mapping = new KeyMappingData();
        mapping.keyBindings.Add(new KeyMappingEntry(ActionCode.Heal, KeyCode.F2));
        File.WriteAllText("Assets/Database/KeyMappingData.json", JsonUtility.ToJson(mapping));
        File.WriteAllText("Assets/Database/SoundData.json", JsonUtility.ToJson(new SoundData { masterVolume = 0.37f, sfxVolume = 0.23f }));
    }

    public static void Verify()
    {
        if (!SessionState.GetBool(Key, false)) return;
        if (InputManager.Instance.GetKeyActions()[ActionCode.Heal] != KeyCode.F2)
            throw new Exception("Saved key binding was overwritten during startup.");
        if (Mathf.Abs(SoundManager.Instance.MasterVolume - 0.37f) > 0.001f ||
            Mathf.Abs(SoundManager.Instance.GetVolume(SoundType.EFFECT) - 0.23f) > 0.001f)
            throw new Exception("Saved sound settings were not applied.");
    }

    public static void Restore()
    {
        if (!SessionState.GetBool(Key, false)) return;
        foreach (var name in Files)
            foreach (var suffix in new[] { "", ".meta" })
            {
                string path = "Assets/Database/" + name + suffix;
                if (File.Exists(path)) File.SetAttributes(path, FileAttributes.Normal);
                if (SessionState.GetBool(Key + path, false))
                {
                    File.WriteAllBytes(path, Convert.FromBase64String(SessionState.GetString(Key + path + ".data", "")));
                    File.SetAttributes(path, (FileAttributes)SessionState.GetInt(Key + path + ".attributes", 0));
                }
                else if (File.Exists(path)) File.Delete(path);
            }
        if (!SessionState.GetBool(Key + ".directory", true) && Directory.GetFileSystemEntries("Assets/Database").Length == 0)
        {
            Directory.Delete("Assets/Database");
            if (!SessionState.GetBool(Key + ".directoryMeta", true) && File.Exists("Assets/Database.meta")) File.Delete("Assets/Database.meta");
        }
        SessionState.SetBool(Key, false);
    }
}
