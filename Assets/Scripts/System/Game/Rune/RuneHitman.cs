using System.Collections;
using System.Collections.Generic;
using playerCharacter;
using UnityEngine;
using Visual.Effect;

//Super-Crazy Primal Class...
public class RuneHitman : SingletonObject<RuneHitman>
{
    public IEnumerator Activate_Breeze()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);
            EffectCreator.Instance.CreateEffect(1,PlayerCharacter.Instance.transform);
            PlayerCharacter.Instance.Heal(PlayerCharacter.Instance.stat.healthMax.TotalValue);
            Debug.Log("Rune_Breeze Healed");    
        }
    }
    
    
    public void Activate_SwordAuraExercise()
    {
    }

    public void Activate_SwordAuraExercise_Effecient()
    {
    }

    public void Activate_SwordAuraExercise_LimitBreak()
    {
        //if...
    }

    public void Activate_StoneArmor()
    {
        
    }
}
