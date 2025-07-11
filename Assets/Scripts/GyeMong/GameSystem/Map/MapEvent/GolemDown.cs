using GyeMong.EventSystem.Event.Boss;
using GyeMong.EventSystem.Event.Chat;
using GyeMong.EventSystem.Event.CinematicEvent;
using GyeMong.EventSystem.Event.Input;
using GyeMong.GameSystem.Creature.Player.Component;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GyeMong.GameSystem.Map.MapEvent
{
    public class GolemDown : MonoBehaviour
    {
        [Header("Stop Animator")]
        [SerializeField] private Animator targetAnimator;

        [Header("Change Sprite")]
        [SerializeField] private SpriteRenderer targetSpriteRenderer;
        [SerializeField] private Sprite newSprite1;

        [Header("Chat Data")]
        [SerializeField] private MultiChatMessageData chatData;
        [SerializeField] private float autoSkipTime = 3f;

        [Header("Boss Room Object")]
        [SerializeField] private GameObject bossRoomObj1;
        [SerializeField] private GameObject bossRoomObj2;

        public IEnumerator Trigger()
        {
            return TriggerEvents();
        }

        private IEnumerator TriggerEvents()
        {
            var stopAnimEvent = new CustomStopAnimatorEvent();
            stopAnimEvent.SetAnimator(targetAnimator);
            yield return stopAnimEvent.Execute();

            var changeSpriteEvent = new ChangeSpriteEvent();
            changeSpriteEvent.SetSpriteRenderer(targetSpriteRenderer);
            changeSpriteEvent.SetSprite(newSprite1);
            yield return changeSpriteEvent.Execute();

            yield return new HideBossHealthBarEvent().Execute();

            yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = false }).Execute());

            yield return StartCoroutine((new OpenChatEvent().Execute()));

            yield return new ShowMessages(chatData, autoSkipTime).Execute();

            yield return StartCoroutine((new CloseChatEvent().Execute()));

            yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = true }).Execute());

            SceneContext.CameraManager.CameraFollow(GameObject.FindGameObjectWithTag("Player").transform);

            var zoomEvent = new CameraZoomInOut();
            zoomEvent.SetSize(3.6f);
            zoomEvent.SetDuration(0.5f);
            yield return StartCoroutine(zoomEvent.Execute());

            var activateBossRoomEvent = new ActivateBossRoomEvent();

            var deactivateEvent = new DeActivateBossRoomEvent();

            deactivateEvent.SetBossRoomObject(bossRoomObj1);
            yield return deactivateEvent.Execute();

            deactivateEvent.SetBossRoomObject(bossRoomObj2);
            yield return deactivateEvent.Execute();
        }

        public class CustomStopAnimatorEvent : StopAnimatorEvent
        {
            public void SetAnimator(Animator animator)
            {
                _animator = animator;
            }
        }
    }
}
