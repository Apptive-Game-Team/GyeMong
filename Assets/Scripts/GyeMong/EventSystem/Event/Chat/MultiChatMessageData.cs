using System;
using System.Collections;
using System.Collections.Generic;
using GyeMong.EventSystem.Controller;
using GyeMong.SoundSystem;
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
            public bool isLeft;
            public List<string> messages;
            public BackgroundImage backgroundImage;
            public ChatImage chatImage;
            public float chatDelay = 3f;
        }

        public List<MultiChatMessage> chatMessages;

        public IEnumerator Play()
        {
            yield return ChatController.Open();
            foreach (var chat in chatMessages)
            {
                SoundObject _soundObject = Sound.Play("EFFECT_Chat_Sound", true);
                yield return new WaitForSeconds(0.2f);
                
                ChatController.SetBackgroundImage(ChatController.GetBackgroundImageSprite(chat.backgroundImage));
                ChatController.SetChatImage(ChatController.GetChatImageSprite(chat.chatImage));
                
                Sound.Stop(_soundObject);
                yield return ChatController.MultipleChat(chat, 3f);
            }
            ChatController.Close();
        }
    }
}