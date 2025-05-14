using System;
using System.Collections;
using System.Collections.Generic;
using GyeMong.InputSystem;
using GyeMong.SoundSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GyeMong.EventSystem.Controller
{
    [Serializable]
    public class MultiChatMessage
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

        public IEnumerator MultipleChat(MultiChatMessage multiChatMessage, float autoSkipTime)
        {
            nameText.text = multiChatMessage.name;
            messageText.text = "";

            foreach (string line in multiChatMessage.messages)
            {
                SoundObject _soundObject;
                _soundObject = Sound.Play("EFFECT_Keyboard_Sound", true);
                yield return ShowMultipleChat(line);
                Sound.Stop(_soundObject);
                messageText.text += "\n";

                float timer = Time.time;
                yield return new WaitUntil(() => (Time.time - timer) > autoSkipTime ||
                                                 InputManager.Instance.GetKeyDown(ActionCode.Interaction));
            }

        }

        private IEnumerator ShowMultipleChat(string messages)
        {
            foreach (char c in messages)
            {
                if (InputManager.Instance.GetKeyDown(ActionCode.Interaction))
                {
                    messageText.text = messages;
                    yield return new WaitForSeconds(SHOW_CHAT_DELAY);
                    break;
                }
                messageText.text += c;
                yield return new WaitForSeconds(SHOW_CHAT_DELAY);
            }
        }

        public IEnumerator ShowSpeechBubbleChat(GameObject NPC, string message, float destroyDelay)
        {
            GameObject speechBubbles = Instantiate(speechBubble, NPC.transform.position + new Vector3(0.51f, 1.43f, 0), Quaternion.identity);
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
}