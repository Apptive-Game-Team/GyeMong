using System;
using System.Collections;
using System.Collections.Generic;
using playerCharacter;
using UnityEngine;

public abstract class Creature : MonoBehaviour
{
    private const float BLINK_DELAY = 0.15f;
    
    protected float maxHp;
    protected float currentHp;
    public float CurrentHp {get { return currentHp; }}
    public static float currentShield;
    public float CurrentShield {get { return currentShield; }}

    public float damage;
    
    protected float speed;
    protected float detectionRange;
    public float MeleeAttackRange {get; protected set;}
    public float RangedAttackRange {get; protected set;}
    
    private Animator _animator;
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

    public abstract void TakeDamage(float damage);
    public abstract void ChangeState();
    
    protected IEnumerator Blink()
    {
        MaterialController.SetMaterial(MaterialController.MaterialType.HIT);
        MaterialController.SetFloat(1);
        yield return new WaitForSeconds(BLINK_DELAY);
        MaterialController.SetMaterial(MaterialController.MaterialType.HIT);
        MaterialController.SetFloat(0);
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
                    }
                }
                _states = states.ToArray();
            }
            return _states;
        }
    }
}

