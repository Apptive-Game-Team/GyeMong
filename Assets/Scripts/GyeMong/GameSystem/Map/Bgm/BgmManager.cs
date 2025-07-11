using GyeMong.GameSystem.Map.Portal;
using GyeMong.SoundSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BgmManager
{
    private static Coroutine bgmCoroutine;
    private static SoundObject soundObject;
    
    public static void Initialize()
    {
        if (bgmCoroutine != null)
        {
            Stop();
        }
        Play(PortalManager.Instance.sceneDataList.GetSceneDataByName(
            SceneManager.GetActiveScene().name)
            .defaultBGM);
        
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            if (bgmCoroutine != null)
            {
                Stop();
            }
            Play(PortalManager.Instance.sceneDataList.GetSceneDataByName(scene.name).defaultBGM);
        };
    }
    
    public static void Play(string sound)
    {
        if (bgmCoroutine == null)
        {
            soundObject = SoundManager.Instance.GetBgmObject();
        }

        soundObject.SetSoundSourceByName(sound);
        soundObject.SetLoop(true);
        bgmCoroutine = soundObject.StartCoroutine(soundObject.Play());
    }
    
    public static void Stop()
    {
        if (bgmCoroutine != null)
        {
            soundObject.Stop();
            bgmCoroutine = null;
        }
        
        
    }
}
