using System;
using System.IO;
using GyeMong.InputSystem;
using UnityEngine;
using Util;

namespace GyeMong.DataSystem
{
    public class DataManager : SingletonObject<DataManager>
    {
#if UNITY_EDITOR
        private readonly string savePath = Application.dataPath + "/Database";
#else
    private string savePath;
#endif

        protected override void Awake()
        {
            base.Awake();
#if UNITY_EDITOR
#else
        savePath = Path.Combine(Application.persistentDataPath, "Database");
#endif
        }
    
        private void Start()
        {
            LoadKeyMappings();
        }

        public void SaveSection<T>(T sectionData, string fileName) where T : class
        {
            Directory.CreateDirectory(savePath);
            string filePath = Path.Combine(savePath, fileName + ".json");
            string json = JsonUtility.ToJson(sectionData, true);
#if !UNITY_EDITOR
            json = SavedSectionCodec.Encrypt(json);
#endif
            if (File.Exists(filePath))
                File.SetAttributes(filePath, File.GetAttributes(filePath) & ~FileAttributes.ReadOnly);
            File.WriteAllText(filePath, json);
        }

        public T LoadSection<T>(string fileName) where T : class, new()
        {
            string filePath = Path.Combine(savePath, fileName + ".json");
            if (!File.Exists(filePath)) return new T();
            try
            {
                string json = SavedSectionCodec.Decrypt(File.ReadAllText(filePath));
                return JsonUtility.FromJson<T>(json) ?? new T();
            }
            catch (Exception error) when (error is IOException || error is UnauthorizedAccessException ||
                error is FormatException || error is System.Security.Cryptography.CryptographicException ||
                error is ArgumentException)
            {
                // Preserve the original file so a failed load does not destroy recoverable data.
                Debug.LogWarning($"Cannot load {fileName}; using defaults. {error.Message}");
                return new T();
            }
        }

        private void LoadKeyMappings()
        {
            InputManager.Instance.SetDefaultKey();
            KeyMappingData keyMappingData = LoadSection<KeyMappingData>("KeyMappingData");
            if (keyMappingData.keyBindings == null) return;
            foreach (KeyMappingEntry entry in keyMappingData.keyBindings)
            {
                if (entry == null ||
                    !Enum.TryParse(entry.actionCode, out ActionCode actionCode) || !Enum.IsDefined(typeof(ActionCode), actionCode) ||
                    !Enum.TryParse(entry.keyCode, out KeyCode keyCode) || !Enum.IsDefined(typeof(KeyCode), keyCode))
                    continue;
                InputManager.Instance.SetKey(actionCode, keyCode);
            }
        }

        public void DeleteAllData()
        {
            try
            {
                if (Directory.Exists(savePath))
                {
                    DirectoryInfo directory = new(savePath);

                    // 모든 파일 삭제
                    foreach (FileInfo file in directory.GetFiles())
                    {
                        file.IsReadOnly = false;
                        file.Delete();
                    }

                    // 모든 폴더 삭제
                    foreach (DirectoryInfo subDirectory in directory.GetDirectories())
                    {
                        subDirectory.Delete(true);
                    }

                    Debug.Log("All saved data has been deleted.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to delete data: {ex.Message}");
            }
        }

    }
}
