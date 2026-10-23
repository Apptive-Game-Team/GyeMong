using GyeMong.GameSystem.Map.Portal;
using GyeMong.SoundSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BgmManager
{
    private static Coroutine bgmCoroutine;
    private static SoundObject soundObject;
    private static string currentBgmName = "";
    
    public static void Initialize()
    {
        Play();
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode) { Play(scene); }
    
    private static void Play()
    {
        Play(SceneManager.GetActiveScene());
    }
    

    private static void Play(Scene scene)
    {
        if (PortalManager.Instance.sceneDataList.TryGetSceneDataByName(scene.name, out var data)) Play(data.defaultBGM);
        else StopPlayback(); // Story-only scenes provide music through their own event sequence.
    }
    
    public static void Play(string sound)
    {
        if (currentBgmName == sound && bgmCoroutine != null)
        {
            return;
        }
        StopPlayback();
        soundObject = SoundManager.Instance.GetBgmObject();

        currentBgmName = sound;
        soundObject.SetSoundSourceByName(sound);
        soundObject.SetLoop(true);
        bgmCoroutine = soundObject.StartCoroutine(soundObject.Play());
    }
    
    public static void Stop()
    {
        StopPlayback();
        Play();
    }

    private static void StopPlayback()
    {
        if (bgmCoroutine != null)
        {
            if (soundObject != null) soundObject.Stop();
            bgmCoroutine = null;
        }
    }
}
