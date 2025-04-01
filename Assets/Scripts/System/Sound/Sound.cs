using System.Collections;
using UnityEngine;
using Util.Interface;
using Util.ObjectCreator;

namespace System.Sound
{
    public class Sound
    {
        private static ObjectPool<SoundObject> _objectPool = null;
        private static ObjectPool<SoundObject> ObjectPool
        {
            get
            {
                if (_objectPool == null)
                {
                    _objectPool = new ObjectPool<SoundObject>(10, UnityEngine.Resources.Load<GameObject>("Prefabs/System/Sound/SoundObject"), SoundManager.Instance.transform);
                }
                return _objectPool;
            }
        } 
        public static SoundObject Play(string soundId, bool loop = false)
        {
            SoundObject soundObject = ObjectPool.GetObject();
            soundObject.gameObject.SetActive(true);
            soundObject.Initialize();
            soundObject.SetSoundSourceByName(soundId);
            soundObject.SetLoop(loop);
            soundObject.StartCoroutine(Play(soundObject, new SoundPlayCallback(soundObject)));
            return soundObject;
        }
        public static void Stop(SoundObject soundObject)
        {
            soundObject?.Stop();
            soundObject?.gameObject.SetActive(false);
        }
        private class SoundPlayCallback : ICallback
        {
            private SoundObject _soundObject;
            
            public SoundPlayCallback(SoundObject soundObject)
            {
                _soundObject = soundObject;
            }
            public void OnProcessCompleted()
            {
                _soundObject.gameObject.SetActive(false);
            }
        }
        
        public static IEnumerator Play(SoundObject soundObject, ICallback callback)
        {
            yield return soundObject.Play();
            callback.OnProcessCompleted();
        }
    }
}