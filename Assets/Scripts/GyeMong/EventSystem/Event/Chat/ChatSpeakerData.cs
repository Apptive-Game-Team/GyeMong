using System;
using System.Collections.Generic;
using UnityEngine;

namespace GyeMong.EventSystem.Event.Chat
{
    [CreateAssetMenu(fileName = "ChatSpeakerData", menuName = "ScriptableObject/ChatSpeakerData", order = 1)]
    public class ChatSpeakerData : ScriptableObject
    {

        public enum ChatSpeakerFace
        {
            None = 0,
            Normal = 1,
            Angry = 4,
            Doubt = 10, // 의심
            Embarrassed = 11, // 당황
            Happy = 12, // 행복
            Nodding = 13, // 끄덕임
            Thinking = 14, // 생각
            Smile = 2,
            Sad = 3,
        }

        [Serializable]
        public struct ChatSpeakerFaceInfo
        {   
            public ChatSpeakerFace faceType;
            public Sprite faceImage;
        }
            
        
        [Serializable]
        public struct ChatSpeakerInfo
        {
            public ChatSpeakerType speakerType;
            public string speakerName;
            public Sprite image;
            public List<ChatSpeakerFaceInfo> Faces;
        }

        public List<ChatSpeakerInfo> ChatSpeakers;
    }
}

