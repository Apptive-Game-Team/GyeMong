using System.Collections;
using System.Collections.Generic;
using playerCharacter;
using UnityEngine;
using Visual.Effect;
using Creature.Boss;

//Super-Crazy Primal Class...
public class BuffHitman : MonoBehaviour
{
    public void ActiveRune_Breeze(BuffData buff)
    {
        EffectCreator.Instance.CreateEffect(1,PlayerCharacter.Instance.transform);
        PlayerCharacter.Instance.Heal(buff.amount1);
        Debug.Log("Rune_Breeze Healed");
    }

    public void ActiveRune_Vine(BuffData buff)
    {
        Debug.Log("Rune_Vine Activated");
        Collider2D targetCollider = Physics2D.OverlapCircle(PlayerCharacter.Instance.transform.position, buff.amount1);
        Boss boss = FindObjectOfType<Boss>();
        if(boss != null && buff.amount1 > Vector3.Distance(PlayerCharacter.Instance.transform.position, boss.transform.position))
        {
            BuffComponent buffComp = boss.GetComponent<BuffComponent>();
            EffectCreator.Instance.CreateEffect(2, buffComp.transform);
            buffComp!.AddBuff(new BuffData() { buffType = BuffType.BUFF_SNARE, duration = buff.amount2, disposeMode = BuffDisposeMode.TEMPORARY });
            Debug.Log(buffComp.gameObject.name + "Snared by " + buff.amount2);
        }
    }

    public void ActiveRune_Flower(BuffData buff)
    {
        //On Attack Hit, Instantiate a Flower Prefab that has hitbox, when someone on collide, damage 20% of originals.
        //when Attack Implemented....
    }
}
