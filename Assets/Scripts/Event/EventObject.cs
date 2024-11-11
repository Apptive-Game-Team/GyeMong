using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventObject : InteractableObject
{
    private enum EventTrigger
    {
        OnCollisionEnter = 0,
        OnInteraction = 1,
        OnAwake = 2,
    }

    [SerializeField]
    private bool isLoop = false;

    [SerializeField]
    private EventTrigger trigger;

    [SerializeField]
    private int triggerLimitCounter = -1;

    [SerializeField]
    [SerializeReference]
    public List<Event> eventSequence = new List<Event>();

    private Coroutine eventLoop = null;

    private void Start()
    {
        if (trigger == EventTrigger.OnAwake && triggerLimitCounter != 0)
        {
            TriggerEvent();
            triggerLimitCounter -= 1;
        }
    }

    private IEnumerator EventLoop()
    {
        do
        {
            foreach (Event eventObject in eventSequence)
            {
                yield return eventObject.execute();
            }
        } while (isLoop);
    }

    public void TriggerEvent()
    {
        if (eventLoop != null)
        {
            KillEvent();
        }
        eventLoop = StartCoroutine(EventLoop());
    }

    public void KillEvent()
    {
        if (eventLoop != null)
            StopCoroutine(eventLoop);
    }

    protected override void OnInteraction(Collider2D collision)
    {
        if (trigger == EventTrigger.OnInteraction && triggerLimitCounter != 0)
        {
            TriggerEvent();
            triggerLimitCounter -= 1;
        }
            
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (trigger == EventTrigger.OnCollisionEnter && triggerLimitCounter != 0)
        {
            TriggerEvent();
            triggerLimitCounter -= 1;
        }
    }
}
