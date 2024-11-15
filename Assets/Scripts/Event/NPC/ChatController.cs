using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[Serializable]
public struct ChatMessage
{
    public string name;
    public string message;
}

public class ChatController : MonoBehaviour
{

    private Image chatWindow;
    private const float CHAT_WINDOW_ALPHA = 0.7f;
    private const float SHOW_CHAT_DELAY = 0.1f;
    private TMP_Text nameText;
    private TMP_Text messageText;

    private void Awake()
    {
        chatWindow = transform.Find("ChatWindow").GetComponent<Image>();
        nameText = chatWindow.transform.Find("NameArea").GetComponent<TMP_Text>();
        messageText = chatWindow.transform.Find("MessageArea").GetComponent<TMP_Text>();
    }

    public void Open()
    {
        Color color = chatWindow.color;
        color.a = CHAT_WINDOW_ALPHA;
        chatWindow.color = color;
        color = messageText.color;
        color.a = 1;
        messageText.color = color;
        nameText.color = color;
    }

    public void Close()
    {
        Color color = chatWindow.color;
        color.a = 0;
        chatWindow.color = color;
        color = messageText.color;
        color.a = 0;
        messageText.color = color;
        nameText.color = color;
        nameText.text = "";
        messageText.text = "";
    }

    public IEnumerator Chat(ChatMessage chatMessage)
    {
        nameText.text = chatMessage.name;
        return ShowChat(chatMessage.message);
    }

    private IEnumerator ShowChat(string message)
    {
        foreach (char c in message)
        {
            messageText.text += c;
            yield return new WaitForSeconds(SHOW_CHAT_DELAY);
        }
    }
}
