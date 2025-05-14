using System;
using System.Collections;
using System.Collections.Generic;
using GyeMong.EventSystem.Controller;
using GyeMong.GameSystem.Creature.Player;
using GyeMong.InputSystem;
using UnityEngine;
using GyeMong.SoundSystem;

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
        [SerializeField] List<MultiChatMessage> multiMessages;
        [SerializeField] float autoSkipTime = 3f;
        float soundDelay = 0.2f;
        SoundObject _soundObject;


        public override IEnumerator Execute(EventObject eventObject = null)
        {
            foreach (MultiChatMessage chat in multiMessages)
            {
                _soundObject = Sound.Play("EFFECT_Chat_Sound", true);
                yield return new WaitForSeconds(soundDelay);
                Sound.Stop(_soundObject);
                yield return EffectManager.Instance.GetChatController().MultipleChat(chat, autoSkipTime);
            }
        }

    }

    [Serializable]
    public class ShowChatEvent : ChatEvent
    {
        [SerializeField] ChatMessage message;
        [SerializeField] float autoSkipTime = 3f;

        public override IEnumerator Execute(EventObject eventObject = null)
        {
            yield return EffectManager.Instance.GetChatController().Chat(message, autoSkipTime);
        }
    }

    [Serializable]
    public class ShowChatSequence : ChatEvent
    {
        [SerializeField] MultiChatMessage messages;
        [SerializeField] float autoSkipTime = 3f;

        public override IEnumerator Execute(EventObject eventObject = null)
        {
            foreach (string message in messages.messages)
            {
                yield return EffectManager.Instance.GetChatController().Chat(new ChatMessage(messages.name, message), autoSkipTime);
                yield return new SkippableDelayEvent() { delayTime = 999 }.Execute(eventObject);
            }
        }
    }

    [Serializable]
    public class MultipleShowChatEvent : ChatEvent
    {
        [SerializeField] MultiChatMessage messages;
        [SerializeField] float autoSkipTime = 3f;

        public override IEnumerator Execute(EventObject eventObject = null)
        {
            return EffectManager.Instance.GetChatController().MultipleChat(messages, autoSkipTime);
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