using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Super-Crazy Primal Class...
public class BuffHitman : MonoBehaviour
{
    public void ActiveRune_Breeze(BuffData buff, BuffTestComponent testComp)
    {
        testComp.playerHealth += buff.amount1;
    }

    public void ActiveRune_Vine(BuffData buff, BuffTestComponent testComp)
    {
        Collider2D targetCollider = Physics2D.OverlapCircle(testComp.transform.position, buff.amount1);
        BuffComponent targetBuffComp = targetCollider.GetComponent<BuffComponent>();
        targetBuffComp.AddBuff(BuffManager.Instance.runeDataList.GetRuneData(2).runeBuff);
    }

    public void ActiveRune_Flower(BuffTestComponent testComp, BuffData buff)
    {
        //On Attack Hit, Instantiate a Flower Prefab that has hitbox, when someone on collide, damage 20% of originals.
        //when Attack Implemented....
    }
}
