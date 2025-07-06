using System.Collections;
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
        [SerializeField] private float cameraSpeed;
        [SerializeField] private MultiChatMessageData battleOpeningChat;
        private float autoSkipTime = 3f;

        private void Start()
        {
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
            
            yield return StartCoroutine(SceneContext.CameraManager.CameraMove(cameraDestination, cameraSpeed));
            yield return StartCoroutine((new OpenChatEvent().Execute()));
            yield return new ShowMessages(battleOpeningChat, autoSkipTime).Execute();
            yield return StartCoroutine((new CloseChatEvent().Execute()));
            yield return StartCoroutine( (new SetKeyInputEvent(){_isEnable = false}).Execute());
            yield return StartCoroutine( (new SetKeyInputEvent(){_isEnable = true}).Execute());
            SceneManager.LoadScene("Scenes/Summer/NagaRogueScene");
        }
    }
}
