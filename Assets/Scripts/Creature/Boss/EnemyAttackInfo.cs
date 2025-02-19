using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackInfo : MonoBehaviour
{
    public float damage;
    public SoundObject soundObject;
    public bool isDestroyOnHit;
    public bool grazable;
    public bool grazed;
    public bool isAttacked;
    public bool isMultiHit;
    public float multiHitDelay;

    public void Initialize(float damage, SoundObject soundObject, bool isDestroyOnHit, bool grazable, bool grazed = false, bool isAttacked = false, bool isMultiHit = false, float multiHitDelay = 0f)
    {
        this.damage = damage;
        this.soundObject = soundObject;
        this.isDestroyOnHit = isDestroyOnHit;
        this.grazable = grazable;
        this.grazed = grazed;
        this.isAttacked = isAttacked;
        this.isMultiHit = isMultiHit;
        this.multiHitDelay = multiHitDelay;
    }
}
