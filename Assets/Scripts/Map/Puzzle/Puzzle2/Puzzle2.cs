using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using playerCharacter;
using UnityEngine.VFX;

public class Puzzle2 : MonoBehaviour, IEventTriggerable
{
    private Collider2D[] paths;
    private Transform[] roots;
    PlayerCharacter player;
    Collider2D playerCollider;
    List<Collider2D> pathList;
    private EventObject eventObject;
    private EventObject pathEventObject;
    private EventStatus<int> rootNum;
    
    [SerializeField] private VisualEffect vfxRenderer;
    
    int curRootNum = 0;

    private void Start()
    {
        rootNum = GetComponent<IntEventStatus>();
        eventObject = GetComponent<EventObject>();
        player = PlayerCharacter.Instance;
        playerCollider = player.GetComponentInChildren<Collider2D>();
        GameObject pRoots = transform.Find("Roots").gameObject;
        roots = transform.Find("Roots").GetComponentsInChildren<Transform>()
            .Where(component => component.gameObject != pRoots && component.transform.parent == pRoots.transform)
            .ToArray();;
        Array.Sort(roots, (a, b) => a.GetSiblingIndex().CompareTo(b.GetSiblingIndex()));
        Transform pathTransform = transform.Find("Path");
        paths = pathTransform.GetComponentsInChildren<Collider2D>();
        pathEventObject = pathTransform.GetComponent<EventObject>();
    }

    private void Update()
    {
        vfxRenderer.SetVector3("ColliderPos", PlayerCharacter.Instance.transform.position);
        UpdateParticle();
        if (!CheckOnPath())
        {
            eventObject.Trigger();
        }
    }
    
    void UpdateParticle()
    {
        Event[] events =  pathEventObject.EventSequence;
        foreach (Event @event in events)
        {
            if (@event is ParticleAToBEvent)
            {
                ParticleAToBEvent particleEvent = @event as ParticleAToBEvent;
                int index = rootNum.GetStatus();
                if (index >= roots.Length - 1)
                    return;
                particleEvent._startPosition = roots[index].position;
                particleEvent._endPosition = roots[index+1].position;
            }
        }
    }
    
    bool CheckOnPath()
    {
        bool isOn = false;
        foreach (Collider2D collider in paths)
        {
            isOn |= collider.IsTouching(playerCollider);
        }
        return isOn;
    }
    
    void UpdateResetTime()
    {
        
    }

    private void MovePlayerPosition()
    {
        player.transform.position = roots[rootNum.GetStatus()].position;
    }
    public void Trigger()
    {
        MovePlayerPosition();
    }
}