using System.Collections;
using playerCharacter;
using Unity.VisualScripting;
using UnityEngine;



public class Golem : Boss
{
    [SerializeField] public GameObject cubePrefab;
    [SerializeField] public GameObject cubeShadowPrefab;
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private GameObject meleeAttackPrefab1;
    [SerializeField] private GameObject meleeAttackPrefab2;
    [SerializeField] private GameObject shockwavePrefab;
    private Shield shieldComponenet;
    [SerializeField] private static Animator _animator;

    [SerializeField] private SoundObject _shockwavesoundObject;
    public SoundObject ShockwaveSoundObject => _shockwavesoundObject;
    [SerializeField] private SoundObject _tossSoundObject;
    public SoundObject TossSoundObject => _tossSoundObject;
    
    
    private void Initialize()
    {
        maxPhase = 2;
        maxHps.Add(200f);
        maxHps.Add(300f);
        currentShield = 0f;
    }
    
    public override void TakeDamage(float damage)
    {
        throw new System.NotImplementedException();
    }
    
    private Vector3[] GetCirclePoints(Vector3 center, float radius, int numberOfPoints)
    {
        Vector3[] points = new Vector3[numberOfPoints];
        for (int i = 0; i < numberOfPoints; i++)
        {
            float angle = i * Mathf.PI * 2 / numberOfPoints;
            float x = center.x + Mathf.Cos(angle) * radius;
            float y = center.y + Mathf.Sin(angle) * radius;
            points[i] = new Vector3(x, y, 0);
        }
        return points;
    }

    public IEnumerator MakeShockwave(int targetRadius = 14)
    {
        int startRadius = 4;
        for (int i = startRadius; i <= targetRadius; i++)
        {
            Vector3[] points = GetCirclePoints(transform.position, i, i * 3 + 10);
            ShockwaveSoundObject.SetSoundSourceByName("ENEMY_Shockwave");
            StartCoroutine(ShockwaveSoundObject.Play());
            for (int j = 0; j < points.Length; j++)
            {
                Instantiate(shockwavePrefab, points[j], Quaternion.identity);
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    protected abstract class GolemState : BaseState
    {
        public Golem Golem => creature as Golem;
    }
    
    protected class MeleeAttack : GolemState
    { 
        public override int GetWeight()
        {
            return (creature.DistanceToPlayer < creature.MeleeAttackRange) ? 5 : 0;
        }

        public override IEnumerator StateCoroutine()
        {
            _animator.SetBool("TwoHand", true);
            
            yield return new WaitForSeconds(2f);
            yield return Golem.MakeShockwave();
            
            _animator.SetBool("TwoHand", false);
            creature.ChangeState();
        }
    }

    protected class FallingCubeAttack : GolemState
    {
        public override int GetWeight()
        {
            return 5;
        }

        public override IEnumerator StateCoroutine()
        {
            _animator.SetBool("Toss", true);
            
            creature.StartCoroutine(Golem.TossSoundObject.Play());
            yield return new WaitForSeconds(2f);
            GameObject cube= Instantiate(Golem.cubePrefab, PlayerCharacter.Instance.transform.position + new Vector3(0, 4, 0), Quaternion.identity);
            Cube cubeComponent = cube.GetComponent<Cube>();
            cubeComponent.SetDamage(creature.damage);
            GameObject shadow = Instantiate(Golem.cubeShadowPrefab, PlayerCharacter.Instance.transform.position - new Vector3(0, 0.6f, 0), Quaternion.identity);
            cubeComponent.DetectShadow(shadow);

            _animator.SetBool("Toss", false);
            
            yield return new WaitUntil(()=>cube.IsDestroyed());
            
            creature.ChangeState();
        }
    }   

    protected class ChargeShield : GolemState
    {
        public override int GetWeight()
        {
            return 5;
        }

        public override IEnumerator StateCoroutine()
        {
            yield return new WaitForSeconds(2f);
            currentShield = 30f;
            Golem.MaterialController.SetMaterial(MaterialController.MaterialType.SHIELD);
            Golem.MaterialController.SetFloat(1);
            
            creature.ChangeState();
        }
    }

    protected class UpStoneAttack : GolemState
    {
        public override int GetWeight()
        {
            return (Golem.CurrentPhase == 0) ? 5 : 0;
        }

        public override IEnumerator StateCoroutine()
        {
            
        }
    } 
}
