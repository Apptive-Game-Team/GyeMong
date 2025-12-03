using System.Collections.Generic;
using GyeMong.GameSystem.Map.Portal;
using GyeMong.SoundSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util;

public class BgmManager : SingletonObject<BgmManager>
{
    private static Coroutine bgmCoroutine;
    private static SoundObject soundObject;
    private static string currentBgmName = "";

    protected override void PostConstruct()
    {
        Initialize();
    }
    
    public static void Initialize()
    {
        Play();
        
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            Play(scene);
        };
    }
    
    public static string GetBgmName()
    {
        return currentBgmName;
    }
    
    private static void Play()
    {
        if (bgmCoroutine != null)
        {
            Stop();
        }
        Play(PortalManager.Instance.sceneDataList.GetSceneDataByName(
                SceneManager.GetActiveScene().name)
            .defaultBGM);
    }
    

    private static void Play(Scene scene)
    {
        if (bgmCoroutine != null)
        {
            Stop();
        }
        Play(PortalManager.Instance.sceneDataList.GetSceneDataByName(scene.name).defaultBGM);
    }
    
    public static void Play(string sound)
    {
        if (currentBgmName == sound && bgmCoroutine != null)
        {
            return;
        }
        if (bgmCoroutine == null)
        {
            soundObject = SoundManager.Instance.GetBgmObject();
        }

        currentBgmName = sound;
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

        Play();
    }
    
    public static class BgmStack
    {
        private static readonly Stack<string> bgmStack = new Stack<string>();

        public static void Push(string bgmName)
        {
            bgmStack.Push(BgmManager.GetBgmName());
            BgmManager.Play(bgmName);
        }

        public static void Pop()
        {
            if (bgmStack.Count > 0)
            {
                string bgmName = bgmStack.Pop();
                BgmManager.Play(bgmName);
            }
        }
    }
}
