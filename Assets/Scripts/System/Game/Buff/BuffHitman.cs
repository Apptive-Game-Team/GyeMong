using System.Collections;
using System.Collections.Generic;
using playerCharacter;
using UnityEngine;

//Super-Crazy Primal Class...
public class BuffHitman : MonoBehaviour
{
    public void ActiveRune_Breeze(BuffData buff)
    {
        EffectCreator.Instance.CreateEffect(1,PlayerCharacter.Instance.transform);
        PlayerCharacter.Instance.Heal(buff.amount1);
        Debug.Log("Rune_Breeze Healed");
    }

    public void ActiveRune_Flower(BuffData buff)
    {
        //On Attack Hit, Instantiate a Flower Prefab that has hitbox, when someone on collide, damage 20% of originals.
        //when Attack Implemented....
    }
}
