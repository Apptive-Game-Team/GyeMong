using System;
using System.Collections.Generic;
using System.Event;
using System.Event.Event.Status;
using System.Event.Interface;
using System.Linq;
using Creature.Player;
using UnityEngine;
using UnityEngine.VFX;

namespace Map.Puzzle.FogMaze
{
    // Check Player is on Path
    // and if Player is not on Path, then Trigger Event Object
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
    
        private int _curRootNum = 0;

        private void Start()
        {
            _rootNum = GetComponent<IntEventStatus>(); // for checking current root number
            _eventObject = GetComponent<EventObject>(); // for triggering event
            _player = PlayerCharacter.Instance; // for checking player position
            _playerCollider = _player.GetComponentInChildren<Collider2D>();
            
            InitializeRoots();
            InitializePaths();
        }

        private void InitializePaths()
        {
            Transform pathTransform = transform.Find("Path");
            _paths = pathTransform.GetComponentsInChildren<Collider2D>();
        }
        
        private void InitializeRoots()
        {
            GameObject pRoots = transform.Find("Roots").gameObject; // for getting roots
            _roots = transform.Find("Roots").GetComponentsInChildren<Transform>()
                .Where(component => component.gameObject != pRoots && component.transform.parent == pRoots.transform)
                .ToArray();
            Array.Sort(_roots, (a, b) => a.GetSiblingIndex().CompareTo(b.GetSiblingIndex()));
        }

        private void Update()
        {
            if (!CheckOnPath())
            {
                _eventObject.Trigger();
            }
        }
    
        bool CheckOnPath() // Check Player is on Path
        {
            bool isOn = false;
            foreach (Collider2D collider in _paths)
            {
                isOn |= collider.IsTouching(_playerCollider);
            }
            return isOn;
        }
    
        private void MovePlayerPosition() // Move Player to Root Position
        {
            _player.transform.position = _roots[_rootNum.GetStatus()].position;
        }
        
        public void Trigger()
        {
            MovePlayerPosition();
        }
    }
}