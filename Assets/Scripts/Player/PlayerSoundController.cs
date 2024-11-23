using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace playerCharacter
{
    public enum Sound
    {
        Sword,
        Walk,
        Run,
    }

    public class PlayerSoundController : MonoBehaviour
    {
        private Dictionary<Sound, SoundObject> soundObjects = new Dictionary<Sound, SoundObject>();

        private void Awake()
        {
            soundObjects.Add(Sound.Sword, transform.Find("SwordSound").GetComponent<SoundObject>());
        }

        public void Trigger(Sound sound)
        {
            StartCoroutine(soundObjects[sound].Play());
        }
    }
}