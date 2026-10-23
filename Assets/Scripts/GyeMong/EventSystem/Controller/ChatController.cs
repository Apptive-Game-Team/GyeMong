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
using GyeMong.UISystem;

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
        private const float CHAT_WINDOW_ALPHA = 1f;
        private const float SHOW_CHAT_DELAY = 0.1f;
        private static TMP_Text nameText;
        private static TMP_Text messageText;
        private static bool isWorking = false;
        private static Vector2 namedMessagePosition;
        private static Vector2 namedMessageSize;
        private static ChatSpeakerType activeSpeaker;
        private static string activeLine;
        private static readonly List<string> previousLines = new List<string>();

        private void OnEnable() { GameLocalization.Changed += RefreshLanguage; }
        private void OnDisable() { GameLocalization.Changed -= RefreshLanguage; StopDialogueSounds(); }

        private static readonly List<SoundObject> dialogueSounds = new List<SoundObject>();

        public static SoundObject PlayDialogueSound(string id)
        {
            var sound = Sound.Play(id, true);
            dialogueSounds.Add(sound);
            return sound;
        }

        public static void StopDialogueSound(SoundObject sound)
        {
            if (sound != null) Sound.Stop(sound);
            dialogueSounds.Remove(sound);
        }

        public static void StopDialogueSounds()
        {
            foreach (var sound in dialogueSounds)
                if (sound != null) Sound.Stop(sound);
            dialogueSounds.Clear();
        }

        private static void RefreshLanguage()
        {
            if (nameText == null || messageText == null || activeLine == null) return;
            SetSpeaker(activeSpeaker);
            string line = GameLocalization.Text(activeLine);
            string prefix = string.Join("\n", previousLines.ConvertAll(GameLocalization.Text));
            if (prefix.Length > 0) prefix += "\n";
            if (messageText.GetPreferredValues(prefix + line, messageText.rectTransform.rect.width, 0).y > messageText.rectTransform.rect.height)
            {
                previousLines.Clear();
                prefix = string.Empty;
            }
            messageText.text = prefix + line;
            messageText.ForceMeshUpdate();
        }

        private void Awake()
        {
            isWorking = false;
            activeLine = null;
            previousLines.Clear();
            chatWindow = transform.Find("ChatWindow").GetComponent<Image>();
            nameText = transform.Find("NameArea").GetComponent<TMP_Text>();
            messageText = transform.Find("MessageArea").GetComponent<TMP_Text>();
            backGround = transform.Find("BackgroundArea").GetComponent <Image>();
            characterImage = transform.Find("CharacterImageArea").GetComponent<Image>();
            characterImage2 = transform.Find("CharacterImageArea2").GetComponent<Image>();
            chattingImage = transform.Find("ChatImageArea").GetComponent<Image>();
            namedMessagePosition = messageText.rectTransform.anchoredPosition;
            namedMessageSize = messageText.rectTransform.sizeDelta;
            Close();
            backGround.color = new Color(1, 1, 1, 0);
            chattingImage.color = new Color(1, 1, 1, 0);
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
            activeLine = null;
            previousLines.Clear();
            messageText.maxVisibleCharacters = int.MaxValue;
            SetCharacterImage(null, true);
            isWorking = false;
        }

        public static IEnumerator MultipleChat(MultiChatMessageData.MultiChatMessage multiChatMessage, float autoSkipTime)
        {
            SetSpeaker(multiChatMessage.speakerName);
            messageText.text = "";

            ChatSpeakerData speakerData = Resources.Load<ChatSpeakerData>("ScriptableObjects/Chat/ChatSpeakerData");
            var speakerInfo = speakerData != null ? speakerData.ChatSpeakers.Find(info => info.speakerType == multiChatMessage.speakerName) : default;
            Sprite characterSprite = speakerInfo.image;
            if (multiChatMessage.speakerName != ChatSpeakerType.None &&
                multiChatMessage.faceType != ChatSpeakerData.ChatSpeakerFace.None && speakerInfo.Faces != null)
                characterSprite = speakerInfo.Faces.Find(info => info.faceType == multiChatMessage.faceType).faceImage;
            
            if (characterSprite == null)
            {
                characterSprite = speakerInfo.image;
            }
            
            SetCharacterImage(multiChatMessage.speakerName == ChatSpeakerType.None ? null : characterSprite, multiChatMessage.isLeft);

            previousLines.Clear();

            foreach (string source in multiChatMessage.messages)
            {
                SoundObject _soundObject = PlayDialogueSound("EFFECT_Keyboard_Sound");
                yield return ShowMultipleChat(source);
                StopDialogueSound(_soundObject);

                float timer = Time.time;
                int openedFrame = Time.frameCount;
                yield return new WaitUntil(() => InputManager.Instance.ConsumeAdvanceInput(openedFrame) ||
                                                 (Time.time - timer) > autoSkipTime);
                previousLines.Add(source);
            }
        }

        private static IEnumerator ShowMultipleChat(string source)
        {
            activeLine = source;
            RefreshLanguage();
            int prefixLength = messageText.text.Length - GameLocalization.Text(source).Length;
            messageText.maxVisibleCharacters = prefixLength;
            int openedFrame = Time.frameCount;
            float nextCharacterTime = Time.time;
            while (messageText.maxVisibleCharacters < messageText.textInfo.characterCount)
            {
                if (InputManager.Instance.ConsumeAdvanceInput(openedFrame))
                {
                    messageText.maxVisibleCharacters = int.MaxValue;
                    yield break;
                }
                if (Time.time >= nextCharacterTime)
                {
                    messageText.maxVisibleCharacters++;
                    nextCharacterTime = Time.time + SHOW_CHAT_DELAY;
                }
                yield return null;
            }
            messageText.maxVisibleCharacters = int.MaxValue;
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
            // None is intentionally absent from the speaker catalogue.
            if (speakerName == ChatSpeakerType.None) return string.Empty;
            ChatSpeakerData speakerData = Resources.Load<ChatSpeakerData>("ScriptableObjects/Chat/ChatSpeakerData");
            if (speakerData == null) return string.Empty;
            
            foreach (var info in speakerData.ChatSpeakers)
            {
                if (info.speakerType == speakerName)
                {
                    return string.IsNullOrWhiteSpace(info.speakerName) ? string.Empty : GameLocalization.Text(info.speakerName) + " :";
                }
            }

            return string.Empty;
        }

        public static void SetSpeaker(ChatSpeakerType speaker)
        {
            activeSpeaker = speaker;
            nameText.text = SetSpeakerName(speaker);
            bool hasSpeaker = !string.IsNullOrWhiteSpace(nameText.text);
            // Use the same readable size; reserve width instead of shrinking long names.
            nameText.enableAutoSizing = false;
            messageText.enableAutoSizing = false;
            nameText.fontSize = messageText.fontSize = 32;
            nameText.enableWordWrapping = false;
            nameText.enabled = hasSpeaker;
            // Restore the authored geometry every time; narration must not accumulate offsets.
            messageText.rectTransform.anchoredPosition = namedMessagePosition;
            messageText.rectTransform.sizeDelta = namedMessageSize;
            float nameWidth = hasSpeaker ? nameText.GetPreferredValues(nameText.text).x : 0;
            nameText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, nameWidth);
            if (hasSpeaker)
            {
                float shift = nameText.rectTransform.anchoredPosition.x + nameWidth + 24 - namedMessagePosition.x;
                messageText.rectTransform.anchoredPosition += new Vector2(shift, 0);
                messageText.rectTransform.sizeDelta -= new Vector2(shift, 0);
            }
            if (!hasSpeaker)
            {
                float reclaimedWidth = namedMessagePosition.x - nameText.rectTransform.anchoredPosition.x;
                messageText.rectTransform.anchoredPosition -= new Vector2(reclaimedWidth, 0);
                messageText.rectTransform.sizeDelta += new Vector2(reclaimedWidth, 0);
            }
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
            LocalizedGameText.Bind(messageText, message);

            int order = NPC.GetComponent<SpriteRenderer>().sortingOrder;
            speechBubbles.GetComponent<SpriteRenderer>().sortingOrder = order + 1;
            messageText.GetComponent<MeshRenderer>().sortingOrder = order + 2;

            speechBubbles.SetActive(true);
            yield return new WaitForSeconds(destroyDelay);
            Destroy(speechBubbles);
        }
    }
}
