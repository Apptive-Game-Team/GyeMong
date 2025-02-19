using System;
using System.Collections;
using playerCharacter;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Map.Puzzle.Maze
{
    public class DarknessController : MonoBehaviour
    {
        private const float INTENSITY_CHANGE_SPEED = 0.05f;
        private const float INTENSITY_CHANGE_THRESHOLD = 0.01f;
        
        private Light2D _globalLight;
        private GameObject _playerLight;
        private GameObject _player;

        private void Start()
        {
            _player = PlayerCharacter.Instance.gameObject;
            _globalLight = GameObject.Find("GlobalLight2D").GetComponent<Light2D>();
            _playerLight = _player.transform.Find("PlayerLight").GetComponent<Light2D>().gameObject;
        }

        public IEnumerator ChangeIntensity(bool isInMaze)
        {
            float targetIntensity = isInMaze ? 0f : 1f;
            float ratio = 0f;

            _playerLight.SetActive(isInMaze);
            while (Mathf.Abs(_globalLight.intensity - targetIntensity) > INTENSITY_CHANGE_THRESHOLD)
            {
                ratio += Time.deltaTime;
                _globalLight.intensity = Mathf.Lerp(_globalLight.intensity, targetIntensity, ratio);
                yield return new WaitForSeconds(INTENSITY_CHANGE_SPEED);
            }
            _globalLight.intensity = targetIntensity;

            yield return null;
        }
    }
}