using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventObject : InteractableObject, IAttackable, IEventTriggerable
{
    private enum EventTrigger
    {
        OnCollisionEnter = 0,
        OnInteraction = 1,
        OnAwake = 2,
        OnAttacked = 3,
        OnCalled = 4,
    }

    [SerializeField]
    private bool isLoop = false;

    [SerializeField]
    private EventTrigger trigger;

    [SerializeField]
    private int triggerLimitCounter = -1;

    [SerializeReference]
    private List<Event> eventSequence = new List<Event>();

    private Coroutine eventLoop = null;

    public Event[] EventSequence => eventSequence.ToArray(); 
    
    public void SetTriggerLimitCounter(int counter)
    {
        triggerLimitCounter = counter;
    }
    private List<ToggeableCondition> FindToggleableConditions()
    {
        List<ToggeableCondition> result = new List<ToggeableCondition>();
        foreach (Event @event in eventSequence)
        {
            if (@event == null)
            {
                continue;
            }
            List<ToggeableCondition> temp = @event.FindToggleableConditions();
            if (temp != null)
            {
                result.AddRange(temp);
            }
        }
        return result;
    }

    private void InitializeToggleableConditions()
    {
        List<ToggeableCondition> conditions = FindToggleableConditions();
        foreach (ToggeableCondition condition in conditions)
        {
            condition.Check();
        }
    }

    private void Start()
    {
        InitializeToggleableConditions();
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
                yield return eventObject.Execute(this);
            }
        } while (isLoop);
        eventLoop = null;
    }

    public void Trigger()
    {
        if (triggerLimitCounter != 0)
        {
            TriggerEvent();
            triggerLimitCounter -= 1;
        }
    }
    
    private void TriggerEvent()
    {
        if (eventLoop != null)
        {
            return;
        }
        eventLoop = StartCoroutine(EventLoop());
    }

    public void KillEvent()
    {
        if (eventLoop != null)
            StopCoroutine(eventLoop);
        eventLoop = null;
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
        if (trigger == EventTrigger.OnCollisionEnter && collision.CompareTag("Player") && triggerLimitCounter != 0)
        {
            TriggerEvent();
            triggerLimitCounter -= 1;
        }
    }

    public void OnAttacked(float damage = 0)
    {
        if (trigger == EventTrigger.OnAttacked && triggerLimitCounter != 0)
        {
            TriggerEvent();
            triggerLimitCounter -= 1;
        }
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
