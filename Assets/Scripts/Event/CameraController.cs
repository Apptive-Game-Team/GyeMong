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
    private bool isFollowing = true;
    
    public bool IsFollowing
    {
        get => isFollowing && !isShaking;
        set => isFollowing = value;
    }
    
    void Awake()
    {
        EffectManager.Instance.SetCameraController(this);
        player = GameObject.Find("Player");
    }

    
    void Update()
    {
        if (IsFollowing)
            transform.position = player.transform.position + Vector3.back * defaultCameraZ;
    }

    public IEnumerator ShakeCamera(float time)
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
}
