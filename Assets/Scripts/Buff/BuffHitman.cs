using System.Collections;
using System.Collections.Generic;
using playerCharacter;
using UnityEngine;

//Super-Crazy Primal Class...
public class BuffHitman : MonoBehaviour
{
    public void ActiveRune_Breeze(BuffData buff)
    {
        PlayerCharacter.Instance.Heal(buff.amount1);
        Debug.Log("Rune_Breeze Healed");
    }

    public void ActiveRune_Vine(BuffData buff)
    {
        Debug.Log("Rune_Vine Activated");
        Collider2D targetCollider = Physics2D.OverlapCircle(PlayerCharacter.Instance.transform.position, buff.amount1);
        BuffComponent targetBuffComp = targetCollider.GetComponent<BuffComponent>();
        targetBuffComp!.AddBuff(new BuffData(){buffType = BuffType.BUFF_SNARE, duration = buff.amount2, disposeMode = BuffDisposeMode.TEMPORARY});
        Debug.Log(targetBuffComp.gameObject.name + "Snared by " + buff.amount2);
    }

    public void ActiveRune_Flower(BuffData buff)
    {
        //On Attack Hit, Instantiate a Flower Prefab that has hitbox, when someone on collide, damage 20% of originals.
        //when Attack Implemented....
    }
}
