using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Creature.Mob.StateMachineMob.Boss
{
    public abstract class Boss : StateMachineMob
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
        public abstract class CoolDownState : BaseState
        {
            protected float cooldownTime = 0f;
            protected float lastUsedTime = 0f;
            public override bool CanEnterState()
            {
                return Time.time - lastUsedTime >= cooldownTime;
            }
            public override void OnStateExit()
            {
                lastUsedTime = Time.time;
            }
            public virtual Dictionary<Type, int> GetNextStateWeights()
            {
                return new Dictionary<Type, int>();
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
            yield return new WaitForSeconds(2f);
            ChangeState();
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

        protected virtual void Die()
        {
            try
            {
                StopAllCoroutines();
                GameObject.Find("BossDownEventObject").gameObject.GetComponent<EventObject>().Trigger();
            }
            catch
            {
                Debug.Log("BossDownEventObject not found");
            }
        }
    }
}
