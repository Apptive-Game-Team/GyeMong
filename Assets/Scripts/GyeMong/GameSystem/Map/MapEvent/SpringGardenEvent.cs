using GyeMong.EventSystem.Event.Input;
using System.Collections;
using System.Collections.Generic;
using GyeMong.EventSystem.Event;
using GyeMong.EventSystem.Event.Chat;
using GyeMong.EventSystem.Event.CinematicEvent;
using UnityEngine;
using GyeMong.EventSystem.Event.EventScene;

namespace GyeMong.GameSystem.Map.MapEvent
{
    public class SpringGardenEvent : MonoBehaviour
    {
        [SerializeField] private GameObject targetSlime;
        [SerializeField] private GameObject[] slimes;

        [SerializeField] private float cameraSize1;
        [SerializeField] private float cameraDuration1;
        [SerializeField] private Vector3 cameraDestination1;
        [SerializeField] private float cameraSpeed1;
        [SerializeField] private Vector3 cameraDestination1_2;
        [SerializeField] private float cameraSpeed1_2;
        [SerializeField] private float cameraSpeed2;
        [SerializeField] private Vector3 cameraDestination3;
        [SerializeField] private float cameraSpeed3;
        [SerializeField] private float cameraSize3;
        [SerializeField] private float cameraDuration3;
        [SerializeField] private List<MultiChatMessageData> beforeScript;
        [SerializeField] private GameObject elfChild;
        private float _delayTime = 1f;

        private bool _isTutorial;

        private void Start()
        {
            _isTutorial = !PlayerPrefs.HasKey("TutorialFlag");
            StartCoroutine(TriggerEvents());
        }

        public IEnumerator Trigger()
        {
            return TriggerEvents();
        }

        private IEnumerator TriggerEvents()
        {
            yield return (new SetKeyInputEvent() { _isEnable = false }).Execute();
            
            // 해변 도착
            var cameraZoomEvent1 = new CameraZoomInOut();
            cameraZoomEvent1.SetSize(cameraSize1);
            cameraZoomEvent1.SetDuration(cameraDuration1);
            yield return cameraZoomEvent1.Execute();
            var cameraMoveEvent1 = new CameraMove();
            cameraMoveEvent1.SetDestination(cameraDestination1);
            cameraMoveEvent1.SetSpeed(cameraSpeed1);
            yield return cameraMoveEvent1.Execute();
            yield return StartCoroutine(SceneContext.EffectManager.FadeOut(0f));
            yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(SceneContext.EffectManager.FadeIn(1f));
            yield return new WaitForSeconds(0.5f);
            var cameraMoveEvent2 = new CameraMove();
            cameraMoveEvent2.SetDestination(cameraDestination1_2);
            cameraMoveEvent2.SetSpeed(cameraSpeed1_2);
            yield return cameraMoveEvent2.Execute();
            yield return new WaitForSeconds(1f);
            var animParamEvent = new SetAnimatorParameter {_creatureType = SetAnimatorParameter.CreatureType.Player};
            animParamEvent.SetParameter("yDir", 0);
            animParamEvent.SetParameter("xDir", -1);
            yield return animParamEvent.Execute();
            yield return new WaitForSeconds(0.4f);
            animParamEvent.SetParameter("xDir", 1);
            yield return animParamEvent.Execute();
            yield return new WaitForSeconds(0.4f);
            animParamEvent.SetParameter("xDir", -1);
            yield return animParamEvent.Execute();
            yield return new WaitForSeconds(0.4f);
            animParamEvent.SetParameter("xDir", 1);
            yield return animParamEvent.Execute();
            yield return new WaitForSeconds(1f);
            
            
            // 꼬마 엘프 조우
            var cameraMoveEvent = new CameraMove();
            Vector3 cameraDestination = ((SceneContext.Character.transform.position + elfChild.transform.position) / 2);
            cameraDestination.z = -10;
            cameraMoveEvent.SetDestination(cameraDestination);
            cameraMoveEvent.SetSpeed(cameraSpeed2);
            yield return cameraMoveEvent.Execute();
            if (beforeScript != null)
            {
                foreach (var script in beforeScript)
                {
                    yield return script.Play();
                }
            }
            
            if (_isTutorial)
            {
                yield return (new SkippablePopupWindowEvent()
                    { Title = "플레이어 이동", Message = "W, A, S, D를 눌러서 이동할 수 있다.", Duration = 3f }).Execute();
                yield return (new SkippablePopupWindowEvent()
                    { Title = "돌진", Message = "좌측 Shift키를 눌러 돌진할 수 있다.", Duration = 3f }).Execute();
                yield return (new SkippablePopupWindowEvent()
                    { Title = "기본공격", Message = "마우스 좌클릭을 통해 공격할 수 있다.", Duration = 3f }).Execute();
            }
            
            // 슬라임 전투 이벤트
            SlimeEvents slimeEvent = new SlimeEvents(targetSlime, slimes);
            
            yield return (new SetKeyInputEvent() { _isEnable = false }).Execute();
            yield return SceneContext.CameraManager.CameraMove(cameraDestination3, cameraSpeed3);
            yield return (new SetActiveObject() {_gameObject = elfChild, isActive = false}).Execute();
            var cameraZoomEvent3 = new CameraZoomInOut();
            cameraZoomEvent3.SetSize(cameraSize3);
            cameraZoomEvent3.SetDuration(cameraDuration3);
            yield return cameraZoomEvent3.Execute();
            yield return slimeEvent.Execute();
            yield return new WaitForSeconds(_delayTime);
            SceneContext.CameraManager.CameraFollow(SceneContext.Character.gameObject.transform);
            yield return (new SetKeyInputEvent() { _isEnable = true }).Execute();
        }
    }
}

