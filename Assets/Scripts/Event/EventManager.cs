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
        Color color = hurtEffect.color;
        color.a = amount;
        hurtEffect.color = color;
    }

    /// <summary>
    /// vibrate camera
    /// </summary>
    public void ShakeCamera()
    {
        cameraController.ShakeCamera();
    }

    private void CachingImages()
    {
        hurtEffect = transform.Find("HurtEffect").GetComponent<RawImage>();
    }

    protected override void Awake()
    {
        base.Awake();
        CachingImages();
    }
}
