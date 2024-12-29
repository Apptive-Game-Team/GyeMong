using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Puzzle2RewardClaim : MonoBehaviour, IEventTriggerable
{
    [SerializeField] private VisualEffect vfxRenderer;
    
    private const float DELTA_TIME = 0.02f;
    private IEnumerator ClearFog()
    {
        float timer = 0;
        float clearTime = 5f;
        float currentRadius = vfxRenderer.GetFloat("Radius");
        float targetRadius = 10;
        float deltaRadius = (targetRadius - currentRadius) * (DELTA_TIME/clearTime);
        while (timer < clearTime)
        {
            vfxRenderer.SetFloat("Radius", currentRadius += deltaRadius);
            yield return new WaitForSeconds(DELTA_TIME);
            timer += DELTA_TIME;
        }
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        StartCoroutine(ClearFog());
    }

    public void Trigger()
    {
        RuneObjectCreator.Instance.DrawRuneObject(3, transform.position);
        StartCoroutine(ClearFog());
    }
}
