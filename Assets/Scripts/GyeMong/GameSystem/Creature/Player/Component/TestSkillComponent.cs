using System.Collections;
using System.Collections.Generic;
using GyeMong.GameSystem.Creature.Player.Controller;
using UnityEngine;

public interface TestSkillComponent
{
    public IEnumerator UseSkill();
}

public class DefaultSkillComponent : MonoBehaviour, TestSkillComponent
{
    public IEnumerator UseSkill()
    {
        // soundController.Trigger(PlayerSoundType.SWORD_SKILL);
        // isAttacking = true;
        // canMove = false;
        // animator.SetBool("isAttacking", true);
        //
        // curSkillGauge -= stat.SkillCost;
        // changeListenerCaller.CallSkillGaugeChangeListeners(curSkillGauge);
        // SpawnAttackCollider(attackColliderPrefab);
        // SpawnSkillCollider();
        // movement = Vector2.zero;
        // StopPlayer();
        //
        // canMove = true;
        //
        yield return new WaitForSeconds(1);
        //
        // animator.SetBool("isAttacking", false);
        //     
        // isAttacking = false;
    }
}
