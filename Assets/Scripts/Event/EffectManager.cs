using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Every Event Management class
/// </summary>
public class EffectManager : SingletonObject<EffectManager>
{

    private CameraController cameraController;
    private RawImage hurtEffect;
    private RawImage black;
    private const float FADING_DELTA_TIME = 0.05f;

    public ChatController GetChatController()
    {
        return GetComponent<ChatController>();
    }

    public void SetCameraController(CameraController cameraController)
    {
        this.cameraController = cameraController;
    }

    /// <summary>
    /// set HurtEffect transparency
    /// </summary>
    /// <param name="amount">0(none)~1(fill) alpha value</param>
    public IEnumerator HurtEffect(float amount)
    {
        return Fade(hurtEffect, amount);
    }

    /// <summary>
    /// vibrate camera
    /// </summary>
    public IEnumerator ShakeCamera(float time=0.5f)
    {
        return cameraController.ShakeCamera(time);
    }

    /// <summary>
    /// The screen is getting Ligther
    /// </summary>
    /// <returns>return IEnumerator</returns>
    public IEnumerator FadeIn()
    {
        return Fade(black, 0);
    }

    /// <summary>
    /// The screen is getting darker
    /// </summary>
    /// /// <returns>return IEnumerator</returns>
    public IEnumerator FadeOut()
    {
        return Fade(black, 1);
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
