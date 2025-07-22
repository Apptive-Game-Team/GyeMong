using System.Collections;
using System.Collections.Generic;
using GyeMong.EventSystem.Event;
using GyeMong.EventSystem.Event.Boss;
using GyeMong.EventSystem.Event.Chat;
using GyeMong.EventSystem.Event.CinematicEvent;
using GyeMong.EventSystem.Event.Input;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Summer.NagaWarrior;
using GyeMong.SoundSystem;
using UnityEngine;

namespace GyeMong.GameSystem.Map.Boss
{
    public class NagaWarriorEntrance : BossRoomEntrance
    {
        [SerializeField] private Vector3 playerDestination;
        [SerializeField] private float playerMoveSpeed;
        [SerializeField] private GameObject wall;
        [SerializeField] private Vector3 cameraDestination;
        [SerializeField] private float cameraSpeed;
        [SerializeField] private MultiChatMessageData chatData1;
        [SerializeField] private float autoSkipTime = 3f;
        [SerializeField] private DailyCycleManager dailyCycleManager;
        private float delayTime = 1f;
        private bool _isTriggered = false;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !_isTriggered)
            {
                StartCoroutine(TriggerEvents());
            }
        }
        
        private IEnumerator TriggerEvents()
        {
            _isTriggered = true;
            yield return StartCoroutine( (new SetKeyInputEvent(){_isEnable = false}).Execute());
            yield return StartCoroutine((new MoveCreatureEvent()
            {
                creatureType = MoveCreatureEvent.CreatureType.Player,
                iControllable = null,
                speed = playerMoveSpeed,
                target = playerDestination
            }).Execute());
            yield return StartCoroutine((new SetActiveObject()
            {
                _gameObject = wall,
                isActive = true
            }).Execute());
            yield return StartCoroutine( (new SetKeyInputEvent(){_isEnable = false}).Execute());
            yield return StartCoroutine(SceneContext.CameraManager.CameraMove(cameraDestination, cameraSpeed));
            boss.GetComponent<NagaWarrior>().curBGM = Sound.Play("BGM_Summer_NagaWarrior", true);

            yield return StartCoroutine((new OpenChatEvent().Execute()));

            yield return new ShowMessages(chatData1, autoSkipTime).Execute();

            yield return StartCoroutine((new CloseChatEvent().Execute()));

            yield return new WaitForSeconds(delayTime);

            yield return StartCoroutine((new ShowBossHealthBarEvent() { _boss = boss }).Execute());
            yield return StartCoroutine((new CameraFollowPlayer()).Execute());
            boss.ChangeState();
            dailyCycleManager.StartCoroutine(dailyCycleManager.DayCycleRoutine());
            yield return StartCoroutine( (new SetKeyInputEvent(){_isEnable = true}).Execute());
        }
    }
}
