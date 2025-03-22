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

[Serializable]
public struct MultiChatMessage
{
    public string name;
    public List<string> messages;
    public Nullable<float> chatDelay;
}

public class ChatController : MonoBehaviour
{

    private Image chatWindow;
    [SerializeField] private GameObject speechBubble;
    private const float CHAT_WINDOW_ALPHA = 0.7f;
    private const float SHOW_CHAT_DELAY = 0.1f;
    private TMP_Text nameText;
    private TMP_Text messageText;
    private bool isWorking = false;

    private void Awake()
    {
        chatWindow = transform.Find("ChatWindow").GetComponent<Image>();
        nameText = chatWindow.transform.Find("NameArea").GetComponent<TMP_Text>();
        messageText = chatWindow.transform.Find("MessageArea").GetComponent<TMP_Text>();
    }

    public IEnumerator Open()
    {
        yield return new WaitWhile(() => isWorking);
        isWorking = true;
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
        isWorking = false;
    }

    public IEnumerator Chat(ChatMessage chatMessage)
    {
        nameText.text = chatMessage.name;
        messageText.text = "";
        yield return ShowChat(chatMessage.message);
    }

    public IEnumerator MultipleChat(MultiChatMessage multiChatMessage)
    {
        nameText.text = multiChatMessage.name;
        messageText.text = "";

        foreach (string line in multiChatMessage.messages)
        {
            yield return ShowMultipleChat(line);
            messageText.text += "\n";
        }

        yield return new WaitForSeconds(multiChatMessage.chatDelay.GetValueOrDefault(0));
    }

    private IEnumerator ShowChat(string message)
    {
        foreach (char c in message)
        {
            messageText.text += c;
            yield return new WaitForSeconds(SHOW_CHAT_DELAY);
        }
    }

    private IEnumerator ShowMultipleChat(string messages)
    {
        foreach (char c in messages)
        {
            messageText.text += c;
            yield return new WaitForSeconds(SHOW_CHAT_DELAY);
        }
    }

    public IEnumerator ShowSpeechBubbleChat(GameObject NPC, string message, float destroyDelay)
    {
        GameObject speechBubbles = Instantiate(speechBubble, NPC.transform.position + new Vector3(0.51f,1.43f,0), Quaternion.identity);
        TextMeshPro messageText = speechBubbles.transform.Find("Message").GetComponent<TextMeshPro>();
        messageText.text = message;

        int order = NPC.GetComponent<SpriteRenderer>().sortingOrder;
        speechBubbles.GetComponent<SpriteRenderer>().sortingOrder = order + 1;
        messageText.GetComponent<MeshRenderer>().sortingOrder = order + 2;
        
        speechBubbles.SetActive(true);
        yield return new WaitForSeconds(destroyDelay);
        Destroy(speechBubbles);
    }
}
