using System;
using System.Collections;
using System.Collections.Generic;
using GyeMong.EventSystem.Controller;
using GyeMong.GameSystem.Creature.Player;
using GyeMong.InputSystem;
using UnityEngine;
using GyeMong.SoundSystem;
using GyeMong.GameSystem.Creature.Player.Component;

namespace GyeMong.EventSystem.Event.Chat
{
    public abstract class ChatEvent : Event
    {
    }

    public class OpenChatEvent : ChatEvent
    {
        public override IEnumerator Execute(EventObject eventObject = null)
        {
            InputManager.Instance.SetActionState(false);
            PlayerCharacter.Instance.isControlled = true;
            PlayerCharacter.Instance.StopPlayer();
            return EffectManager.Instance.GetChatController().Open();
        }
    }

    public class CloseChatEvent : ChatEvent
    {
        public override IEnumerator Execute(EventObject eventObject = null)
        {
            InputManager.Instance.SetActionState(true);
            PlayerCharacter.Instance.isControlled = false;
            PlayerCharacter.Instance.StopPlayer(true);
            EffectManager.Instance.GetChatController().Close();
            yield return null;
        }
    }

    [Serializable]
    public class ShowMessages : ChatEvent
    {
        [SerializeField] private MultiChatMessageData chatData;
        [SerializeField] private BackgroundImageData backgroundImageData;

        [SerializeField] float autoSkipTime = 3f;
        float soundDelay = 0.2f;
        SoundObject _soundObject;

        public override IEnumerator Execute(EventObject eventObject = null)
        {
            foreach (var chat in chatData.chatMessages)
            {
                _soundObject = Sound.Play("EFFECT_Chat_Sound", true);
                yield return new WaitForSeconds(soundDelay);
                Sound.Stop(_soundObject);

                var backgroundSprite = GetBackgroundImageSprite(chat.backgroundImage);

                var multiChatMessage = new GyeMong.EventSystem.Controller.MultiChatMessage
                {
                    name = chat.speakerName.ToString(),
                    messages = chat.messages,
                    chatDelay = chat.chatDelay,
                    backgroundImage = backgroundSprite,
                };

                yield return EffectManager.Instance.GetChatController().MultipleChat(multiChatMessage, autoSkipTime);
            }
        }

        private Sprite GetBackgroundImageSprite(BackgroundImage backgroundImage)
        {
            foreach (var imageInfo in backgroundImageData.backgroundImages)
            {
                if (imageInfo.backgroundImage == backgroundImage)
                {
                    return imageInfo.image;
                }
            }

            Debug.LogWarning($"Background image not found for {backgroundImage}");
            return null;
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