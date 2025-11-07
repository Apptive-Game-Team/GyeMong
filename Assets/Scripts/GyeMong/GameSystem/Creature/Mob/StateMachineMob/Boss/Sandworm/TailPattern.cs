using System.Collections;
using UnityEngine;
using DG.Tweening;
using GyeMong.GameSystem.Indicator;
using GyeMong.SoundSystem;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Sandworm
{
    public class TailPattern : MonoBehaviour
    {
        [SerializeField] private GameObject showEffect;
        [SerializeField] private GameObject tailAttack;
        [SerializeField] private GameObject sandworm;
        private float _attackDelay;
        private float _nextAttackDelay;
        private float _detroyDelay;
        private float _spawnPosAdj;
        private Coroutine _curCoroutine;

        private void Awake()
        {
            _spawnPosAdj = 0.3f;
            _attackDelay = 1f;
            _nextAttackDelay = 3f;
        }

        private IEnumerator TailAttackPattern()
        {
            while (true)
            {
                Vector3 targetPos = SceneContext.Character.transform.position;
                Vector3 sandwormDir = Vector3.Distance(targetPos, sandworm.transform.position) > 1f ? 
                    (sandworm.transform.position - targetPos).normalized : Vector3.zero;
                Vector3 spawnPos = targetPos + sandwormDir * _spawnPosAdj;
                
                StartCoroutine(IndicatorGenerator.Instance.GenerateIndicator(tailAttack, spawnPos, Quaternion.Euler(0f, 0f, 0f), _attackDelay));
                yield return new WaitForSeconds(_attackDelay);
                
                GameObject tail = Instantiate(tailAttack, spawnPos, Quaternion.identity);
                Instantiate(showEffect, spawnPos, Quaternion.identity);
                Sound.Play("ENEMY_Map_Tail_Attack");

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
    }
}
