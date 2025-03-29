using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using Visual.Camera;

public abstract class CameraEvent : Event { }

public class CameraMove : CameraEvent
{
    [SerializeField] private Vector3 destination;
    [SerializeField] private float speed;
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        return CameraManager.Instance.CameraMove(destination, speed);
    }
}

public class CameraFollow : CameraEvent
{
    [SerializeField] private GameObject followObject;
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        CameraManager.Instance.CameraFollow(followObject.transform);
        return null;
    }
}

public class CameraZoomInOut : CameraEvent
{
    [SerializeField] private float size;
    [SerializeField] private float duration;
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        return CameraManager.Instance.CameraZoomInOut(size, duration);
    }
}

public class CameraZoomReset : CameraEvent
{
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        CameraManager.Instance.CameraZoomReset();
        return null;
    }
}

public class CameraShake : CameraEvent
{
    [SerializeField] private float force;
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        CameraManager.Instance.CameraShake(force);
        return null;
    }
}