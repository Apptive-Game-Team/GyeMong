using System;
using System.Collections;
using DG.Tweening;
using GyeMong.EventSystem.Event.Chat;
using GyeMong.InputSystem;
using GyeMong.SoundSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GyeMong.EventSystem.Controller
{
    public class ChatController : MonoBehaviour
    {
        private static Image chatWindow;
        private static Image backGround;
        private static Image characterImage;
        private static Image characterImage2;
        private static Image chattingImage;
        [SerializeField] private GameObject speechBubble;
        private const float CHAT_WINDOW_ALPHA = 0.8f;
        private const float SHOW_CHAT_DELAY = 0.1f;
        private static TMP_Text nameText;
        private static TMP_Text messageText;
        private static bool isWorking = false;

        private void Awake()
        {
            chatWindow = transform.Find("ChatWindow").GetComponent<Image>();
            nameText = transform.Find("NameArea").GetComponent<TMP_Text>();
            messageText = transform.Find("MessageArea").GetComponent<TMP_Text>();
            backGround = transform.Find("BackgroundArea").GetComponent <Image>();
            characterImage = transform.Find("CharacterImageArea").GetComponent<Image>();
            characterImage2 = transform.Find("CharacterImageArea2").GetComponent<Image>();
            chattingImage = transform.Find("ChatImageArea").GetComponent<Image>();
        }

        public static IEnumerator Open()
        {
            yield return new WaitWhile(() => isWorking);
            SetCharacterImage(null, true);
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
            SetCharacterImage(null, true);
            isWorking = false;
        }

        public static IEnumerator MultipleChat(MultiChatMessageData.MultiChatMessage multiChatMessage, float autoSkipTime)
        {
            nameText.text = SetSpeakerName(multiChatMessage.speakerName);
            messageText.text = "";

            ChatSpeakerData speakerData = Resources.Load<ChatSpeakerData>("ScriptableObjects/Chat/ChatSpeakerData");
            var speakerInfo = speakerData.ChatSpeakers.Find(info => info.speakerType == multiChatMessage.speakerName);
            Sprite characterSprite = multiChatMessage.faceType == ChatSpeakerData.ChatSpeakerFace.None ? speakerInfo.image :
                speakerInfo.Faces.Find(info => info.faceType == multiChatMessage.faceType).faceImage;
            
            if (characterSprite == null)
            {
                characterSprite = speakerInfo.image;
            }
            
            SetCharacterImage(characterSprite, multiChatMessage.isLeft);

            string accumulatedText = "";

            foreach (string line in multiChatMessage.messages)
            {
                SoundObject _soundObject = Sound.Play("EFFECT_Keyboard_Sound", true);
                yield return ShowMultipleChat(line, accumulatedText);
                Sound.Stop(_soundObject);

                accumulatedText += line + "\n";

                float timer = Time.time;
                yield return new WaitUntil(() => (Time.time - timer) > autoSkipTime ||
                                                 InputManager.Instance.GetKeyDown(ActionCode.Interaction));
            }
        }

        private static IEnumerator ShowMultipleChat(string newLine, string prefix)
        {
            string currentText = "";
            foreach (char c in newLine)
            {
                if (InputManager.Instance.GetKeyDown(ActionCode.Interaction))
                {
                    currentText = newLine;
                    messageText.text = prefix + currentText;
                    yield return new WaitForSeconds(SHOW_CHAT_DELAY);
                    yield break;
                }
                currentText += c;
                messageText.text = prefix + currentText;
                yield return new WaitForSeconds(SHOW_CHAT_DELAY);
            }
        }

        public static void SetBackgroundImage(Sprite sprite)
        {
            if (backGround != null)
            {
                SetImage(backGround, sprite);
            }
            else SetImage(backGround, null);
        }
        public static void SetChatImage(Sprite sprite)
        {
            if (chattingImage == null) return;

            if (sprite != null)
            {
                chattingImage.sprite = sprite;
                chattingImage.color = new Color(1, 1, 1, 0);
                chattingImage.enabled = true;

                Vector3 startPos = chattingImage.rectTransform.localPosition + new Vector3(0, 50, 0);
                chattingImage.rectTransform.localPosition = startPos;

                chattingImage.rectTransform.DOLocalMoveY(startPos.y - 50, 0.5f).SetEase(Ease.OutCubic);
                chattingImage.DOFade(1f, 0.5f);
            }
            else
            {
                chattingImage.sprite = null;
                chattingImage.color = new Color(0, 0, 0, 0);
                chattingImage.enabled = false;
            }
        }
        public static void SetCharacterImage(Sprite sprite, bool isLeft)
        {
            if (sprite == null)
            {
                SetImage(characterImage, sprite);
                SetImage(characterImage2, sprite);
                return;
            }
            
            if (isLeft)
            {
                SetImage(characterImage, sprite);

                if (characterImage2.enabled && characterImage2.sprite != null)
                {
                    characterImage2.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                }
            }

            else
            {
                SetImage(characterImage2, sprite);

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
                    return info.speakerName + " : ";
                }
            }

            return speakerName.ToString();
        }
        
        public static Sprite GetBackgroundImageSprite(BackgroundImage backgroundImage)
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

        public static Sprite GetChatImageSprite(ChatImage chatImage)
        {
            ChatImageData chatImageData = Resources.Load<ChatImageData>("ScriptableObjects/Chat/ChatImageData");
            foreach (var imageInfo in chatImageData.chatImages)
            {
                if (imageInfo.chatImage == chatImage)
                {
                    return imageInfo.image;
                }
            }

            return null;
        }

        public static Sprite GetCharacterImageSprite(ChatSpeakerType speakerType)
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

        private static void SetImage(Image targetImage, Sprite sprite)
        {
            if (targetImage == null) return;

            if (sprite != null)
            {
                targetImage.sprite = sprite;
                targetImage.color = Color.white;
                targetImage.enabled = true;
            }
            else
            {
                targetImage.sprite = null;
                targetImage.color = new Color(0, 0, 0, 0);
                targetImage.enabled = false;
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