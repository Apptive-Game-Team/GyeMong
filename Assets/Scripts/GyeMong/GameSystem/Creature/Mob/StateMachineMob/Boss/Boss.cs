using System;
using System.Collections;
using System.Collections.Generic;
using GyeMong.EventSystem;
using GyeMong.EventSystem.Interface;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Component.Material;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss
{
    public abstract class Boss : StateMachineMob, IEventTriggerable
    {
        protected int currentPhase = 0;
        protected int maxPhase;
        protected List<float> maxHps = new List<float>();

        public int CurrentPhase { get { return currentPhase; } }
        public float CurrentMaxHp
        {
            get
            {
                try
                {
                    return maxHps[currentPhase];
                }
                catch (Exception)
                {
                    return 100;
                }
            }
        }
        protected void Start()
        {
            Initialize();
        }

        protected abstract void Initialize();

        public override void OnAttacked(float damage)
        {
            if (currentHp > 0)
            {
                base.OnAttacked(damage);
                CheckPhaseTransition();
            }
        }

        protected void CheckPhaseTransition()
        {
            if (currentHp <= 0)
            {
                TransPhase();
            }
        }

        protected virtual void TransPhase()
        {
            if (currentPhase < maxHps.Count - 1)
            {
                currentPhase++;
                StopAllCoroutines();
                MaterialController.SetMaterial(MaterialController.MaterialType.DEFAULT);
                StartCoroutine(ChangingPhase());
            }
            else
            {
                MaterialController.SetMaterial(MaterialController.MaterialType.DEFAULT);
                Die();
            }
        }

        public IEnumerator ChangingPhase()
        {
            currentHp = CurrentMaxHp;
            GameObject.Find("PhaseChangeObj").GetComponent<EventObject>().Trigger();
            yield return null;
        }

        public override IEnumerator Stun(float duration)
        {
            currentShield = 0f;
            MaterialController.SetMaterial(MaterialController.MaterialType.DEFAULT);
            currentState.OnStateExit();
            StopCoroutine(_currentStateCoroutine);
            yield return new WaitForSeconds(duration);
            ChangeState();
        }

        public virtual void Die()
        {
            try
            {
                currentState.OnStateExit();
                StopAllCoroutines();
                GameObject.Find("BossDownEventObject").gameObject.GetComponent<EventObject>().Trigger();
            }
            catch
            {
                Debug.Log("BossDownEventObject not found");
            }
        }
        public void Trigger()
        {
            ChangeState();
        }
    }
}
