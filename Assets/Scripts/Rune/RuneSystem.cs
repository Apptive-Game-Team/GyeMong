using playerCharacter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace runeSystem
{
    public class RuneSystem : MonoBehaviour
    {
        public GameObject runeCanvas;
        public PlayerCharacter playerCharacter;

        public GameObject springRune;
        private Toggle[] springToggles;

        private bool isRuneActive = false;

        private int selectedRuneIndex = 0;
        private int previousSelectedRuneIndex = -1;

        public Color selectedColor = Color.blue;
        public Color isOnColor = Color.green;
        public Color unacquiredColor = Color.black;

        private Sprite[] originalSprites;
        private Color[] originalBackgroundColors;

        private bool[] runeAcquiredStatus;

        void Start()
        {
            runeCanvas.SetActive(false);

            springToggles = springRune.GetComponentsInChildren<Toggle>();
            runeAcquiredStatus = new bool[springToggles.Length];

            originalSprites = new Sprite[springToggles.Length];
            originalBackgroundColors = new Color[springToggles.Length];
            for (int i = 0; i < springToggles.Length; i++)
            {
                Image toggleImage = springToggles[i].GetComponentInChildren<Image>();
                if (toggleImage != null)
                {
                    originalSprites[i] = toggleImage.sprite;
                    originalBackgroundColors[i] = toggleImage.color;

                    toggleImage.color = unacquiredColor;
                }
            }

            foreach (var toggle in springToggles)
            {
                toggle.onValueChanged.AddListener(delegate { OnRuneToggleChanged(toggle); });
            }

            UpdateSelectedRune();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ToggleRunePage();
            }

            if (isRuneActive)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    NavigateToggles(true);
                }

                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    NavigateToggles(false);
                }

                if (Input.GetKeyDown(KeyCode.Z))
                {
                    ToggleSelectedRune();
                }
            }
        }

        private void ToggleRunePage()
        {
            isRuneActive = !isRuneActive;
            runeCanvas.SetActive(isRuneActive);

            if (isRuneActive)
            {
                Time.timeScale = 0f;
                UpdateSelectedRune();
            }
            else
            {
                Time.timeScale = 1f;
            }

        }

        private void NavigateToggles(bool isLeft)
        {
            previousSelectedRuneIndex = selectedRuneIndex;

            if (isLeft)
            {
                selectedRuneIndex = selectedRuneIndex - 1;
            }
            else
            {
                selectedRuneIndex = selectedRuneIndex + 1;
            }

            if (selectedRuneIndex < 0)
            {
                selectedRuneIndex = springToggles.Length - 1;
            }
            else if (selectedRuneIndex >= springToggles.Length)
            {
                selectedRuneIndex = 0;
            }

            UpdateSelectedRune();
        }

        private void UpdateSelectedRune()
        {

            if (previousSelectedRuneIndex != -1)
            {
                Transform previousBackground = springToggles[previousSelectedRuneIndex].transform.Find("Background");
                if (previousBackground != null)
                {
                    Image previousBackgroundImage = previousBackground.GetComponent<Image>();
                    if (previousBackgroundImage != null)
                    {
                        if (springToggles[previousSelectedRuneIndex].isOn)
                        {
                            previousBackgroundImage.color = isOnColor;
                        }
                        else
                        {
                            if (!runeAcquiredStatus[previousSelectedRuneIndex])
                            {
                                previousBackgroundImage.color = unacquiredColor;
                            }
                            else
                            {
                                previousBackgroundImage.sprite = originalSprites[previousSelectedRuneIndex];
                                previousBackgroundImage.color = originalBackgroundColors[previousSelectedRuneIndex];
                            }
                        }
                    }
                }
            }

            Transform selectedBackground = springToggles[selectedRuneIndex].transform.Find("Background");
            if (selectedBackground != null)
            {
                Image selectedBackgroundImage = selectedBackground.GetComponent<Image>();
                if (selectedBackgroundImage != null)
                {
                    selectedBackgroundImage.color = selectedColor;
                }
            }
        }

        private void ToggleSelectedRune()
        {
            if (!runeAcquiredStatus[selectedRuneIndex])
            {
                return;
            }

            springToggles[selectedRuneIndex].isOn = !springToggles[selectedRuneIndex].isOn;
        }

        private void OnRuneToggleChanged(Toggle toggle)
        {
            int runeIndex = System.Array.IndexOf(springToggles, toggle);

            if (toggle.isOn)
            {
                ActivateRune(runeIndex);
                SetBackgroundColor(runeIndex, isOnColor);
            }
            else
            {
                DeactivateRune(runeIndex);
                SetBackgroundColor(runeIndex, originalBackgroundColors[runeIndex]);
            }
        }

        public void ActivateRune(int runeIndex)
        {
            if (!runeAcquiredStatus[runeIndex])
            {
                return;
            }

            switch (runeIndex)
            {
                case 0:
                    playerCharacter.moveSpeed += 1.0f;
                    break;
                case 1:
                    playerCharacter.moveSpeed += 2.0f;
                    break;
                case 2:
                    playerCharacter.moveSpeed += 3.0f;
                    break;
                case 3:
                    playerCharacter.moveSpeed += 4.0f;
                    break;
                case 4:
                    playerCharacter.moveSpeed += 5.0f;
                    break;
            }
        }

        public void DeactivateRune(int runeIndex)
        {
            if (!runeAcquiredStatus[runeIndex])
            {
                return;
            }

            switch (runeIndex)
            {
                case 0:
                    playerCharacter.moveSpeed -= 1.0f;
                    break;
                case 1:
                    playerCharacter.moveSpeed -= 2.0f;
                    break;
                case 2:
                    playerCharacter.moveSpeed -= 3.0f;
                    break;
                case 3:
                    playerCharacter.moveSpeed -= 4.0f;
                    break;
                case 4:
                    playerCharacter.moveSpeed -= 5.0f;
                    break;
            }
        }

        private void SetBackgroundColor(int runeIndex, Color color)
        {
            Transform background = springToggles[runeIndex].transform.Find("Background");
            if (background != null)
            {
                Image backgroundImage = background.GetComponent<Image>();
                if (backgroundImage != null)
                {
                    backgroundImage.color = color;
                }
            }
        }

        public void AcquireRune(int runeIndex)
        {
            runeAcquiredStatus[runeIndex] = true;

            Image toggleImage = springToggles[runeIndex].GetComponentInChildren<Image>();
            if (toggleImage != null)
            {
                toggleImage.color = originalBackgroundColors[runeIndex];
            }

            UpdateSelectedRune();
        }

    }
}
