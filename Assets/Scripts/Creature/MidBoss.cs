using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidBoss : Boss
{
    private void Awake()
    {
        maxHealth = 100f;
        curHealth = maxHealth;
        speed = 1;
        player = GameObject.FindGameObjectWithTag("Player");
    }
    void Start()
    {
        wall.SetActive(false);
    }
    void Update()
    {
        
    }
}
