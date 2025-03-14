using Creature.Boss.Component;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LineIndicator : IndicatorBase
{
    [SerializeField] private GameObject linePrefab;
    public override void Initialize(Vector3 startPosition, Vector3 direction, float range, float duration)
    {
        indicator = Instantiate(linePrefab, startPosition, Quaternion.LookRotation(Vector3.forward, direction)).transform;
    }
    public override IEnumerator GrowIndicator(float range, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float scaleX = Mathf.Lerp(0, range, elapsedTime / duration);
            indicator.localScale = new Vector3(scaleX, 1, 1);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);
        Destroy(indicator.gameObject);
    }
}