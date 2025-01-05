using System.Collections;
using System.Collections.Generic;
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

    public abstract class GolemState : BaseState
    {
        public Golem Golem => creature as Golem;
    }
    
    public class MeleeAttack : GolemState
    { 
        public override int GetWeight()
        {
            return (creature.DistanceToPlayer < creature.MeleeAttackRange) ? 5 : 0;
        }

        public override IEnumerator StateCoroutine()
        {
            Golem.Animator.SetBool("TwoHand", true);
            
            yield return new WaitForSeconds(2f);
            yield return Golem.MakeShockwave(4);
            
            Golem.Animator.SetBool("TwoHand", false);
            creature.ChangeState();
        }
    }

    public class FallingCubeAttack : GolemState
    {
        public override int GetWeight()
        {
            return 5;
        }

        public override IEnumerator StateCoroutine()
        {
            Golem.Animator.SetBool("Toss", true);
            
            creature.StartCoroutine(Golem.TossSoundObject.Play());
            yield return new WaitForSeconds(2f);
            GameObject cube= Instantiate(Golem.cubePrefab, PlayerCharacter.Instance.transform.position + new Vector3(0, 4, 0), Quaternion.identity);
            Cube cubeComponent = cube.GetComponent<Cube>();
            cubeComponent.SetDamage(creature.damage);
            GameObject shadow = Instantiate(Golem.cubeShadowPrefab, PlayerCharacter.Instance.transform.position - new Vector3(0, 0.6f, 0), Quaternion.identity);
            cubeComponent.DetectShadow(shadow);

            Golem.Animator.SetBool("Toss", false);
            
            yield return new WaitUntil(()=>cube.IsDestroyed());
            
            creature.ChangeState();
        }
    }   

    public class ChargeShield : GolemState
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

    public class UpStoneAttack : GolemState
    {
        public override int GetWeight()
        {
            return (Golem.CurrentPhase == 0) ? 5 : 0;
        }

        public override IEnumerator StateCoroutine()
        {
            Golem.Animator.SetBool("OneHand", true);
            yield return new WaitForSeconds(1f);

             int numberOfObjects = 5;
             float interval = 0.2f;
             float fixedDistance = 7f;

             List<GameObject> spawnedObjects = new List<GameObject>();

             Vector3 direction = Golem.DirectionToPlayer;
             Vector3 startPosition = Golem.transform.position;

             Golem.StartCoroutine(SpawnFloor(startPosition, direction, fixedDistance, numberOfObjects, interval));

             Golem.StartCoroutine(DestroyFloor(spawnedObjects, 0.5f));
             Golem.Animator.SetBool("OneHand", false);
             
             creature.ChangeState();
        }
        
        private IEnumerator SpawnFloor(Vector3 startPosition, Vector3 direction, float fixedDistance, int numberOfObjects, float interval)
        {
            List<GameObject> spawnedObjects = new List<GameObject>();
            for (int i = 0; i <= numberOfObjects; i++)
            {
                Vector3 spawnPosition = startPosition + direction * (fixedDistance * ((float)i / numberOfObjects));
                GameObject floor = Instantiate(Golem.floorPrefab, spawnPosition, Quaternion.identity);
                spawnedObjects.Add(floor);
                Golem._shockwavesoundObject.SetSoundSourceByName("ENEMY_Shockwave");
                Golem.StartCoroutine(Golem._shockwavesoundObject.Play());
                yield return new WaitForSeconds(interval); // ���� ������Ʈ �������� ���
            }
            yield return spawnedObjects;
        }
        
        private IEnumerator DestroyFloor(List<GameObject> objects, float delay)
        {
            yield return new WaitForSeconds(delay); 
            foreach (GameObject obj in objects)
            {
                if (obj != null)
                { 
                    Destroy(obj);
                } 
            }
        }
    } 
    
    public class ShockwaveAttack : GolemState
    {
        public override int GetWeight()
        {
            return (Golem.CurrentPhase == 1) ? 5 : 0;
        }

        public override IEnumerator StateCoroutine()
        {
            Golem.Animator.SetBool("TwoHand", true);
            
            yield return new WaitForSeconds(2f);
            yield return Golem.MakeShockwave(14);
            
            Golem.Animator.SetBool("TwoHand", false);
            creature.ChangeState();
        }
    }
}
