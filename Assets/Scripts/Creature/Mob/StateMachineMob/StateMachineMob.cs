using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Creature.Mob.StateMachineMob
{
    public class StateMachineMob : Mob
    {
        protected Coroutine _currentStateCoroutine;
        protected BaseState currentState;
        private void Update()
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
        
        public override IEnumerator Stun(float stunTime)
        { 
            currentState.OnStateExit();
            StopCoroutine(_currentStateCoroutine);
         
            yield return new WaitForSeconds(stunTime);
            ChangeState();
        }
        public void ChangeState()
        {
            if (_currentStateCoroutine != null)
            {
                currentState.OnStateExit();
                StopCoroutine(_currentStateCoroutine);
            }
            if (currentState == null)
            {
                SetInitialState();
            }
            else if (currentState is Boss.Boss.CoolDownState)
            {
                ChangeStateForBoss();
            }
            else
            {
                ChangeStateForNormal();
            }
        }
        private void ChangeStateForBoss()
        {
            List<Type> weightedStates = new();
            Dictionary<Type, int> nextStateWeights = ((Boss.Boss.CoolDownState)currentState).GetNextStateWeights();

            foreach (var state in States)
            {
                if (nextStateWeights.TryGetValue(state.GetType(), out int weight) && state.CanEnterState())
                {
                    weightedStates.AddRange(Enumerable.Repeat(state.GetType(), weight));
                }
            }
            System.Type nextStateType = weightedStates[Random.Range(0, weightedStates.Count)];
            currentState = States.First(s => s.GetType() == nextStateType);
            _currentStateCoroutine = StartCoroutine(currentState.StateCoroutine());
        }
        private void ChangeStateForNormal()
        {
            List<int> weights = new();
            int index = 0;
            BaseState[] states = States;

            foreach (BaseState state in states)
            {
                weights.AddRange(Enumerable.Repeat(index++, state.GetWeight()));
            }

            int randomIndex = Random.Range(0, weights.Count);
            currentState = states[weights[randomIndex]];
            _currentStateCoroutine = StartCoroutine(states[weights[randomIndex]].StateCoroutine());
        }
        
        
        private void SetInitialState()
        {
            BaseState[] states = States;
            List<int> weights = new();
            int index = 0;

            foreach (BaseState state in states)
            {
                weights.AddRange(Enumerable.Repeat(index++, state.GetWeight()));
            }

            int randomIndex = Random.Range(0, weights.Count);
            currentState = states[weights[randomIndex]];
            _currentStateCoroutine = StartCoroutine(states[weights[randomIndex]].StateCoroutine());
        }
        public void ChangeState(BaseState state)
        {
            if (_currentStateCoroutine != null)
            {
                currentState.OnStateExit();
                StopCoroutine(_currentStateCoroutine);
            }
           
            currentState = state;
        
            _currentStateCoroutine = StartCoroutine(state.StateCoroutine());
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
                        if (!type.IsAbstract)
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
