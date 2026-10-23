using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob
{
    public class StateMachineMob : Mob
    {
        protected Coroutine _currentStateCoroutine;
        protected BaseState currentState;
        private int stateVersion;
        protected virtual void Update()
        {
            if (currentState != null)
            {
                currentState.OnStateUpdate();
            }
        }
        public override void StartMob()
        {
            currentHp = maxHp;
            currentShield = 0;
            ChangeState();
        }
        
        protected void StopCurrentState()
        {
            stateVersion++;
            var previousState = currentState;
            var previousCoroutine = _currentStateCoroutine;
            currentState = null;
            _currentStateCoroutine = null;
            if (previousCoroutine != null) StopCoroutine(previousCoroutine);
            previousState?.OnStateExit();
        }

        private void StartState(BaseState state)
        {
            if (state == null) return;
            int version = ++stateVersion;
            currentState = state;
            Coroutine started = StartCoroutine(state.StateCoroutine());
            // A state may transition synchronously before its first yield.
            if (stateVersion == version) _currentStateCoroutine = started;
            else if (started != null) StopCoroutine(started);
        }

        public override IEnumerator Stun(float stunTime)
        {
            StopCurrentState();
         
            yield return new WaitForSeconds(stunTime);
            ChangeState();
        }
        public void ChangeState()
        {
            StopCurrentState();
            List<BaseState> weightedStates = new();
            BaseState[] states = States;
            foreach (BaseState state in states)
            {
                int weight = state.GetWeight();
                for (int i = 0; i < weight; i++)
                {
                    weightedStates.Add(state);
                }
            }
            if (weightedStates.Count > 0)
            {
                int randomIndex = Random.Range(0, weightedStates.Count);
                StartState(weightedStates[randomIndex]);
            }
        }
        public void ChangeState(BaseState state)
        {
            StopCurrentState();
           
            StartState(state);
        }
        public void ChangeState(Dictionary<Type, int> nextStateWeights)
        {
            StopCurrentState();
            List<BaseState> weightedStates = new();
            for (int i = 0; i < States.Length; i++)
            {
                var state = States[i];
                if (nextStateWeights.TryGetValue(state.GetType(), out int weight) && weight > 0 && state.CanEnterState())
                {
                    weightedStates.AddRange(Enumerable.Repeat(state, weight));
                }
            }
            if (weightedStates.Count > 0)
            {
                int randomIndex = Random.Range(0, weightedStates.Count);
                StartState(weightedStates[randomIndex]);
            }
            else
            {
                ChangeState();
            }
        }
        
        public abstract class BaseState
        {
            public StateMachineMob mob;
            public abstract int GetWeight();
            public abstract IEnumerator StateCoroutine();
            public virtual bool CanEnterState()
            {
                return true;
            }
            public virtual void OnStateUpdate()
            { 
            }
            public virtual void OnStateExit()
            {
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
        }
        private BaseState[] _states;
        public BaseState[] States
        {
            get
            {
                if (_states == null)
                {
                    List<BaseState> states = new List<BaseState>();
                    Type parentType = GetType();
                    Type[] stateTypes = parentType.GetNestedTypes();
                    foreach (Type type in stateTypes)
                    {
                        if (!type.IsAbstract && typeof(BaseState).IsAssignableFrom(type))
                        {
                            states.Add(Activator.CreateInstance(type) as BaseState);
                            states[states.Count - 1].mob = this;
                        }
                    }
                    _states = states.ToArray();
                }
                return _states;
            }
        }
    }
}
