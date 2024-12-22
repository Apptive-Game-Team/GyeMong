using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using playerCharacter;

public class Puzzle2 : MonoBehaviour, IEventTriggerable
{
    private Collider2D[] paths;
    private Transform[] roots;
    Tilemap fogTilemap;
    PlayerCharacter player;
    Collider2D playerCollider;
    List<Collider2D> pathList;
    private EventObject eventObject;
    private EventObject pathEventObject;
    private EventStatus<int> rootNum;
    
    const int visionRange = 2;
    int curRootNum = 0;

    private void Start()
    {
        rootNum = GetComponent<IntEventStatus>();
        eventObject = GetComponent<EventObject>();
        player = PlayerCharacter.Instance;
        playerCollider = player.GetComponent<Collider2D>();
        GameObject pRoots = transform.Find("Roots").gameObject;
        roots = transform.Find("Roots").GetComponentsInChildren<Transform>()
            .Where(component => component.gameObject != pRoots && component.transform.parent == pRoots.transform)
            .ToArray();;
        Array.Sort(roots, (a, b) => a.GetSiblingIndex().CompareTo(b.GetSiblingIndex()));
        Transform pathTransform = transform.Find("Path");
        paths = pathTransform.GetComponentsInChildren<Collider2D>();
        pathEventObject = pathTransform.GetComponent<EventObject>();
        fogTilemap = GameObject.Find("fogTilemap").GetComponent<Tilemap>();
    }

    private void Update()
    {
        UpdateFog();
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

    void UpdateFog()
    {
        Vector3Int playerPos = fogTilemap.WorldToCell(player.transform.position);

        for(int i=-visionRange; i<= visionRange ;i++)
        {
            for(int j=-visionRange;j<=visionRange ;j++)
            {
                fogTilemap.SetTile(playerPos + new Vector3Int(i, j, 0), null);
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
        print(rootNum.GetStatus());
        player.transform.position = roots[rootNum.GetStatus()].position;
    }
    public void Trigger()
    {
        MovePlayerPosition();
    }
}