using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventStatus<T> : MonoBehaviour
{
    [SerializeField] private T status;

    public void SetStatus(T status)
    {
        this.status = status;
    }
    public T GetStatus()
    {
        return status;
    }
}
