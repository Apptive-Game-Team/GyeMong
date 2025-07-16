using System.Collections;
using GyeMong.EventSystem.Event;
using GyeMong.EventSystem.Event.Chat;
using GyeMong.EventSystem.Event.CinematicEvent;
using GyeMong.EventSystem.Event.Input;
using GyeMong.SoundSystem;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Summer.NagaRogue
{
    public class NagaRogueOpeningEvent : MonoBehaviour
    {
        [SerializeField] private global::NagaRogue nagaRogue; 
        [SerializeField] private Vector3 playerDestination;
        [SerializeField] private float playerMoveSpeed;
        [SerializeField] private GameObject wall;
        [SerializeField] private Vector3 cameraDestination;
        [SerializeField] private float cameraSpeed;
        [SerializeField] private float cameraZoomSize;
        [SerializeField] private float cameraZoomSpeed;
        [SerializeField] private MultiChatMessageData battleOpeningChat;
        private float autoSkipTime = 3f;
        private bool _isTriggered = false;

        public IEnumerator TriggerEvents()
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
            yield return new ShowMessages(battleOpeningChat, autoSkipTime).Execute();
            yield return StartCoroutine((new CloseChatEvent().Execute()));
            Sound.Play("BGM_Battle_NagaRogue",true);
            yield return StartCoroutine( (new SetKeyInputEvent(){_isEnable = false}).Execute());
            nagaRogue.ChangeState();
            yield return StartCoroutine( (new SetKeyInputEvent(){_isEnable = true}).Execute());
        }
    }
}
