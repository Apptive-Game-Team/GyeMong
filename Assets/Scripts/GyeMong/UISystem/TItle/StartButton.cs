using GyeMong.GameSystem.Map.Stage.Select;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using GyeMong.EventSystem.Event.Chat;
using GyeMong.EventSystem.Event;

namespace GyeMong.UISystem.TItle
{
    public class StartButton : MonoBehaviour
    {
        [SerializeField] private string bgmName;

        private GameObject optionExitButton;

        private void Start()
        {
            optionExitButton = GameObject.Find("OptionController").transform.GetChild(0).GetChild(0).Find("OptionExitButton").gameObject;
            StartCoroutine(TriggerEvents());
        }

        public void StartGame()
        {
            optionExitButton.SetActive(true);
            PlayerPrefs.SetInt(StageSelectPage.MAX_STAGE_ID_KEY, 0);
            SceneManager.LoadScene("Cinematic");
        }

        public IEnumerator Trigger()
        {
            return TriggerEvents();
        }

        private IEnumerator TriggerEvents()
        {
            yield return StartCoroutine((new BGMEvent(bgmName)).Execute());
        }
    }
}
