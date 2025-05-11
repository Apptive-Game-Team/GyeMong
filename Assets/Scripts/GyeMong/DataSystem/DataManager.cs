using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using GyeMong.GameSystem.Creature.Player;
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
        private readonly string encryptionKey = "GyemongFighting!"; // 16, 24, 32 글자로 key 설정

        private void Awake()
        {
#if UNITY_EDITOR
#else
        savePath = Path.Combine(Application.persistentDataPath, "Database");
#endif
        }
    
        private void Start()
        {
            LoadKeyMappings();
        }

        // 특정 구역 저장
        public void SaveSection<T>(T sectionData, string fileName) where T : class
        {
#if UNITY_EDITOR
            string json = JsonUtility.ToJson(sectionData, true); // JSON으로 직렬화
            string filePath = Path.Combine(savePath, fileName + ".json");
            // 파일의 읽기 전용 속성을 해제 (파일이 이미 존재할 경우)
            if (File.Exists(filePath))
            {
                FileAttributes attributes = File.GetAttributes(filePath);
                if (attributes.HasFlag(FileAttributes.ReadOnly))
                {
                    File.SetAttributes(filePath, attributes & ~FileAttributes.ReadOnly);
                }
            }

            // 파일 저장
            File.WriteAllText(filePath, json);

            // 파일을 다시 읽기 전용으로 설정
            File.SetAttributes(filePath, FileAttributes.ReadOnly);

            Debug.Log($"{fileName} saved at {savePath} as ReadOnly.");
#else
        EnsurePathExists();
        string json = JsonUtility.ToJson(sectionData, true); // JSON으로 직렬화
        string encryptedData = Encrypt(json); // JSON 데이터를 암호화
        string filePath = Path.Combine(savePath, fileName + ".json");

        // 파일 저장
        File.WriteAllText(filePath, encryptedData);

        Debug.Log($"{fileName} saved at {savePath} as ReadOnly and encrypted.");
#endif
        
        }

        private void EnsurePathExists()
        {
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
        }

        // 특정 구역 불러오기
        public T LoadSection<T>(string fileName) where T : class, new()
        {
            string filePath = Path.Combine(savePath, fileName + ".json");
            if (File.Exists(filePath))
            {
#if UNITY_EDITOR
                string json = File.ReadAllText(filePath); // 파일에서 json 데이터 읽기
                try
                {
                    return JsonUtility.FromJson<T>(json); // JSON에서 객체로 역직렬화
                }
                catch
                {
                    json = Decrypt(json); // 데이터를 복호화
                    return JsonUtility.FromJson<T>(json); // JSON에서 객체로 역직렬화
                }
            
#else
            string encryptedData = File.ReadAllText(filePath); // 파일에서 암호화된 데이터 읽기
            string json = Decrypt(encryptedData); // 데이터를 복호화
            return JsonUtility.FromJson<T>(json); // JSON에서 객체로 역직렬화
#endif
            }
            else
            {
                Debug.LogWarning($"{fileName} not found, returning new instance.");
                return new T(); // 파일이 없으면 새 인스턴스 반환
            }
        }

        // AES 암호화
        private string Encrypt(string plainText)
        {
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(encryptionKey);
            aes.IV = new byte[16]; // 16-byte 초기화 벡터 (0으로 초기화)
            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var writer = new StreamWriter(cs))
            {
                writer.Write(plainText);
            }
            return Convert.ToBase64String(ms.ToArray()); // 암호화된 데이터를 Base64로 인코딩
        }

        // AES 복호화
        private string Decrypt(string cipherText)
        {
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(encryptionKey);
            aes.IV = new byte[16]; // 16-byte 초기화 벡터 (0으로 초기화)
            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var reader = new StreamReader(cs);
            return reader.ReadToEnd(); // 복호화된 데이터를 반환
        }
        private void LoadKeyMappings()
        {
            InputManager.Instance.SetDefaultKey();
            KeyMappingData keyMappingData = LoadSection<KeyMappingData>("KeyMappingData");
            for (int i = 0; i < keyMappingData.keyBindings.Count; i++)
            {
                KeyMappingEntry entry = keyMappingData.keyBindings[i];

                ActionCode actionCode = (ActionCode)Enum.Parse(typeof(ActionCode), entry.actionCode);
                KeyCode keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), entry.keyCode);

                InputManager.Instance.SetKey(actionCode, keyCode);
            }
        }

        public void LoadPlayerData()
        {
            PlayerData playerData = LoadSection<PlayerData>("PlayerData");
            if (!playerData.isFirst)
            {
                PlayerCharacter.Instance.transform.position = playerData.playerPosition;
                StartCoroutine(PlayerCharacter.Instance.LoadPlayerEffect());
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
