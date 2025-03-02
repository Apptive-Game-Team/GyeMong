using Creature.Minion.Slime;
using UnityEngine;

[CreateAssetMenu(fileName = "SlimeSprites", menuName = "ScriptableObject/SlimeSprites")]
public class SlimeSprites : ScriptableObject
{
    public Sprite[] idleSprites;
    public Sprite[] rangedAttackSprites;
    public Sprite[] meleeAttackSprites;
    public Sprite[] dieSprites;
    
    public Sprite[] GetSprite(SlimeAnimator.AnimationType type)
    {
        switch (type)
        {
            case SlimeAnimator.AnimationType.IDLE:
                return idleSprites;
            case SlimeAnimator.AnimationType.MELEE_ATTACK:
                return meleeAttackSprites;
            case SlimeAnimator.AnimationType.RANGED_ATTACK:
                return rangedAttackSprites;
            case SlimeAnimator.AnimationType.DIE:
                return dieSprites;
        }
        return null;
    }
}