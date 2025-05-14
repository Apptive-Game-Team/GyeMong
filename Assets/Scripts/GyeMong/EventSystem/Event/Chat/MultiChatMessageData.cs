using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MultiChatMessageData", menuName = "ScriptableObject/MultiChatMessageData", order = 1)]
public class MultiChatMessageData : ScriptableObject
{
    [Serializable]
    public class MultiChatMessage
    {
        public string speakerName;
        public List<string> messages;
        public float chatDelay = 3f;
    }

    public List<MultiChatMessage> chatMessages;
}
