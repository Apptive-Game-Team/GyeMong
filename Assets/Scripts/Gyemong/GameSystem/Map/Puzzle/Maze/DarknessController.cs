using System.Collections;
using Gyemong.GameSystem.Creature.Player;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Gyemong.GameSystem.Map.Puzzle.Maze
{
    public class DarknessController : MonoBehaviour
    {
        private const float INTENSITY_CHANGE_DEFAULT_DURATION = 1;
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
        
        public void SetPlayerLight(bool isInMaze)
        {
            _playerLight.SetActive(isInMaze);
        }

        public IEnumerator ChangeIntensity(float intensity, float duration = INTENSITY_CHANGE_DEFAULT_DURATION)
        {
            float targetIntensity = intensity;
            float startIntensity = _globalLight.intensity;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float ratio = elapsedTime / duration;
                _globalLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, ratio);
                yield return null;
            }

            _globalLight.intensity = targetIntensity;
        }

    }
}