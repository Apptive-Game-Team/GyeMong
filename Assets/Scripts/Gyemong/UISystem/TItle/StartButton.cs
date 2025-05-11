using Gyemong.DataSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gyemong.UISystem.TItle
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
            DataManager.Instance.DeleteAllData();
            SceneManager.LoadScene("Cinematic");
        }
    }
}
