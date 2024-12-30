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
        if (ConditionManager.Instance.Conditions.ContainsKey("spring_puzzle2_rune_drop"))
        {
            if (!ConditionManager.Instance.Conditions["spring_puzzle2_rune_drop"])
            {
                RuneObjectCreator.Instance.DrawRuneObject(2, transform.position);
            }
        }
        else
        {
            ConditionManager.Instance.Conditions.Add("spring_puzzle2_rune_drop", true);
            RuneObjectCreator.Instance.DrawRuneObject(2, transform.position);
        }
        
    }

    public void Trigger()
    {
        StartCoroutine(ClearFog());
    }
}
