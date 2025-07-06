using GyeMong.EventSystem.Event.Boss;
using GyeMong.EventSystem.Event.Chat;
using GyeMong.GameSystem.Creature.Player.Component;
using System.Collections;
using UnityEngine;

namespace GyeMong.GameSystem.Map.MapEvent
{
    public class ElfDown : MonoBehaviour
    {
        [Header("Chat Data")]
        [SerializeField] private MultiChatMessageData chatData;
        [SerializeField] private float autoSkipTime = 3f;

        [Header("Boss Room Object")]
        [SerializeField] private GameObject bossRoomBgm1;
        [SerializeField] private GameObject bossRoomBgm2;
        [SerializeField] private GameObject bossRoomObj_wall;

        public IEnumerator Trigger()
        {
            return TriggerEvents();
        }

        private IEnumerator TriggerEvents()
        {
            yield return StartCoroutine((new OpenChatEvent().Execute()));

            yield return new ShowMessages(chatData, autoSkipTime).Execute();

            yield return StartCoroutine((new CloseChatEvent().Execute()));

            var activateBossRoomEvent = new ActivateBossRoomEvent();
            activateBossRoomEvent.SetBossRoomObject(bossRoomBgm1);
            yield return activateBossRoomEvent.Execute();

            var deactivateEvent = new DeActivateBossRoomEvent();
            deactivateEvent.SetBossRoomObject(bossRoomBgm2);
            yield return deactivateEvent.Execute();

            yield return new HideBossHealthBarEvent().Execute();

            deactivateEvent.SetBossRoomObject(bossRoomObj_wall);
            yield return deactivateEvent.Execute();
        }
    }
}
