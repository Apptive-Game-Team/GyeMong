using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidBoss : Boss
{   
    // Start is called before the first frame update
    private void Awake()
    {
        maxHealth = 100f;
        curHealth = maxHealth;
        speed = 1;
        DetectPlayer();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
