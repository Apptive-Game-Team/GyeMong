using System.Collections;
using playerCharacter;
using TMPro;
using UnityEngine;

public class Elf : Boss
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

    public abstract class ElfState : BaseState
    {
        public Elf Elf => creature as Elf;
    }

    public class MoveState : ElfState
    {
        public override int GetWeight()
        {
            return (Elf.DistanceToPlayer > Elf.MeleeAttackRange) ? 5 : 0;
        }

        public override IEnumerator StateCoroutine()
        {
            Elf.Animator.SetBool("isMove", true);
            Elf.Animator.SetFloat("moveType", 0);
            float duration = 2f;
            float timer = 0f;

            while (duration > timer && Elf.DistanceToPlayer > Elf.MeleeAttackRange)
            {
                timer += Time.deltaTime;
                yield return null;
                Elf.TrackPlayer();
                Elf.Animator.SetFloat("xDir", Elf.DirectionToPlayer.x);
                Elf.Animator.SetFloat("yDir", Elf.DirectionToPlayer.y);
            }

            Elf.Animator.SetBool("iMove", false);
            Elf.ChangeState();
        }
    }

    public new class BackStep : ElfState
    {
        public override int GetWeight()
        {
            return (Elf.DistanceToPlayer < Elf.RangedAttackRange / 2) ? 5 : 0;
        }

        public override IEnumerator StateCoroutine()
        {
            Elf.Animator.SetBool("isMove", true);
            Elf.Animator.SetFloat("moveType", 1);
            Elf.Animator.SetFloat("xDir", Elf.DirectionToPlayer.x);
            Elf.Animator.SetFloat("yDir", Elf.DirectionToPlayer.y);
            yield return Elf.BackStep(Elf.RangedAttackRange);

            Elf.Animator.SetBool("isMove", false);
            Elf.ChangeState();
        }
    }

    public class RangedAttack : ElfState
    {
        public override int GetWeight()
        {
            return (Elf.DistanceToPlayer > Elf.RangedAttackRange / 2) ? 5 : 0;
        }

        public override IEnumerator StateCoroutine()
        {
            Elf.Animator.SetBool("attackDelay", true);
            Elf.Animator.SetFloat("attackType", 0);
            yield return new WaitForSeconds(0.5f);
            Elf.Animator.SetBool("attackDelay", false);
            GameObject arrow = Instantiate(Elf.arrowPrefab, Elf.transform.position, Quaternion.identity);
            Elf.RotateArrowTowardsPlayer(arrow);
            yield return Elf.arrowSoundObject.Play();
            yield return new WaitForSeconds(1f);
            Elf.ChangeState();
        }
    }
    public class SeedRangedAttak : ElfState
    {
        public override int GetWeight()
        {
            if (Elf.CurrentPhase == 1)
            {
                return (Elf.DistanceToPlayer > Elf.RangedAttackRange / 2) ? 5 : 0;
            }
            return 0;
        }

        public override IEnumerator StateCoroutine()
        {
            Elf.Animator.SetBool("attackDelay", true);
            Elf.Animator.SetFloat("attackType", 1);
            yield return new WaitForSeconds(1f);
            Elf.Animator.SetBool("attackDelay", false);
            int count = 0;
            while (count < 4)
            {
                GameObject seed = Instantiate(Elf.seedPrefab, Elf.transform.position, Quaternion.identity);
                Elf.RotateArrowTowardsPlayer(seed);
                yield return Elf.arrowSoundObject.Play();
                count++;
            }
            Elf.ChangeState();
        }
    }
    public class MeleeAttack : ElfState
    {
        public override int GetWeight()
        {
            return (Elf.DistanceToPlayer < Elf.MeleeAttackRange) ? 5 : 0;
        }
        public override IEnumerator StateCoroutine()
        {
            Elf.Animator.SetBool("attackDelay", true);
            Elf.Animator.SetFloat("attackType", 2);
            yield return new WaitForSeconds(0.2f);
            Elf.Animator.SetBool("attackDelay", false);
            Elf.meleeAttackPrefab.SetActive(true);
            Vector3 direction = Elf.DirectionToPlayer;
            Elf.meleeAttackPrefab.transform.position = Elf.transform.position + Elf.DirectionToPlayer * Elf.MeleeAttackRange;
            yield return new WaitForSeconds(0.3f);
            Elf.meleeAttackPrefab.SetActive(false);
            Elf.ChangeState();
        }
    }
    public class WhipAttack : ElfState
    {
        public override int GetWeight()
        {
            return (Elf.DistanceToPlayer < Elf.MeleeAttackRange) ? 5 : 0;
        }
        public override IEnumerator StateCoroutine()
        {
            Elf.Animator.SetBool("attackDelay", true);
            Elf.Animator.SetFloat("attackType", 3);
            yield return new WaitForSeconds(0.2f);
            Elf.Animator.SetBool("attackDelay", false);
            Instantiate(Elf.vinePrefab, Elf.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(2f);
            Elf.ChangeState();
        }
    }
}


