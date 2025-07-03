using GyeMong.EventSystem.Event.Input;
using System.Collections;
using GyeMong.EventSystem.Event;
using UnityEngine;
using GyeMong.EventSystem.Event.EventScene;

namespace GyeMong.GameSystem.Map.MapEvent
{
    public class SpringGardenEvent : MonoBehaviour
    {
        [SerializeField] private GameObject targetSlime;
        [SerializeField] private GameObject[] slimes;

        [SerializeField] private Vector3 cameraDestination;
        [SerializeField] private float cameraSpeed;
        private float _delayTime = 1f;

        private bool _isTutorial;

        private void Start()
        {
            _isTutorial = PlayerPrefs.GetInt("Tutorial", 0) == 0;
            StartCoroutine(TriggerEvents());
        }

        public IEnumerator Trigger()
        {
            return TriggerEvents();
        }

        private IEnumerator TriggerEvents()
        {
            if (_isTutorial)
            {
                yield return StartCoroutine((new SkippablePopupWindowEvent()
                    { Title = "Test Title", Message = "Test Message", Duration = 3f }).Execute());
                //PlayerPrefs.SetInt("Tutorial", 1); 빌드 할 때 주석 풀기.
            }

            SlimeEvents slimeEvent = new SlimeEvents(targetSlime, slimes);
            
            yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = false }).Execute());
            yield return StartCoroutine(SceneContext.CameraManager.CameraMove(cameraDestination, cameraSpeed));            
            yield return StartCoroutine(slimeEvent.Execute());
            yield return new WaitForSeconds(_delayTime);
            SceneContext.CameraManager.CameraFollow(GameObject.FindGameObjectWithTag("Player").transform);
            yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = true }).Execute());
        }
    }
}

