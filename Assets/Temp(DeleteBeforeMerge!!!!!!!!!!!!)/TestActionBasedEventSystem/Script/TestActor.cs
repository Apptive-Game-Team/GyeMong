using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestActor : MonoBehaviour
{
    public static event Action<bool> OnCombatStateChanged; // 🔥 Event for combat status change
    public static event Action OnPlayerGrazed;

    private bool _isInCombat;
    private float lastCombatTime;
    private float combatDuration = 1f; // Time before exiting combat after last attack/damage

    public bool IsInCombat
    {
        get => _isInCombat;
        private set
        {
            if (_isInCombat != value) // 🔥 Only trigger event if combat state actually changes
            {
                _isInCombat = value;
                OnCombatStateChanged?.Invoke(_isInCombat); // 🔥 Notify all listeners
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            OnGraze();
        }
        
        if (_isInCombat && Time.time - lastCombatTime > combatDuration)
        {
            ExitCombat();
        }
    }

    public void EnterCombat()
    {
        lastCombatTime = Time.time;
        IsInCombat = true;  // 🔥 Triggers event
        Debug.Log($"EnterCombat at '{lastCombatTime}'");
    }

    public void ExitCombat()
    {
        IsInCombat = false; // 🔥 Triggers event
        Debug.Log($"EnterCombat at '{lastCombatTime}'");
    }

    public void OnGraze()
    {
        OnPlayerGrazed?.Invoke();
        EnterCombat(); // Call when the player attacks
    }
    public void OnHit() => EnterCombat(); // Call when the player takes damage
    
    
}
