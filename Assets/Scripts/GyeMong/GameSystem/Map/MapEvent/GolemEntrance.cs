using System;
using GyeMong.EventSystem.Event.Boss;
using GyeMong.EventSystem.Event.CinematicEvent;
using GyeMong.EventSystem.Event.Input;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Slime.Components;
using GyeMong.GameSystem.Map.Boss;
using System.Collections;
using System.Collections.Generic;
using GyeMong.EventSystem.Event.Chat;
using UnityEngine;

namespace GyeMong.GameSystem.Map.MapEvent
{
    public class GolemEntrance : MonoBehaviour
    {
        [Header("Target Animator")]
        [SerializeField] private GolemIKController golem;

        [Header("Change Sprite")]
        [SerializeField] private SpriteRenderer targetSpriteRenderer;
        [SerializeField] private Sprite newSprite1;
        [SerializeField] private Sprite newSprite2;

        [Header("Move Creature : Player")]
        [SerializeField] private Transform moveTarget;
        [SerializeField] private float moveSpeed = 2f;

        [Header("Camera Move")]
        [SerializeField] private Vector3 cameraDestination1;
        [SerializeField] private Vector3 cameraDestination2;
        [SerializeField] private float cameraSpeed1;
        [SerializeField] private float cameraSpeed2;

        [Header("Boss Room Object")]
        [SerializeField] private GameObject bossRoomObj1;
        [SerializeField] private GameObject bossRoomObj2;

        [Header("Boss")]
        [SerializeField] private GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Boss boss;

        [Header("Boss Room Entrance")]
        [SerializeField] private BossRoomEntrance bossRoomEntrance;

        [Header("Animation Event")]
        [SerializeField] private List<Sprite> animationFrames = new List<Sprite>(6);
        [SerializeField] private SpriteRenderer someRenderer;
        [SerializeField] private List<MultiChatMessageData> beforeScript;

        private bool _isTriggered = false;

        private void Start()
        {
            StartCoroutine(TriggerEvents());
        }

        private IEnumerator TriggerEvents()
        {
            yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = false }).Execute());
            _isTriggered = true;

            golem.StopAnimation();

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

            yield return StartCoroutine(SceneContext.CameraManager.CameraMove(cameraDestination1, cameraSpeed1));

            yield return StartCoroutine(SceneContext.CameraManager.CameraMove(cameraDestination2, cameraSpeed2));

            var zoomEvent = new CameraZoomInOut();
            zoomEvent.SetSize(10f);
            zoomEvent.SetDuration(3f);
            yield return StartCoroutine(zoomEvent.Execute());

            var deactivateEvent = new DeActivateBossRoomEvent();

            var activateBossRoomEvent = new ActivateBossRoomEvent();

            yield return new WaitForSeconds(1);

            changeSpriteEvent.SetSpriteRenderer(targetSpriteRenderer);
            changeSpriteEvent.SetSprite(newSprite2);
            yield return changeSpriteEvent.Execute();

            yield return new WaitForSeconds(1);

            BgmManager.Play("BGM_Spring_Boss");
            
            var animEvent = new GyeMong.EventSystem.Event.CinematicEvent.AnimationEvent();
            animEvent.SetRenderer(someRenderer);
            animEvent.SetFrames(animationFrames);
            animEvent.SetDeltaTime(0.5f);
            yield return animEvent.Execute();

            golem.StartIdleAnimation();
            
            if (beforeScript != null)
            {
                foreach (var script in beforeScript)
                {
                    yield return script.Play();
                }
            }

            var showHpEvent = new ShowBossHealthBarEvent();
            showHpEvent.SetBoss(boss);
            yield return showHpEvent.Execute();

            var animParamEvent = new SetAnimatorParameter();
            animParamEvent._creatureType = SetAnimatorParameter.CreatureType.Player;
            animParamEvent.SetParameter("Speed", 10f);
            yield return animParamEvent.Execute();

            yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = true }).Execute());

            bossRoomEntrance.Trigger();

            activateBossRoomEvent.SetBossRoomObject(bossRoomObj1);
            yield return activateBossRoomEvent.Execute();

            activateBossRoomEvent.SetBossRoomObject(bossRoomObj2);
            yield return activateBossRoomEvent.Execute();
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

