using System.Collections;
using playerCharacter;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Map.Puzzle.Maze
{
    public class MazeDarkness : MonoBehaviour, IEventTriggerable
    {
        private GameObject _player;
        private bool _isInMaze = false;
        [SerializeField] private DarknessController _darknessController;
        
        private void Start()
        {
            _player = PlayerCharacter.Instance.gameObject;
        }

        private void OnTriggerExit2D(Collider2D other) 
        {
            if (!ConditionManager.Instance.Conditions.ContainsKey("spring_puzzle1_clear"))
            {
                if (other.CompareTag("Player"))
                {
                    bool previousState = _isInMaze;
                    _isInMaze = _player.transform.position.x < transform.position.x;

                    if (previousState != _isInMaze)
                    {
                        StopAllCoroutines();
                        StartCoroutine(_darknessController.ChangeIntensity(_isInMaze));
                    }
                }
            }
        }

        public void Trigger()
        {
            StartCoroutine(_darknessController.ChangeIntensity(false));
            _isInMaze = false;
        }
    }
}
