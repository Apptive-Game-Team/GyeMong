using GyeMong.EventSystem.Event.Input;
using System.Collections;
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
        private float delayTime = 1f;

        private bool _isTriggered = false;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !_isTriggered)
            {
                StartCoroutine(TriggerEvents());
            }
        }

        public IEnumerator Trigger()
        {
            return TriggerEvents();
        }

        private IEnumerator TriggerEvents()
        {
            _isTriggered = true;

            SlimeEvents slimeEvent = new SlimeEvents(targetSlime, slimes);

            yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = false }).Execute());
            yield return StartCoroutine(SceneContext.CameraManager.CameraMove(cameraDestination, cameraSpeed));            
            yield return StartCoroutine(slimeEvent.Execute());
            yield return new WaitForSeconds(delayTime);
            SceneContext.CameraManager.CameraFollow(GameObject.FindGameObjectWithTag("Player").transform);
            yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = true }).Execute());
        }
    }
}

