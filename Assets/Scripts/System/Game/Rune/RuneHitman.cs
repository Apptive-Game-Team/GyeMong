using System.Collections;
using System.Collections.Generic;
using playerCharacter;
using UnityEngine;

//Super-Crazy Primal Class...
public class RuneHitman : SingletonObject<RuneHitman>
{
    public IEnumerator Activate_Breeze()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);
            EffectCreator.Instance.CreateEffect(1,PlayerCharacter.Instance.transform);
            PlayerCharacter.Instance.Heal(PlayerCharacter.Instance.maxHealth * 0.01f);
            Debug.Log("Rune_Breeze Healed");    
        }
    }
    
    
    public void Activate_SwordAuraExercise()
    {
        PlayerCharacter.Instance.swordAuraCoef = 1.5f;
    }

    public void Activate_SwordAuraExercise_Effecient()
    {
        PlayerCharacter.Instance.skillUsageGauge *= 0.9f;
    }

    public void Activate_SwordAuraExercise_LimitBreak()
    {
        //if...
    }

    public void Activate_StoneArmor()
    {
        
    }
}
