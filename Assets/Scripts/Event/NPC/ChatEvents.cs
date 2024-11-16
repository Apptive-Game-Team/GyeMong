using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class OpenChatEvent : Event
{
    public override IEnumerator execute()
    {
        return EffectManager.Instance.GetChatController().Open();
    }
}

public class CloseChatEvent : Event
{
    public override IEnumerator execute()
    {
        EffectManager.Instance.GetChatController().Close();
        yield return null;
    }
}

[Serializable]
public class ShowChatEvent : Event
{
    [SerializeField]
    ChatMessage message;

    public override IEnumerator execute()
    {
        return EffectManager.Instance.GetChatController().Chat(message);
    }
}