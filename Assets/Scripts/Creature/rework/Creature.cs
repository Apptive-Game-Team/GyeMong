using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using playerCharacter;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class Creature : MonoBehaviour, IAttackable
{
    private const float BLINK_DELAY = 0.15f;
    
    protected float maxHp;
    protected float currentHp;
    public float CurrentHp {get { return currentHp; }}
    public float currentShield;
    public float CurrentShield {get { return currentShield; }}

    public float damage;
    
    protected float speed;
    protected float detectionRange;
    public float MeleeAttackRange {get; protected set;}
    public float RangedAttackRange {get; protected set;}

    private Coroutine _currentStateCoroutine;

    protected Animator _animator;
    private MaterialController _materialController;
    public MaterialController MaterialController
    {
        get
        {
            if (_materialController == null)
            {
                _materialController = GetComponent<MaterialController>();
            }
            return _materialController;
        }
    }
    public Animator Animator
    {
        get
        {
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }
            return _animator;
        }
    }
    
    public float DistanceToPlayer => Vector3.Distance(transform.position, PlayerCharacter.Instance.transform.position);
    public Vector3 DirectionToPlayer => (PlayerCharacter.Instance.transform.position - transform.position).normalized;

    public void ChangeState()
    {
        if (_currentStateCoroutine != null)
            StopCoroutine(_currentStateCoroutine);
        
        BaseState[] states = States;
        BaseState nextState = null;
        List<int> weights = new();
        int index = 0;
        int randomIndex;
        foreach (BaseState state in states)
        {
            weights.AddRange(Enumerable.Repeat(index++, state.GetWeight()));
        }
        randomIndex = Random.Range(0, weights.Count);
        _currentStateCoroutine = StartCoroutine(states[weights[randomIndex]].StateCoroutine());
    }
    
    protected IEnumerator Blink()
    {
        MaterialController.SetMaterial(MaterialController.MaterialType.HIT);
        MaterialController.SetFloat(1);
        yield return new WaitForSeconds(BLINK_DELAY);
        if (MaterialController.GetCurrentMaterialType() == MaterialController.MaterialType.HIT)
        {
            MaterialController.SetFloat(0);
        }
    }
    
    public virtual IEnumerator Stun()
    {
         StopCoroutine(_currentStateCoroutine);
         yield return new WaitForSeconds(5f);
         ChangeState();
    }

    public abstract class BaseState
    {
        public Creature creature;
        public abstract int GetWeight();
        public abstract IEnumerator StateCoroutine();
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
                        states[states.Count - 1].creature = this;
                    }
                }
                _states = states.ToArray();
            }
            return _states;
        }
    }

    public virtual void OnAttacked(float damage)
    {
        if (currentShield >= damage)
         {
             currentShield -= damage;
         }
         else
         {
             float temp = currentShield;
             currentShield = 0;
             MaterialController.SetMaterial(MaterialController.MaterialType.DEFAULT);
             StartCoroutine(Blink());
             currentHp -= (damage-temp);
         }
    }
}

