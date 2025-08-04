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

        [SerializeField] private Vector3 cameraDestination;
        [SerializeField] private float cameraSpeed;
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
            yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = false }).Execute());
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
            
            if (beforeScript != null)
            {
                foreach (var script in beforeScript)
                {
                    yield return script.Play();
                }
            }
            
            if (_isTutorial)
            {
                yield return StartCoroutine((new SkippablePopupWindowEvent()
                    { Title = "플레이어 이동", Message = "W, A, S, D를 눌러서 이동할 수 있다.", Duration = 3f }).Execute());
                yield return StartCoroutine((new SkippablePopupWindowEvent()
                    { Title = "돌진", Message = "좌측 Shift키를 눌러 돌진할 수 있다.", Duration = 3f }).Execute());
                yield return StartCoroutine((new SkippablePopupWindowEvent()
                    { Title = "기본공격", Message = "마우스 좌클릭을 통해 공격할 수 있다.", Duration = 3f }).Execute());
            }

            SlimeEvents slimeEvent = new SlimeEvents(targetSlime, slimes);
            
            yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = false }).Execute());
            yield return StartCoroutine(SceneContext.CameraManager.CameraMove(cameraDestination, cameraSpeed));            
            yield return StartCoroutine(slimeEvent.Execute());
            yield return new WaitForSeconds(_delayTime);
            yield return (new SetActiveObject() {_gameObject = elfChild, isActive = false}).Execute();
            SceneContext.CameraManager.CameraFollow(SceneContext.Character.gameObject.transform);
            yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = true }).Execute());
        }
    }
}

