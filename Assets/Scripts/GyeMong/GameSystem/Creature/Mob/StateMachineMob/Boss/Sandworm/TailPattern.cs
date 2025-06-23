using System;
using System.Collections;
using System.Collections.Generic;
using GyeMong.GameSystem.Creature.Player;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using GyeMong.SoundSystem;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Sandworm
{
    public class TailPattern : MonoBehaviour
    {
        [SerializeField] private GameObject tailImage;
        [SerializeField] private GameObject indicatorImage;
        [SerializeField] private GameObject sandworm;
        private float _attackDelay;
        private float _nextAttackDelay;
        private float _detroyDelay;
        private float _spawnPosAdj;
        private float _burstDuration;
        private Coroutine _curCoroutine;

        private void Awake()
        {
            _spawnPosAdj = 0.1f;
            _attackDelay = 1.2f;
            _nextAttackDelay = 2.4f;
            _detroyDelay = 0.5f;
            _burstDuration = 0.5f;
        }

        private IEnumerator TailAttackPattern()
        {
            while (true)
            {
                Vector3 targetPos = PlayerCharacter.Instance.transform.position;
                Vector3 sandwormDir = Vector3.Distance(targetPos, sandworm.transform.position) > 1f ? 
                    (sandworm.transform.position - targetPos).normalized : Vector3.zero;
                Vector3 spawnPos = targetPos + sandwormDir * _spawnPosAdj;
                
                GameObject indicator = Instantiate(indicatorImage, spawnPos, Quaternion.Euler(0f, 0f, 90f));
                Destroy(indicator, _attackDelay);
                yield return new WaitForSeconds(_attackDelay);
                
                GameObject tail = Instantiate(tailImage, spawnPos, Quaternion.Euler(-90f, 0f, 0f));
                Sound.Play("ENEMY_Map_Tail_Attack");
                Destroy(tail, _detroyDelay);
                FlipTail(tail, PlayerCharacter.Instance.transform.position.x < spawnPos.x);
                BurstEffect(tail);

                yield return new WaitForSeconds(_nextAttackDelay);
            }
        }

        public void StartPattern()
        {
            _curCoroutine = StartCoroutine(TailAttackPattern());
        }

        public void StopPattern()
        {
            if (_curCoroutine != null)
                StopCoroutine(_curCoroutine);
        }

        private void FlipTail(GameObject tail, bool flag)
        {
            tail.GetComponent<SpriteRenderer>().flipX = flag;
        }

        private void BurstEffect(GameObject tail)
        {
            Vector3 burstRotation = new Vector3(0f, 0f, 0f);
            tail.transform.DORotate(burstRotation, _burstDuration).SetEase(Ease.InOutSine);
        }
    }
}
