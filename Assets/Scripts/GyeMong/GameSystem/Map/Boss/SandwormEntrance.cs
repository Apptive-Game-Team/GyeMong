using System.Collections;
using System.Collections.Generic;
using GyeMong.EventSystem.Controller;
using GyeMong.EventSystem.Event;
using GyeMong.EventSystem.Event.Boss;
using GyeMong.EventSystem.Event.Chat;
using GyeMong.EventSystem.Event.CinematicEvent;
using GyeMong.EventSystem.Event.Input;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Sandworm;
using GyeMong.SoundSystem;
using UnityEngine;
using Visual.Camera;

namespace GyeMong.GameSystem.Map.Boss
{
    public class SandwormEntrance : BossRoomEntrance
    {
        [SerializeField] private TailPattern mapPattern;
        [SerializeField] private Vector3 playerDestination;
        [SerializeField] private float playerMoveSpeed;
        [SerializeField] private GameObject wall;
        [SerializeField] private Vector3 cameraDestination;
        [SerializeField] private float cameraSpeed;
        [SerializeField] private float cameraZoomSize;
        [SerializeField] private float cameraZoomSpeed;
        [SerializeField] private MultiChatMessageData multiMessages;
        [SerializeField] private float autoSkipTime = 3f;
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
            yield return StartCoroutine((new OpenChatEvent().Execute()));
            yield return new ShowMessages(multiMessages);
            yield return StartCoroutine((new CloseChatEvent().Execute()));
            yield return StartCoroutine( (new SetKeyInputEvent(){_isEnable = false}).Execute());
            yield return StartCoroutine(CameraManager.Instance.CameraMove(cameraDestination, cameraSpeed));
            boss.GetComponent<Sandworm>().curBGM = Sound.Play("BGM_Summer_Sandworm", true);
            yield return StartCoroutine(CameraManager.Instance.CameraZoomInOut(cameraZoomSize, cameraZoomSpeed));
            yield return StartCoroutine((new ShowBossHealthBarEvent() { _boss = boss }).Execute());
            boss.ChangeState();
            mapPattern.StartPattern();
            yield return StartCoroutine( (new SetKeyInputEvent(){_isEnable = true}).Execute());
        }
    }
}
