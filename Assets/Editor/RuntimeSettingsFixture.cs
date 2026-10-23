using System;
using System.IO;
using System.Collections.Generic;
using GyeMong.DataSystem;
using GyeMong.InputSystem;
using GyeMong.SoundSystem;
using UnityEditor;
using UnityEngine;

internal static class RuntimeSettingsFixture
{
    private const string Key = "GyeMong.SettingsFixture";
    private static readonly string[] Files = { "KeyMappingData.json", "SoundData.json" };

    private const string BackupPath = "Library/RuntimeSettingsFixtureBackup.json";
    [Serializable] private class Entry
    {
        public string path;
        public bool exists;
        public string data;
        public int attributes;
    }
    [Serializable] private class Backup
    {
        public bool directory;
        public bool directoryMeta;
        public List<Entry> entries = new List<Entry>();
    }

    public static void Prepare()
    {
        if (File.Exists(BackupPath))
            throw new InvalidOperationException("Recover the previous fixture with RuntimeSettingsFixture.Restore before running again.");
        var backup = new Backup { directory = Directory.Exists("Assets/Database"), directoryMeta = File.Exists("Assets/Database.meta") };
        foreach (var name in Files)
            foreach (var suffix in new[] { "", ".meta" })
            {
                string path = "Assets/Database/" + name + suffix;
                bool exists = File.Exists(path);
                backup.entries.Add(new Entry { path = path, exists = exists,
                    data = exists ? Convert.ToBase64String(File.ReadAllBytes(path)) : "",
                    attributes = exists ? (int)File.GetAttributes(path) : 0 });
            }
        // Persist the complete original state before touching any user file.
        File.WriteAllText(BackupPath, JsonUtility.ToJson(backup));
        SessionState.SetBool(Key, true);
        Directory.CreateDirectory("Assets/Database");
        foreach (var entry in backup.entries)
            if (entry.exists) File.SetAttributes(entry.path, FileAttributes.Normal);
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
        if (!File.Exists(BackupPath)) return;
        var backup = JsonUtility.FromJson<Backup>(File.ReadAllText(BackupPath));
        foreach (var entry in backup.entries)
        {
            if (File.Exists(entry.path)) File.SetAttributes(entry.path, FileAttributes.Normal);
            if (entry.exists)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(entry.path));
                File.WriteAllBytes(entry.path, Convert.FromBase64String(entry.data));
                File.SetAttributes(entry.path, (FileAttributes)entry.attributes);
            }
            else if (File.Exists(entry.path)) File.Delete(entry.path);
        }
        if (!backup.directory && Directory.Exists("Assets/Database") && Directory.GetFileSystemEntries("Assets/Database").Length == 0)
        {
            Directory.Delete("Assets/Database");
            if (!backup.directoryMeta && File.Exists("Assets/Database.meta")) File.Delete("Assets/Database.meta");
        }
        File.Delete(BackupPath);
        SessionState.SetBool(Key, false);
    }
}
