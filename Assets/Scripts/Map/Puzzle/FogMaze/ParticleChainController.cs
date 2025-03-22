using System;
using System.Event;
using System.Event.Event.CinematicEvent;
using System.Event.Event.Status;
using System.Linq;
using UnityEngine;
using Event = System.Event.Event.Event;

namespace Map.Puzzle.FogMaze
{
    public class ParticleChainController : MonoBehaviour
    {
        private EventObject _pathEventObject;
        private Transform[] _roots;
        private EventStatus<int> _rootNum;
        private void Start()
        {
            _rootNum = GetComponent<IntEventStatus>();
            GameObject pRoots = transform.Find("Roots").gameObject;
            _roots = transform.Find("Roots").GetComponentsInChildren<Transform>()
                .Where(component => component.gameObject != pRoots && component.transform.parent == pRoots.transform)
                .ToArray();;
            Array.Sort(_roots, (a, b) => a.GetSiblingIndex().CompareTo(b.GetSiblingIndex()));
            Transform pathTransform = transform.Find("Path");
            _pathEventObject = pathTransform.GetComponent<EventObject>();
        }

        private void Update()
        {
            UpdateParticle();
        }
        
        private void UpdateParticle()
        {
            Event[] events = _pathEventObject.EventSequence;
            foreach (Event @event in events)
            {
                if (@event is ParticleAToBEvent)
                {
                    ParticleAToBEvent particleEvent = @event as ParticleAToBEvent;
                    int index = _rootNum.GetStatus();
                    if (index >= _roots.Length - 1)
                        return;
                    particleEvent._startPosition = _roots[index].position;
                    particleEvent._endPosition = _roots[index+1].position;
                }
            }
        }
    }
}