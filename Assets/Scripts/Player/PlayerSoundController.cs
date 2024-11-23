using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace playerCharacter
{
    public enum Sound
    {
        Sword,
        Foot,
    }

    public enum FloorType
    {
        DEFAULT = -1,
        GRASS = 0,
        STONE = 1,
    }

    public enum WalkType
    {
        DEFAULT = -1,
        RUN = 0,
        WALK = 1,
    }

    public enum FootSoundType
    {
        GRASS_RUN = 0,
        GRASS_WALK = 1,
        STONE_RUN = 2,
        STONE_WALK = 3,
    }

    public class PlayerSoundController : MonoBehaviour
    {
        private Dictionary<Sound, SoundObject> soundObjects = new Dictionary<Sound, SoundObject>();
        private Dictionary<Sound, bool> soundBools = new();

        [SerializeField]
        private List<SequentialSoundSourceList> footSounds = new List<SequentialSoundSourceList>();
        private int curFootSoundIndex = 0;
        private FloorType curFloorType = FloorType.GRASS;
        private WalkType curWalkType = WalkType.WALK;
        private Coroutine footSoundCoroutine = null;
        private void Awake()
        {
            soundObjects.Add(Sound.Sword, transform.Find("SwordSound").GetComponent<SoundObject>());
            soundObjects.Add(Sound.Foot, transform.Find("FootSound").GetComponent<SoundObject>());
            soundBools.Add(Sound.Foot, false);
        }

        public void SetRun(bool isRun)
        {
            UpdateFootSound(walkType: isRun ? WalkType.RUN : WalkType.WALK);
        }

        public void Trigger(Sound sound)
        {
            soundObjects[sound].SetLoop(false);
            StartCoroutine(soundObjects[sound].Play());
        }

        public void SetBool(Sound sound, bool active)
        {
            if (active && !soundBools[sound])
            {
                soundBools[sound] = true;
                footSoundCoroutine = StartCoroutine(PlaySequentialSound(sound));
            }
            else if (!active && soundBools[sound])
            {
                StopCoroutine(footSoundCoroutine);
                footSoundCoroutine = null;
                soundBools[sound] = false;
                soundObjects[sound].Stop();
            }
        }

        private IEnumerator PlaySequentialSound(Sound sound)
        {
            int index = 0;
            while (true)
            {
                index %= footSounds[curFootSoundIndex].GetLength();
                soundObjects[sound].SetSoundSource(footSounds[curFootSoundIndex].GetSoundSource(index));
                yield return soundObjects[sound].Play();
                index += 1;
            }
        }

        private void UpdateFootSound(FloorType floorType = FloorType.DEFAULT, WalkType walkType = WalkType.DEFAULT)
        {
            if (floorType != curFloorType && floorType != FloorType.DEFAULT)
            {
                curFloorType = floorType;
            }
            if (walkType != curWalkType && walkType != WalkType.DEFAULT)
            {
                curWalkType = walkType;
            }
            curFootSoundIndex = (int)curFloorType * 2 + (int)curWalkType;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.name.Contains("Stone"))
            {
                UpdateFootSound(FloorType.STONE);
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.name.Contains("Stone"))
            {
                UpdateFootSound(FloorType.GRASS);
            }
        }
    }
}