using System;
using System.Collections.Generic;
using System.Linq;
using playerCharacter;
using UnityEngine;
using UnityEngine.VFX;

namespace Map.Puzzle.FogMaze
{
    public class OnPathChecker : MonoBehaviour, IEventTriggerable
    {
        private Collider2D[] _paths;
        private Transform[] _roots;
        private PlayerCharacter _player;
        private Collider2D _playerCollider;
        private List<Collider2D> _pathList;
        
        private EventObject _eventObject;
        private EventStatus<int> _rootNum;
    
        [SerializeField] private VisualEffect vfxRenderer;
    
        int _curRootNum = 0;

        private void Start()
        {
            _rootNum = GetComponent<IntEventStatus>();
            _eventObject = GetComponent<EventObject>();
            _player = PlayerCharacter.Instance;
            _playerCollider = _player.GetComponentInChildren<Collider2D>();
            GameObject pRoots = transform.Find("Roots").gameObject;
            _roots = transform.Find("Roots").GetComponentsInChildren<Transform>()
                .Where(component => component.gameObject != pRoots && component.transform.parent == pRoots.transform)
                .ToArray();;
            Array.Sort(_roots, (a, b) => a.GetSiblingIndex().CompareTo(b.GetSiblingIndex()));
            Transform pathTransform = transform.Find("Path");
            _paths = pathTransform.GetComponentsInChildren<Collider2D>();
        }

        private void Update()
        {
            vfxRenderer.SetVector3("ColliderPos", PlayerCharacter.Instance.transform.position);
            if (!CheckOnPath())
            {
                _eventObject.Trigger();
            }
        }
    
        bool CheckOnPath()
        {
            bool isOn = false;
            foreach (Collider2D collider in _paths)
            {
                isOn |= collider.IsTouching(_playerCollider);
            }
            return isOn;
        }
    
        private void MovePlayerPosition()
        {
            _player.transform.position = _roots[_rootNum.GetStatus()].position;
        }
        public void Trigger()
        {
            MovePlayerPosition();
        }
    }
}