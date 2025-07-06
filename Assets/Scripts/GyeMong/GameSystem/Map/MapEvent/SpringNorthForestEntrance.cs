using GyeMong.EventSystem.Event.Boss;
using GyeMong.EventSystem.Event.Chat;
using GyeMong.EventSystem.Event.CinematicEvent;
using GyeMong.EventSystem.Event.Input;
using GyeMong.GameSystem.Map.Boss;
using System.Collections;
using UnityEngine;

namespace GyeMong.GameSystem.Map.MapEvent
{
    public class SpringNorthForestEntrance : MonoBehaviour
    {
        [Header("Boss Room Entrance")]
        [SerializeField] private BossRoomEntrance bossRoomEntrance;

        [Header("Stop Animator")]
        [SerializeField] private Animator targetAnimator;

        [Header("Change Sprite")]
        [SerializeField] private SpriteRenderer targetSpriteRenderer;
        [SerializeField] private Sprite newSprite1;
        [SerializeField] private Sprite newSprite2;

        [Header("Move Creature : Player")]
        [SerializeField] private Transform moveTarget;
        [SerializeField] private float moveSpeed = 2f;

        [Header("Camera Move")]
        [SerializeField] private Vector3 cameraDestination;
        [SerializeField] private float cameraSpeed;

        [Header("Boss Room Object")]
        [SerializeField] private GameObject bossRoomObj;
        [SerializeField] private GameObject bossRoomObj_wall;

        [Header("Chat Data")]
        [SerializeField] private MultiChatMessageData chatData1;
        [SerializeField] private MultiChatMessageData chatData2;
        [SerializeField] private float autoSkipTime = 3f;

        [Header("Start Animator")]
        [SerializeField] private Animator someAnimator;

        [Header("")]
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

            yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = false }).Execute());
 
            var stopAnimEvent = new CustomStopAnimatorEvent();
            stopAnimEvent.SetAnimator(targetAnimator);
            yield return stopAnimEvent.Execute();

            var changeSpriteEvent = new ChangeSpriteEvent();
            changeSpriteEvent.SetSpriteRenderer(targetSpriteRenderer);
            changeSpriteEvent.SetSprite(newSprite1);
            yield return changeSpriteEvent.Execute();

            var moveEvent = new MoveCreatureEvent();
            moveEvent.creatureType = MoveCreatureEvent.CreatureType.Player;
            moveEvent.iControllable = null;
            moveEvent.target = moveTarget.position;
            moveEvent.speed = moveSpeed;
            yield return StartCoroutine(moveEvent.Execute());

            yield return StartCoroutine(SceneContext.CameraManager.CameraMove(cameraDestination, cameraSpeed));

            var activateBossRoomEvent = new ActivateBossRoomEvent();
            activateBossRoomEvent.SetBossRoomObject(bossRoomObj);
            yield return activateBossRoomEvent.Execute();

            var deactivateEvent = new DeActivateBossRoomEvent();
            deactivateEvent.SetBossRoomObject(bossRoomObj);
            yield return deactivateEvent.Execute();

            yield return StartCoroutine((new OpenChatEvent().Execute()));

            yield return new ShowMessages(chatData1, autoSkipTime).Execute();

            changeSpriteEvent.SetSpriteRenderer(targetSpriteRenderer);
            changeSpriteEvent.SetSprite(newSprite2);
            yield return changeSpriteEvent.Execute();

            yield return new WaitForSeconds(delayTime);

            yield return new ShowMessages(chatData2, autoSkipTime).Execute();

            yield return StartCoroutine((new CloseChatEvent().Execute()));

            activateBossRoomEvent.SetBossRoomObject(bossRoomObj_wall);
            yield return activateBossRoomEvent.Execute();

            var startEvent = new StartAnimatorEvent();
            startEvent.SetAnimator(someAnimator);
            yield return startEvent.Execute();

            var showHpEvent = new ShowBossHealthBarEvent();
            showHpEvent.SetBoss(boss);
            yield return showHpEvent.Execute();

            var animParamEvent = new SetAnimatorParameter();
            animParamEvent._creatureType = SetAnimatorParameter.CreatureType.Player;
            animParamEvent.SetParameter("Speed", 10f);
            yield return animParamEvent.Execute();

            yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = true }).Execute());

            SceneContext.CameraManager.CameraFollow(GameObject.FindGameObjectWithTag("Player").transform);

            bossRoomEntrance.Trigger();
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
