using System.Collections;
using System.Collections.Generic;
using System.Sound;
using UnityEngine;
namespace playerCharacter
{
    public class FootSoundController : MonoBehaviour
    {
        [SerializeField]
        private List<SequentialSoundSourceList> footSounds = new List<SequentialSoundSourceList>();
        private int curFootSoundIndex = 0;
        private FloorType curFloorType = FloorType.GRASS;
        private WalkType curWalkType = WalkType.WALK;
        private Coroutine footSoundCoroutine = null;
        
        private bool isPlaying = false;
        private SoundObject _soundObject;
        private void Awake()
        {
            _soundObject = GetComponent<SoundObject>();
        }

        public void SetRun(bool isRun)
        {
            UpdateFootSound(walkType: isRun ? WalkType.RUN : WalkType.WALK);
        }

        public void Trigger(PlayerSoundType sound)
        {
            _soundObject.SetLoop(false);
            StartCoroutine(_soundObject.Play());
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void SetBool(bool active)
        {
            if (active && !isPlaying)
            {
                isPlaying = true;
                footSoundCoroutine = StartCoroutine(PlaySequentialSound());
            }
            else if (!active && isPlaying)
            {
                StopCoroutine(footSoundCoroutine);
                footSoundCoroutine = null;
                isPlaying = false;
                _soundObject.Stop();
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator PlaySequentialSound()
        {
            int index = 0;
            while (true)
            {
                index %= footSounds[curFootSoundIndex].GetLength();
                _soundObject.SetSoundSource(footSounds[curFootSoundIndex].GetSoundSource(index));
                yield return _soundObject.Play();
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