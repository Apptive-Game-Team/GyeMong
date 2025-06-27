using GyeMong.GameSystem.Map.Stage.Select;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GyeMong.UISystem.TItle
{
    public class StartButton : MonoBehaviour
    {
        private GameObject optionExitButton;

        private void Start()
        {
            optionExitButton = GameObject.Find("OptionController").transform.GetChild(0).GetChild(0).Find("OptionExitButton").gameObject;
        }

        public void StartGame()
        {
            optionExitButton.SetActive(true);
            PlayerPrefs.SetInt(StageSelectPage.MAX_STAGE_ID_KEY, 0);
            SceneManager.LoadScene("Cinematic");
        }
    }
}
