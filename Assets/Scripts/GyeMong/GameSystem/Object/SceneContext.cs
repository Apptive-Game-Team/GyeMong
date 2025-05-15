using System;
using System.Collections;
using System.Collections.Generic;
using GyeMong.GameSystem.Creature.Player;
using UnityEngine;

public class SceneContext : MonoBehaviour
{
    public static PlayerCharacter Character;

    private void Awake()
    {
        Character = GetComponent<PlayerCharacter>();
    }
}
