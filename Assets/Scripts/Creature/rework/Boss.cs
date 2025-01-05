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
    public float CurrentMaxHp {get { return maxHps[currentPhase]; }}

    protected void CheckPhaseTransition()
    {
        if (currentHp <= 0)
        {
            TransPhase();
        }
    }
    
    protected void TransPhase()
    {
        if (currentPhase < maxPhase)
        {
            currentPhase++;
            StopAllCoroutines();
            MaterialController.SetMaterial(MaterialController.MaterialType.DEFAULT);
            // StartCoroutine(ChangingPhase());
        }
        else
        {
            Die();
        }
    }

    public void Stun()
    {
        
    }
    
    protected void Die() // Boss Clear Event
    {
        try
        {
            GameObject.Find("BossDownEventObject").gameObject.GetComponent<EventObject>().Trigger();
        }
        catch
        {
            Debug.Log("BossDownEventObject not found");
        }
    }
}
