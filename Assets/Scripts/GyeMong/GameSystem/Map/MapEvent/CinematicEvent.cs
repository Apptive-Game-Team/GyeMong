using System.Collections;
using UnityEngine;
using GyeMong.EventSystem.Event.Chat;
using GyeMong.GameSystem.Map.Stage.Select;

namespace GyeMong.GameSystem.Map.MapEvent
{
    public class CinematicEvent : MonoBehaviour
    {
        [SerializeField] private MultiChatMessageData chatData;
        [SerializeField] private float autoSkipTime = 3f;
        
        private void Start()
        {
            StartCoroutine(TriggerEvents());
        }

        public IEnumerator Trigger()
        {
            return TriggerEvents();
        }

        private IEnumerator TriggerEvents()
        {
            yield return StartCoroutine((new OpenChatEvent().Execute()));
            yield return new ShowMessages(chatData, autoSkipTime).Execute();
            yield return StartCoroutine((new CloseChatEvent().Execute()));

            GyeMong.GameSystem.Map.Stage.Select.Stage currentStage = GyeMong.GameSystem.Map.Stage.Select.Stage.Beach;
            GyeMong.GameSystem.Map.Stage.Select.Stage maxStage = GyeMong.GameSystem.Map.Stage.Select.Stage.Slime;

            StageSelectPage.LoadStageSelectPageOnStageToDestination(currentStage, maxStage);
        }
    }
}

