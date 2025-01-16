using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Boss : Creature
{
    protected int currentPhase = 0;
    protected int maxPhase;
    protected List<float> maxHps = new List<float>();
    
    public int CurrentPhase {get { return currentPhase; }}
    public float CurrentMaxHp {get {
        try
        {
            return maxHps[currentPhase];
        }
        catch (Exception)
        {
            return 100;
        }
    }}

    protected void Start()
    {
        Initialize();
    }

    protected abstract void Initialize();

    public override void OnAttacked(float damage)
    {
        base.OnAttacked(damage);
        CheckPhaseTransition();
    }
    
    protected void CheckPhaseTransition()
    {
        if (currentHp <= 0)
        {
            TransPhase();
        }
    }
    
    protected void TransPhase()
    {
        if (currentPhase < maxHps.Count-1)
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
    
    public override IEnumerator Stun()
    {
        currentShield = 0f;
        MaterialController.SetMaterial(MaterialController.MaterialType.DEFAULT);
        yield return base.Stun();
    }
    
    protected void Die() // Boss Clear Event
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
