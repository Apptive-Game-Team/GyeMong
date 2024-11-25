using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MazeDarkness : MonoBehaviour
{
    private Light2D globalLight;
    private GameObject playerLight;
    private GameObject player;
    private bool isInMaze = false;
    private const float changeRatio = 0.01f;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        globalLight = GameObject.Find("GlobalLight2D").GetComponent<Light2D>();
        playerLight = player.transform.Find("PlayerLight").GetComponent<Light2D>().gameObject;
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
        {
            bool previousState = isInMaze;
            isInMaze = player.transform.position.y > transform.position.y;

            if (previousState != isInMaze)
            {
                StopAllCoroutines();
                StartCoroutine(ChangeIntensity(isInMaze));
            }
        }
    }

    private IEnumerator ChangeIntensity(bool isInMaze)
    {
        float targetIntensity = isInMaze ? 0f : 1f;
        float Ratio = 0f;

        playerLight.SetActive(isInMaze);
        while (Mathf.Abs(globalLight.intensity - targetIntensity) > 0.01f)
        {
            Ratio += Time.deltaTime;
            globalLight.intensity = Mathf.Lerp(globalLight.intensity, targetIntensity, Ratio);
            yield return new WaitForSeconds(0.05f);
        }
        globalLight.intensity = targetIntensity;

        yield return null;
    }
}
