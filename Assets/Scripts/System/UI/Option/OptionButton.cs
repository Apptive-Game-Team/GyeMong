using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace UI.Option
{
    public enum OptionButtonType
    {
        Resume,
        SoundControl,
        KeyMapping,
        OptionExit,
    }

    public class OptionButton : MonoBehaviour
    {
        [SerializeField] Button[] buttons;
        private Dictionary<string, OptionButtonType> buttonMappings  = new()
        {
            { "ResumeButton" , OptionButtonType.Resume },
            { "SoundControlButton" , OptionButtonType.SoundControl },
            { "KeyMappingButton" , OptionButtonType.KeyMapping },
            { "OptionExitButton" , OptionButtonType.OptionExit },
        };
        [SerializeField] private GameObject optionExitButton;

        private void Start() 
        {
            foreach (Button button in buttons)
            {
                if (buttonMappings.TryGetValue(button.name, out OptionButtonType buttonType))
                {
                    button.onClick.AddListener(() => OnButtonClick(buttonType));
                }
                else
                {
                    Debug.LogWarning($"[{button.name}] 버튼에 대한 처리 없음");
                }
            }
        }

        private void OnButtonClick(OptionButtonType buttonType)
        {
            switch (buttonType)
            {
                case OptionButtonType.Resume:
                    Resume();
                    break;
                case OptionButtonType.SoundControl:
                    SoundControl();
                    break;
                case OptionButtonType.KeyMapping:
                    KeyMapping();
                    break;
                case OptionButtonType.OptionExit:
                    OptionExit();
                    break;
            }
        }

        private void Resume()
        {
            OptionUI.Instance.OpenOrCloseOption();
        }

        private void SoundControl()
        {
            OptionUI.Instance.isOptionUITop = false;
            OptionUI.Instance.GetSoundControlUI.OpenSoundControlImage();
        }

        private void KeyMapping()
        {
            OptionUI.Instance.isOptionUITop = false;
            OptionUI.Instance.GetKeyButtonTexts.UpdateKeyText();
            OptionUI.Instance.GetKeyMappingUI.OpenKeyMappingUI();
        }

        private void OptionExit()
        {
            optionExitButton.SetActive(false);
            OptionUI.Instance.OpenOrCloseOption();
            Destroy(playerCharacter.PlayerCharacter.Instance.gameObject);
            SceneManager.LoadScene("TitleScene");
        }
    }
}


