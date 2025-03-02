using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using playerCharacter;
using UnityEngine;
using Random = UnityEngine.Random;

public enum DirectionType
{
    FRONT,
    BACK,
    LEFT,
    RIGHT
}

namespace Creature
{
    public abstract class Creature : MonoBehaviour, IAttackable
{
    private const float BLINK_DELAY = 0.15f;
    
    protected float maxHp;
    [SerializeField] protected float currentHp;
    public float CurrentHp {get { return currentHp; }}
    public float currentShield;
    public float CurrentShield {get { return currentShield; }}

    public float damage;
    
    protected internal float speed;
    protected float detectionRange;
    public float DetectionRange {get { return detectionRange; }}
    public float MeleeAttackRange {get; protected set;}
    public float RangedAttackRange {get; protected set;}

    private Coroutine _currentStateCoroutine;

    protected Animator _animator;
    private MaterialController _materialController;
    private void Update()
    {
        if (currentState != null)
        {
            currentState.OnStateUpdate();
        }
    }
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
    public DirectionType GetDirectionToPlayer(Vector2 directionToPlayer)
    {
        directionToPlayer.Normalize();
        if (Mathf.Abs(directionToPlayer.x) > Mathf.Abs(directionToPlayer.y))
        {
            return directionToPlayer.x > 0 ? DirectionType.RIGHT : DirectionType.LEFT;
        }
        else
        {
            return directionToPlayer.y > 0 ? DirectionType.FRONT : DirectionType.BACK;
        }
    }
    protected BaseState currentState;
    public void ChangeState()
    {
        if (_currentStateCoroutine != null)
        {
            currentState.OnStateExit();
            StopCoroutine(_currentStateCoroutine);
        }
            
        
        BaseState[] states = States;
        List<int> weights = new();
        int index = 0;
        int randomIndex;
        foreach (BaseState state in states)
        {
            weights.AddRange(Enumerable.Repeat(index++, state.GetWeight()));
        }
        randomIndex = Random.Range(0, weights.Count);
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
        currentState.OnStateExit();
        StopCoroutine(_currentStateCoroutine);
         
        yield return new WaitForSeconds(5f);
        ChangeState();
    }
    
    public void TrackPlayer()
    {
        float step = speed * Time.deltaTime;
        Vector3 targetPosition = PlayerCharacter.Instance.transform.position;
        Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, step);
        transform.position = newPosition;
    }
    
    public void TrackPath(List<Vector2> path)
    {
        if (path == null || path.Count == 0)
        {
            return;
        }
        
        Vector2 currentTarget = path[0];
        
        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = new Vector3(currentTarget.x, currentTarget.y, currentPosition.z);
        
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(currentPosition, targetPosition, step);
        
        if (Vector3.Distance(currentPosition, targetPosition) < 0.1f)
        {
            path.RemoveAt(0);
        }
    }
    
    public IEnumerator BackStep(float targetDistance)
     {
         Vector3 playerPosition = PlayerCharacter.Instance.transform.position;
         float backStepSpeed = 50;
         Vector3 direction = (transform.position - playerPosition).normalized;
         Rigidbody2D rb = GetComponent<Rigidbody2D>();
         LayerMask obstacleLayer = LayerMask.GetMask("Obstacle");
         float currentDistance = Vector3.Distance(transform.position, playerPosition);
         RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, targetDistance, obstacleLayer);
         int count=0;
         while (hit.collider != null && count<360)
         {
             float angle = UnityEngine.Random.Range(0,360f);
             direction = Quaternion.Euler(0, 0, angle) * direction;
             hit = Physics2D.Raycast(transform.position, direction, targetDistance, obstacleLayer);
             count++;
         }
         if(hit.collider == null)
         {
             currentDistance = targetDistance;
             count = 0;
             float deltaTime = 0.02f;
             while (currentDistance > 0 && count < 100000)
             {
                 Vector3 deltaDistance = direction * backStepSpeed * deltaTime;
                 currentDistance -= deltaDistance.magnitude;
                 Vector3 newPosition = transform.position + deltaDistance;
                 rb.MovePosition(newPosition);
                 count++;
                 yield return new WaitForSeconds(deltaTime);
             }
         }
         else
         {
             yield return null;
         }
     }

    public abstract class BaseState
    {
        public Creature creature;
        public abstract int GetWeight();

        public abstract IEnumerator StateCoroutine();
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

    public virtual void StartMob()
    {
        currentHp = maxHp;
        currentShield = 0;
        ChangeState();
    }
}
}


