using System.Collections;
using DG.Tweening;
using GyeMong.EventSystem.Event;
using GyeMong.EventSystem.Event.Chat;
using GyeMong.EventSystem.Event.CinematicEvent;
using GyeMong.EventSystem.Event.Input;
using GyeMong.GameSystem.Map.Portal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Summer.NagaRogue
{
    public class SummerSceneOpeningEvent : MonoBehaviour
    {
        [SerializeField] private Vector3 playerDestination;
        [SerializeField] private float playerMoveSpeed;
        [SerializeField] private Vector3 cameraDestination;
        [SerializeField] private GameObject minionObject;
        [SerializeField] private float cameraSpeed;
        [SerializeField] private MultiChatMessageData battleOpeningChat;
        [SerializeField] private NagaRogueOpeningEvent nagaRogueOpeningEvent;
        private float autoSkipTime = 3f;

        private void Start()
        {
            StartCoroutine(TriggerEvents());
        }

        private IEnumerator TriggerEvents()
        {
            yield return SceneContext.CameraManager.CameraZoomInOut(2.5f, 0);
            GameObject minion = Instantiate(minionObject, new Vector3(cameraDestination.x, cameraDestination.y, 0f), Quaternion.identity);
            yield return StartCoroutine( (new SetKeyInputEvent(){_isEnable = false}).Execute());
            yield return StartCoroutine((new MoveCreatureEvent()
            {
                creatureType = MoveCreatureEvent.CreatureType.Player,
                iControllable = null,
                speed = playerMoveSpeed,
                target = playerDestination
            }).Execute());
            yield return new WaitForSeconds(0.5f);
            //SceneContext.CameraManager.CameraFollow(minion.transform);
            yield return SceneContext.Character.transform.DOScale(new Vector3(0.7f,0.7f,0.7f), cameraSpeed);
            StartCoroutine(
                SceneContext.CameraManager.CameraMove(new Vector3(cameraDestination.x, cameraDestination.y, -10f),
                    cameraSpeed));
            yield return StartCoroutine(SceneContext.CameraManager.CameraZoomInOut(3.5f, cameraSpeed));
            yield return StartCoroutine((new OpenChatEvent().Execute()));
            yield return new ShowMessages(battleOpeningChat, autoSkipTime).Execute();
            yield return StartCoroutine((new CloseChatEvent().Execute()));
            yield return StartCoroutine( (new SetKeyInputEvent(){_isEnable = false}).Execute());
            yield return StartCoroutine( (new SetKeyInputEvent(){_isEnable = true}).Execute());
            Destroy(minion);
            SceneContext.Character.transform.localScale = new Vector3(1f, 1f, 1f);
            SceneContext.CameraManager.CameraFollow(SceneContext.Character.transform);
            yield return StartCoroutine(SceneContext.CameraManager.CameraZoomInOut(5f, cameraSpeed));
            StartCoroutine(nagaRogueOpeningEvent.TriggerEvents());
        }
    }
}
