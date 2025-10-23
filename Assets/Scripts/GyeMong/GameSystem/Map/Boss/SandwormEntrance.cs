using System.Collections;
using GyeMong.EventSystem.Event;
using GyeMong.EventSystem.Event.Boss;
using GyeMong.EventSystem.Event.Chat;
using GyeMong.EventSystem.Event.CinematicEvent;
using GyeMong.EventSystem.Event.Input;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Sandworm;
using GyeMong.SoundSystem;
using UnityEngine;

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

        private void Start()
        {
            StartCoroutine(boss.GetComponent<Sandworm>().movement.HideOrShow(true, 0.2f));
            StartCoroutine(TriggerEvents());
        }
        
        private IEnumerator TriggerEvents()
        {
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
            boss.gameObject.SetActive(true);
            yield return boss.GetComponent<Sandworm>().movement.HideOrShow(false, 0.5f);
            yield return multiMessages.Play();
            boss.GetComponent<Sandworm>().curBGM = Sound.Play("BGM_Summer_Sandworm", true);
            yield return StartCoroutine(SceneContext.CameraManager.CameraZoomInOut(cameraZoomSize, cameraZoomSpeed));
            yield return StartCoroutine((new ShowBossHealthBarEvent() { _boss = boss }).Execute());
            boss.ChangeState();
            mapPattern.StartPattern();
            yield return StartCoroutine( (new SetKeyInputEvent(){_isEnable = true}).Execute());
        }
    }
}
