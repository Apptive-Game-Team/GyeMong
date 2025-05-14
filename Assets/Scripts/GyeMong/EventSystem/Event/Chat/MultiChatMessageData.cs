using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GyeMong.EventSystem.Event.Chat
{
    [CreateAssetMenu(fileName = "MultiChatMessageData", menuName = "ScriptableObject/MultiChatMessageData", order = 1)]
    public class MultiChatMessageData : ScriptableObject
    {
        [Serializable]
        public class MultiChatMessage
        {
            public ChatSpeakerType speakerName;
            public List<string> messages;
            public float chatDelay = 3f;
            public BackgroundImage backgroundImage;
        }

        public List<MultiChatMessage> chatMessages;
    }
}