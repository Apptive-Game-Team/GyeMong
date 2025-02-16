using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeShadow : MonoBehaviour
{
    private GameObject player;

    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(FollowAndFall());
    }

    private IEnumerator FollowAndFall()
    {
        float followDuration = 1f;
        float elapsedTime = 0f;
        while (elapsedTime < followDuration)
        {
            if (player != null)
            {
                transform.position = player.transform.position - new Vector3(0, 0.6f, 0);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Cube>() != null)
        {
            Destroy(gameObject);
        }
    }
}
