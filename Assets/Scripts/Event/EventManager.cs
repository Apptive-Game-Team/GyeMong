using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Every Event Management class
/// </summary>
public class EventManager : SingletonObject<EventManager>
{

    private CameraController cameraController;
    private RawImage hurtEffect;
    private RawImage black;
    private const float FADING_DELTA_TIME = 0.05f;

    public void SetCameraController(CameraController cameraController)
    {
        this.cameraController = cameraController;
    }

    /// <summary>
    /// set HurtEffect transparency
    /// </summary>
    /// <param name="amount">0(none)~1(fill) alpha value</param>
    public void HurtEffect(float amount)
    {
        StartCoroutine(Fade(hurtEffect, amount));
    }

    /// <summary>
    /// vibrate camera
    /// </summary>
    public void ShakeCamera()
    {
        cameraController.ShakeCamera();
    }

    /// <summary>
    /// The screen is getting Ligther
    /// </summary>
    public void FadeIn()
    {
        StartCoroutine(Fade(black, 0));
    }

    /// <summary>
    /// The screen is getting darker
    /// </summary>
    public void FadeOut()
    {
        StartCoroutine(Fade(black, 1));
    }


    private IEnumerator Fade(RawImage image, float targetAlpha, float duration = 0.5f)
    {
        Color color = image.color;
        float startAlpha = color.a;
        float deltaAlpha = targetAlpha - startAlpha;
        int targetLoop = (int)(duration / FADING_DELTA_TIME);
        for (int i=0; i<targetLoop; i++)
        {
            color.a = (startAlpha + i * deltaAlpha / targetLoop);
            image.color = color;
            yield return new WaitForSeconds(FADING_DELTA_TIME);
        }
        color.a = targetAlpha;
        image.color = color;
    }

    private void CachingImages()
    {
        hurtEffect = transform.Find("HurtEffect").GetComponent<RawImage>();
        black = transform.Find("Black").GetComponent<RawImage>();
    }

    protected override void Awake()
    {
        base.Awake();
        CachingImages();
    }
}
