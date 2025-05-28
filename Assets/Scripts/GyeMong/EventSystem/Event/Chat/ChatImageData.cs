using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GyeMong.EventSystem.Event.Chat
{
    [CreateAssetMenu(fileName = "ChatImageData", menuName = "ScriptableObject/ChatImageData")]
    public class ChatImageData : ScriptableObject
    {
        [Serializable]
        public struct ChatImageInfo
        {
            public ChatImage chatImage;
            public Sprite image;
        }

        public List<ChatImageInfo> chatImages;
    }
}

