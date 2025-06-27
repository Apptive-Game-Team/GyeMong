using System;
using System.Collections.Generic;
using UnityEngine;

namespace GyeMong.EventSystem.Event.Chat
{
    [CreateAssetMenu(fileName = "ChatSpeakerData", menuName = "ScriptableObject/ChatSpeakerData", order = 1)]
    public class ChatSpeakerData : ScriptableObject
    {
        [Serializable]
        public struct ChatSpeakerInfo
        {
            public ChatSpeakerType speakerType;
            public string speakerName;
            public Sprite image;
        }

        public List<ChatSpeakerInfo> ChatSpeakers;
    }
}

