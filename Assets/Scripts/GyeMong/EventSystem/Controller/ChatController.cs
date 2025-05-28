using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GyeMong.EventSystem.Event.Chat;
using GyeMong.InputSystem;
using GyeMong.SoundSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GyeMong.EventSystem.Event.Chat.MultiChatMessageData;

namespace GyeMong.EventSystem.Controller
{
    public class ChatController : MonoBehaviour
    {
        private static Image chatWindow;
        private static Image backGround;
        private static Image characterImage;
        private static Image characterImage2;
        private static Image showImage;
        [SerializeField] private GameObject speechBubble;
        private const float CHAT_WINDOW_ALPHA = 0.7f;
        private const float SHOW_CHAT_DELAY = 0.1f;
        private static TMP_Text nameText;
        private static TMP_Text messageText;
        private static bool isWorking = false;

        private void Awake()
        {
            chatWindow = transform.Find("ChatWindow").GetComponent<Image>();
            nameText = chatWindow.transform.Find("NameArea").GetComponent<TMP_Text>();
            messageText = chatWindow.transform.Find("MessageArea").GetComponent<TMP_Text>();
            backGround = chatWindow.transform.Find("BackgroundArea").GetComponent <Image>();
            characterImage = chatWindow.transform.Find("CharacterImageArea").GetComponent<Image>();
            characterImage2 = chatWindow.transform.Find("CharacterImageArea2").GetComponent<Image>();
            showImage = chatWindow.transform.Find("ShowImageArea").GetComponent<Image>();
        }

        public static IEnumerator Open()
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

        public static void Close()
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

        public static IEnumerator MultipleChat(MultiChatMessageData.MultiChatMessage multiChatMessage, float autoSkipTime)
        {
            nameText.text = SetSpeakerName(multiChatMessage.speakerName);
            messageText.text = "";
            SetBackgroundImage(GetBackgroundImageSprite(multiChatMessage.backgroundImage));

            ChatSpeakerData speakerData = Resources.Load<ChatSpeakerData>("ScriptableObjects/Chat/ChatSpeakerData");

            var speakerInfo = speakerData.ChatSpeakers.Find(info => info.speakerType == multiChatMessage.speakerName);
            SetCharacterImage(speakerInfo.image, multiChatMessage.isLeft, multiChatMessage.isShowImage);

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

        private static IEnumerator ShowMultipleChat(string messages)
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

        public static void SetBackgroundImage(Sprite sprite)
        {
            if (backGround != null)
            {
                if (sprite != null)
                {
                    backGround.sprite = sprite;
                    backGround.color = Color.white;
                    backGround.enabled = true;
                }
                else
                {
                    backGround.sprite = null;
                    backGround.color = new Color(0, 0, 0, 0);
                    backGround.enabled = false;
                }
            }
        }
        public static void SetCharacterImage(Sprite sprite, bool isLeft, bool isShowImage)
        {
            if (isShowImage)
            {
                if (sprite != null)
                {
                    showImage.sprite = sprite;
                    showImage.color = Color.white;
                    showImage.enabled = true;

                    Vector3 originalPosition = showImage.rectTransform.localPosition;
                    Vector3 startPosition = originalPosition + new Vector3(0, 50f, 0);
                    showImage.rectTransform.localPosition = startPosition;

                    showImage.rectTransform.DOLocalMoveY(originalPosition.y, 0.4f).SetEase(Ease.OutCubic);
                }
                else
                {
                    showImage.sprite = null;
                    showImage.color = new Color(0, 0, 0, 0);
                    showImage.enabled = false;
                }
                return;
            }
            else if (isLeft)
            {
                if (sprite != null)
                {
                    characterImage.sprite = sprite;
                    characterImage.color = Color.white;
                    characterImage.enabled = true;
                }
                else
                {
                    characterImage.sprite = null;
                    characterImage.color = new Color(0, 0, 0, 0);
                    characterImage.enabled = false;
                }

                if (characterImage2.enabled && characterImage2.sprite != null)
                {
                    characterImage2.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                }
            }

            else
            {
                if (sprite != null)
                {
                    characterImage2.sprite = sprite;
                    characterImage2.color = Color.white;
                    characterImage2.enabled = true;
                }
                else
                {
                    characterImage2.sprite = null;
                    characterImage2.color = new Color(0, 0, 0, 0);
                    characterImage2.enabled = false;
                }

                if (characterImage.enabled && characterImage.sprite != null)
                {
                    characterImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                }
            }
        }

        public static String SetSpeakerName(ChatSpeakerType speakerName)
        {
            ChatSpeakerData speakerData = Resources.Load<ChatSpeakerData>("ScriptableObjects/Chat/ChatSpeakerData");
            
            foreach (var info in speakerData.ChatSpeakers)
            {
                if (info.speakerType == speakerName)
                {
                    return info.speakerName;
                }
            }

            return speakerName.ToString();
        }
        
        private static Sprite GetBackgroundImageSprite(BackgroundImage backgroundImage)
        {
            BackgroundImageData backgroundImageData = Resources.Load<BackgroundImageData>("ScriptableObjects/Chat/BackgroundImage");
            foreach (var imageInfo in backgroundImageData.backgroundImages)
            {
                if (imageInfo.backgroundImage == backgroundImage)
                {
                    return imageInfo.image;
                }
            }

            return null;
        }

        private static Sprite GetCharacterImageSprite(ChatSpeakerType speakerType)
        {
            ChatSpeakerData speakerData = Resources.Load<ChatSpeakerData>("ScriptableObjects/Chat/ChatSpeakerData");
            foreach (var imageInfo in speakerData.ChatSpeakers)
            {
                if (imageInfo.speakerType == speakerType)
                {
                    return imageInfo.image;
                }
            }

            return null;
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