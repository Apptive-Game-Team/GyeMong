using GyeMong.EventSystem.Event.Boss;
using GyeMong.EventSystem.Event.Chat;
using GyeMong.EventSystem.Event.CinematicEvent;
using GyeMong.EventSystem.Event.EventScene;
using GyeMong.EventSystem.Event.Input;
using GyeMong.GameSystem.Creature.Player.Component;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Visual.Camera;

namespace GyeMong.GameSystem.Map.MapEvent
{
    public class SpringNorthForestEntrance : MonoBehaviour
    {
        [SerializeField] private Animator targetAnimator;

        [SerializeField] private SpriteRenderer targetSpriteRenderer;
        [SerializeField] private Sprite newSprite;

        [SerializeField] private Transform moveTarget;
        [SerializeField] private float moveSpeed = 2f;

        [SerializeField] private Vector3 cameraDestination;
        [SerializeField] private float cameraSpeed;

        [SerializeField] private GameObject bossRoomObj;

        [SerializeField] private MultiChatMessageData chatData;
        [SerializeField] private float autoSkipTime = 3f;

        [SerializeField] private Animator someAnimator;

        [SerializeField] private GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Boss boss;


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

            //0
            yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = false }).Execute());

            //1
            var stopAnimEvent = new CustomStopAnimatorEvent();
            stopAnimEvent.SetAnimator(targetAnimator);
            yield return StartCoroutine(stopAnimEvent.Execute());

            //2
            var changeSpriteEvent = new ChangeSpriteEvent();
            changeSpriteEvent.SetSpriteRenderer(targetSpriteRenderer);
            changeSpriteEvent.SetSprite(newSprite);
            yield return changeSpriteEvent.Execute();

            //3
            var moveEvent = new MoveCreatureEvent();
            moveEvent.creatureType = MoveCreatureEvent.CreatureType.Player;
            moveEvent.iControllable = null;
            moveEvent.target = moveTarget.position;
            moveEvent.speed = moveSpeed;
            yield return StartCoroutine(moveEvent.Execute());

            //4
            yield return StartCoroutine(CameraManager.Instance.CameraMove(cameraDestination, cameraSpeed));

            //5
            var activateBossRoomEvent = new ActivateBossRoomEvent();
            activateBossRoomEvent.SetBossRoomObject(bossRoomObj);
            yield return activateBossRoomEvent.Execute();

            //6
            var deactivateEvent = new DeActivateBossRoomEvent();
            deactivateEvent.SetBossRoomObject(bossRoomObj);
            yield return deactivateEvent.Execute();

            //7
            yield return StartCoroutine((new OpenChatEvent().Execute()));

            //8
            yield return new ShowMessages(chatData, autoSkipTime).Execute();

            //9
            changeSpriteEvent.SetSpriteRenderer(targetSpriteRenderer);
            changeSpriteEvent.SetSprite(newSprite);
            yield return changeSpriteEvent.Execute();

            //10
            yield return new WaitForSeconds(delayTime);

            //11
            yield return new ShowMessages(chatData, autoSkipTime).Execute();

            //12
            yield return StartCoroutine((new CloseChatEvent().Execute()));

            //13
            yield return activateBossRoomEvent.Execute();

            //14
            var startEvent = new StartAnimatorEvent();
            startEvent.SetAnimator(someAnimator);
            yield return startEvent.Execute();

            //15
            var showHpEvent = new ShowBossHealthBarEvent();
            showHpEvent.SetBoss(boss);
            yield return showHpEvent.Execute();

            //16
            var animParamEvent = new SetAnimatorParameter();
            animParamEvent._creatureType = SetAnimatorParameter.CreatureType.Player;
            animParamEvent.SetParameter("Speed", 2.0f);
            yield return animParamEvent.Execute();

            //17
            yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = true }).Execute());

            //18
            CameraManager.Instance.CameraFollow(GameObject.FindGameObjectWithTag("Player").transform);

            //19
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
