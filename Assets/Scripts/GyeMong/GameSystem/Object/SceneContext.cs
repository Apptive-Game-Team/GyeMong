using System;
using System.Collections;
using System.Collections.Generic;
using GyeMong.GameSystem.Creature.Player;
using UnityEngine;

public class SceneContext : MonoBehaviour
{
    private static PlayerCharacter _character;

    public static PlayerCharacter Character
    {
        get
        {
            if (_character == null)
            {
                _character = FindObjectOfType<PlayerCharacter>();
            }
            return _character;
        }
    }

    private void Awake()
    {
        _character = FindObjectOfType<PlayerCharacter>();
    }
}