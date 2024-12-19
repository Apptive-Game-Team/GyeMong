using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

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