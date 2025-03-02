using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventScene : Event { }

public class SlimeEvents : EventScene
{
    [SerializeField] private GameObject targetSlime;
    [SerializeField] private GameObject[] slimes;
    [SerializeField] private float moveSpeed = 3f;

    public override IEnumerator Execute(EventObject eventObject = null)
    {
        Debug.Log("Start");
        for (int i = 0;i < slimes.Length;i++)
        {
            GameObject randomSlime = slimes[i];
            Vector2 randomDirection = Random.insideUnitCircle.normalized * 4f;
            Vector3 randomPosition = targetSlime.transform.position + new Vector3(randomDirection.x, randomDirection.y, 0);
            randomSlime.transform.position = randomPosition;
            MoveSlimeToTarget(randomSlime);
            float randomTime = Random.Range(0.2f, 0.6f);
            yield return new WaitForSeconds(randomTime);
        }

        yield return new WaitForSeconds(1f);
    }

    private void MoveSlimeToTarget(GameObject slime)
    {
        Vector3 startPosition = slime.transform.position;
        Vector3 targetPosition = targetSlime.transform.position;
        float duration = 2f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            slime.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
        }
    }
}