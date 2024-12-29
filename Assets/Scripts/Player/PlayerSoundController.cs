using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace playerCharacter
{
    public enum PlayerSoundType
    {
        SWORD_SWING = 0,
        SWORD_ATTACK = 1,
        SWORD_DEFEND_START = 2,
        FOOT = 3,
        DASH = 4,
        SWORD_DEFEND_HIT = 5,
        SWORD_DEFEND_PERFECT = 6,
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

    public class PlayerSoundController : MonoBehaviour
    {
        private Dictionary<PlayerSoundType, SoundObject> soundObjects = new();
        private Dictionary<PlayerSoundType, bool> soundBools = new();

        [SerializeField]
        private List<SequentialSoundSourceList> footSounds = new List<SequentialSoundSourceList>();
        private int curFootSoundIndex = 0;
        private FloorType curFloorType = FloorType.GRASS;
        private WalkType curWalkType = WalkType.WALK;
        private Coroutine footSoundCoroutine = null;
        private void Awake()
        {
            soundObjects.Add(PlayerSoundType.DASH, transform.Find("DashSound").GetComponent<SoundObject>());
            soundObjects.Add(PlayerSoundType.SWORD_SWING, transform.Find("SwordSwingSound").GetComponent<SoundObject>());
            soundObjects.Add(PlayerSoundType.SWORD_ATTACK, transform.Find("SwordAttackSound").GetComponent<SoundObject>());
            soundObjects.Add(PlayerSoundType.SWORD_DEFEND_START, transform.Find("SwordDefendSound").GetComponent<SoundObject>());
            soundObjects.Add(PlayerSoundType.SWORD_DEFEND_HIT, transform.Find("SwordDefendHitSound").GetComponent<SoundObject>());
            soundObjects.Add(PlayerSoundType.SWORD_DEFEND_PERFECT, transform.Find("SwordDefendPerfectSound").GetComponent<SoundObject>());
            soundObjects.Add(PlayerSoundType.FOOT, transform.Find("FootSound").GetComponent<SoundObject>());
        }

        public void SetRun(bool isRun)
        {
            UpdateFootSound(walkType: isRun ? WalkType.RUN : WalkType.WALK);
        }

        public void Trigger(PlayerSoundType sound)
        {
            soundObjects[sound].SetLoop(false);
            StartCoroutine(soundObjects[sound].Play());
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void SetBool(PlayerSoundType sound, bool active)
        {
            try
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
            catch (KeyNotFoundException)
            {
                soundBools.Add(sound, false);
                SetBool(sound, active);
            }
            
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator PlaySequentialSound(PlayerSoundType sound)
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