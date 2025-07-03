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
            PlayerPrefs.SetInt("TutorialFlag", 0);
            _isTutorial = PlayerPrefs.GetInt("TutorialFlag", 0) == 0;
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
                    { Title = "플레이어 이동", Message = "W, A, S, D를 눌러서 이동할 수 있다.", Duration = 3f }).Execute());
                yield return StartCoroutine((new SkippablePopupWindowEvent()
                    { Title = "돌진", Message = "좌측 Shift키를 눌러 돌진할 수 있다.", Duration = 3f }).Execute());
                yield return StartCoroutine((new SkippablePopupWindowEvent()
                    { Title = "공격", Message = "마우스 좌 클릭을 통해 공격할 수 있다.", Duration = 3f }).Execute());
            }

            SlimeEvents slimeEvent = new SlimeEvents(targetSlime, slimes);
            
            yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = false }).Execute());
            yield return StartCoroutine(SceneContext.CameraManager.CameraMove(cameraDestination, cameraSpeed));            
            yield return StartCoroutine(slimeEvent.Execute());
            yield return new WaitForSeconds(_delayTime);
            SceneContext.CameraManager.CameraFollow(SceneContext.Character.gameObject.transform);
            yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = true }).Execute());
        }
    }
}

