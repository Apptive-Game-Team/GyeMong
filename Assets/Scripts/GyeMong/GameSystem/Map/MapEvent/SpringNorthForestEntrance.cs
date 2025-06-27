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

            //0 : Input Key False
            yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = false }).Execute());
            Debug.Log("0");

            //1 : Stop Animator
            var stopAnimEvent = new CustomStopAnimatorEvent();
            stopAnimEvent.SetAnimator(targetAnimator);
            yield return stopAnimEvent.Execute();
            Debug.Log("1");

            //2 : Change Sprite
            var changeSpriteEvent = new ChangeSpriteEvent();
            changeSpriteEvent.SetSpriteRenderer(targetSpriteRenderer);
            changeSpriteEvent.SetSprite(newSprite1);
            yield return changeSpriteEvent.Execute();
            Debug.Log("2");

            //3 : Move Creature
            var moveEvent = new MoveCreatureEvent();
            moveEvent.creatureType = MoveCreatureEvent.CreatureType.Player;
            moveEvent.iControllable = null;
            moveEvent.target = moveTarget.position;
            moveEvent.speed = moveSpeed;
            yield return StartCoroutine(moveEvent.Execute());
            Debug.Log("3");

            //4 : Camera Move
            yield return StartCoroutine(SceneContext.CameraManager.CameraMove(cameraDestination, cameraSpeed));
            Debug.Log("4");

            //5 : Activative Boss
            var activateBossRoomEvent = new ActivateBossRoomEvent();
            activateBossRoomEvent.SetBossRoomObject(bossRoomObj);
            yield return activateBossRoomEvent.Execute();
            Debug.Log("5");

            //6
            var deactivateEvent = new DeActivateBossRoomEvent();
            deactivateEvent.SetBossRoomObject(bossRoomObj);
            yield return deactivateEvent.Execute();
            Debug.Log("6");

            //7
            yield return StartCoroutine((new OpenChatEvent().Execute()));
            Debug.Log("7");

            //8
            yield return new ShowMessages(chatData1, autoSkipTime).Execute();
            Debug.Log("8");

            //9
            changeSpriteEvent.SetSpriteRenderer(targetSpriteRenderer);
            changeSpriteEvent.SetSprite(newSprite2);
            yield return changeSpriteEvent.Execute();
            Debug.Log("9");

            //10
            yield return new WaitForSeconds(delayTime);
            Debug.Log("10");

            //11
            yield return new ShowMessages(chatData2, autoSkipTime).Execute();
            Debug.Log("11");

            //12
            yield return StartCoroutine((new CloseChatEvent().Execute()));
            Debug.Log("12");

            //13
            activateBossRoomEvent.SetBossRoomObject(bossRoomObj_wall);
            yield return activateBossRoomEvent.Execute();
            Debug.Log("13");

            //14
            var startEvent = new StartAnimatorEvent();
            startEvent.SetAnimator(someAnimator);
            yield return startEvent.Execute();
            Debug.Log("14");

            //15
            var showHpEvent = new ShowBossHealthBarEvent();
            showHpEvent.SetBoss(boss);
            yield return showHpEvent.Execute();
            Debug.Log("15");

            //16
            var animParamEvent = new SetAnimatorParameter();
            animParamEvent._creatureType = SetAnimatorParameter.CreatureType.Player;
            animParamEvent.SetParameter("Speed", 10f);
            yield return animParamEvent.Execute();
            Debug.Log("16");

            //17
            yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = true }).Execute());
            Debug.Log("17");

            //18
            SceneContext.CameraManager.CameraFollow(GameObject.FindGameObjectWithTag("Player").transform);
            Debug.Log("18");

            //19
            bossRoomEntrance.Trigger();
            Debug.Log("19");
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
