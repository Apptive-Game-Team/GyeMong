using System.Collections;
using playerCharacter;
using TMPro;
using UnityEngine;

public class Elf : Boss
{
{ 
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private GameObject seedPrefab;
    [SerializeField] private GameObject vinePrefab;
    [SerializeField] private GameObject meleeAttackPrefab;
    Vector3 meleeAttackPrefabPos;
    
    private FootSoundController footSoundController;
    [SerializeField] private SoundObject arrowSoundObject;
    [SerializeField] private SoundObject vineSoundObject;
    
    protected override void Initialize()
    {
         maxPhase = 2;
         maxHps.Add(100f);
         maxHps.Add(200f);
         currentHp = maxHps[currentPhase];
         damage = 20f;
         speed = 2f;
         currentShield = 0f;
         detectionRange = 10f;
         MeleeAttackRange = 2f;
         RangedAttackRange = 6f;
         meleeAttackPrefab.SetActive(false);
         footSoundController = transform.Find("FootSoundObject").GetComponent<FootSoundController>();
    }
    
    private void RotateArrowTowardsPlayer(GameObject arrow)
    {
        Vector3 direction = DirectionToPlayer; 
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
     }
}
