using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class CameraEvent : Event
{
    private static CameraController _cameraController;
    protected static CameraController CameraController
    {
        get
        {
            if (_cameraController == null)
            {
                _cameraController = EffectManager.Instance.GetCameraController();
            }
            return _cameraController;
        }
    }
}

[Serializable]
public class ShakeCameraEvent : CameraEvent
{
    public float time;
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        return EffectManager.Instance.ShakeCamera(time);
    }
}

public class StopFollowCameraEvent : CameraEvent
{
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        CameraController cameraController = CameraController;
        cameraController.IsFollowing = false;
        return null;
    }
}

public class StartFollowCameraEvent : CameraEvent
{
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        CameraController cameraController = CameraController;
        cameraController.IsFollowing = true;
        return null;
    }  
}

