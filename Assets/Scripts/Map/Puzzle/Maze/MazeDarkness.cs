using System.Collections;
using playerCharacter;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Map.Puzzle.Maze
{
    public class MazeDarkness : MonoBehaviour, IEventTriggerable
    {
        private Light2D _globalLight;
        private GameObject _playerLight;
        private GameObject _player;
        private bool _isInMaze = false;

        private void Start()
        {
            _player = PlayerCharacter.Instance.gameObject;
            _globalLight = GameObject.Find("GlobalLight2D").GetComponent<Light2D>();
            _playerLight = _player.transform.Find("PlayerLight").GetComponent<Light2D>().gameObject;
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
                        StartCoroutine(ChangeIntensity(_isInMaze));
                    }
                }
            }
        }

        public void Trigger()
        {
            StartCoroutine(ChangeIntensity(false));
            _isInMaze = false;
        }

        private IEnumerator ChangeIntensity(bool isInMaze)
        {
            float targetIntensity = isInMaze ? 0f : 1f;
            float ratio = 0f;

            _playerLight.SetActive(isInMaze);
            while (Mathf.Abs(_globalLight.intensity - targetIntensity) > 0.01f)
            {
                ratio += Time.deltaTime;
                _globalLight.intensity = Mathf.Lerp(_globalLight.intensity, targetIntensity, ratio);
                yield return new WaitForSeconds(0.05f);
            }
            _globalLight.intensity = targetIntensity;

            yield return null;
        }
    }
}
