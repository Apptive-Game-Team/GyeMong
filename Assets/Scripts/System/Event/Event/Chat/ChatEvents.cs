using System.Collections;
using System.Event.Controller;
using UnityEngine;

namespace System.Event.Event.Chat
{
    public abstract class ChatEvent : Event
    {
    }

    public class OpenChatEvent : ChatEvent
    {
        public override IEnumerator Execute(EventObject eventObject = null)
        {
            return EffectManager.Instance.GetChatController().Open();
        }
    }

    public class CloseChatEvent : ChatEvent
    {
        public override IEnumerator Execute(EventObject eventObject = null)
        {
            EffectManager.Instance.GetChatController().Close();
            yield return null;
        }
    }

    [Serializable]
    public class ShowChatEvent : ChatEvent
    {
        [SerializeField]
        ChatMessage message;

        public override IEnumerator Execute(EventObject eventObject = null)
        {
            return EffectManager.Instance.GetChatController().Chat(message);
        }
    }

    [Serializable]
    public class MultipleShowChatEvent : ChatEvent
    {
        [SerializeField]
        MultiChatMessage messages;

        public override IEnumerator Execute(EventObject eventObject = null)
        {
            return EffectManager.Instance.GetChatController().MultipleChat(messages);
        }
    }

    [Serializable]
    public class SpeechBubbleChatEvent : ChatEvent
    {
        [SerializeField] 
        GameObject NPC;
        [SerializeField]
        string message;
        [SerializeField]
        float destroyDelay;
        public override IEnumerator Execute(EventObject eventObject)
        {
            return EffectManager.Instance.GetChatController().ShowSpeechBubbleChat(NPC, message, destroyDelay);
        }
    }
}