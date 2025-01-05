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
        public Golem golem;
        public override void OnStateEnter()
        {
            golem = creature as Golem;
        }
    }
    
    protected class MeleeAttack : GolemState
    { 
        public override void OnStateEnter()
        {
            _animator.SetBool("TwoHand", true);
        }

        public override int GetWeight()
        {
            return (creature.DistanceToPlayer < creature.MeleeAttackRange) ? 5 : 0;
        }

        public override IEnumerator StateCoroutine()
        {
            yield return new WaitForSeconds(2f);
            yield return golem.MakeShockwave();
            creature.ChangeState();
        }
        
        public override void OnStateUpdate() { }

        public override void OnStateExit()
        {
            _animator.SetBool("TwoHand", false);
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
            creature.StartCoroutine(golem.TossSoundObject.Play());
            yield return new WaitForSeconds(2f);
            GameObject cube= Object.Instantiate(golem.cubePrefab, PlayerCharacter.Instance.transform.position + new Vector3(0, 4, 0), Quaternion.identity);
            Cube cubeComponent = cube.GetComponent<Cube>();
            cubeComponent.SetDamage(creature.damage);
            GameObject shadow =  Object.Instantiate(golem.cubeShadowPrefab, PlayerCharacter.Instance.transform.position - new Vector3(0, 0.6f, 0), Quaternion.identity);
            cubeComponent.DetectShadow(shadow);
            
        }
        
        public override void OnStateEnter()
        {
            _animator.SetBool("Toss", true);
        }

        public override void OnStateUpdate() {}

        public override void OnStateExit()
        {
            _animator.SetBool("Toss", false);
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
            creature.GetComponent<Renderer>().material = golem.materials[1];
            creature.GetComponent<Renderer>().material.SetFloat("_isShieldActive", 1f);
            creature.shieldComponenet.SetActive(true);
        }

        public override void OnStateEnter() {}

        public override void OnStateUpdate() {}

        public override void OnStateExit() {}
    }
}
