using System;
using System.IO;
using UnityEngine;

public class DataManager : SingletonObject<DataManager>
{
    private readonly string savePath = Application.dataPath + "/Database";

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        SetKeyMappings();
        SetSoundSettings();
    }

    // 특정 구역 저장
    public void SaveSection<T>(T sectionData, string fileName) where T : class
    {
        string json = JsonUtility.ToJson(sectionData, true); // JSON으로 직렬화
        File.WriteAllText(Path.Combine(savePath, fileName + ".json"), json); // 파일 저장
        Debug.Log($"{fileName} saved at {savePath}");
    }

    // 특정 구역 불러오기
    public T LoadSection<T>(string fileName) where T : class, new()
    {
        string filePath = Path.Combine(savePath, fileName + ".json");
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath); // JSON 파일 읽기
            return JsonUtility.FromJson<T>(json); // JSON에서 객체로 역직렬화
        }
        else
        {
            Debug.LogWarning($"{fileName} not found, returning new instance.");
            return new T(); // 파일이 없으면 새 인스턴스 반환
        }
    }

    private void SetKeyMappings()
    {
        KeyMappingData keyMappingData = DataManager.Instance.LoadSection<KeyMappingData>("KeyMappingData");
        for (int i = 0; i < keyMappingData.keyBindings.Count; i++)
        {
            KeyMappingEntry entry = keyMappingData.keyBindings[i];

            ActionCode actionCode = (ActionCode)Enum.Parse(typeof(ActionCode), entry.actionCode);
            KeyCode keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), entry.keyCode);

            InputManager.Instance.SetKey(actionCode, keyCode);
        }
    }

    private void SetSoundSettings()
    {
        SoundData soundData = DataManager.Instance.LoadSection<SoundData>("SoundData");
        SoundManager.Instance.SetMasterVolume(soundData.masterVolume);
        SoundManager.Instance.SetVolume(SoundType.UI, soundData.UIVolume);
        SoundManager.Instance.SetVolume(SoundType.BGM, soundData.bgmVolume);
        SoundManager.Instance.SetVolume(SoundType.EFFECT, soundData.sfxVolume);
    }

    public SoundData GetSoundSettings()
    {
        SoundData soundData = DataManager.Instance.LoadSection<SoundData>("SoundData");
        return soundData;
    }
}
