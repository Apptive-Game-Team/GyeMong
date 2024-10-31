using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : SingletonObject<EventManager>
{

    private CameraController cameraController;

    public void SetCameraController(CameraController cameraController)
    {
        this.cameraController = cameraController;
    }

    
    public void ShakeCamera()
    {
        cameraController.ShakeCamera();
    }
}
