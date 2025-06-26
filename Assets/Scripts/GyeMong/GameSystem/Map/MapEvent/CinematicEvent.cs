using GyeMong.EventSystem.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Visual.Camera;
using GyeMong.EventSystem.Controller;
using GyeMong.EventSystem.Event;
using GyeMong.EventSystem.Event.Chat;

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
        }
    }
}

