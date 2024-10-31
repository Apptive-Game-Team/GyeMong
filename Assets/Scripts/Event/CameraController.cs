using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject player;
    private int defaultCameraZ = 10;

    private const float SHAKE_AMOUNT = 0.1f;
    private const float SHAKE_DELAY = 0.1f;
    private bool isShaking = false;

    void Awake()
    {
        EventManager.Instance.SetCameraController(this);
        player = GameObject.Find("Player");
    }

    
    void Update()
    {
        if (!isShaking)
            transform.position = player.transform.position + Vector3.back * defaultCameraZ;
    }

    private IEnumerator Shaking(float time)
    {
        float timer = 0;
        isShaking = true;
        while (timer < time)
        {
            yield return new WaitForSeconds(SHAKE_DELAY);
            timer += 0.1f;
            transform.position = player.transform.position + Random.insideUnitSphere * SHAKE_AMOUNT + Vector3.back * defaultCameraZ;
        }
        isShaking = false;
    }

    public void ShakeCamera(float time = 1)
    {
        StartCoroutine(Shaking(time));
    }
}
