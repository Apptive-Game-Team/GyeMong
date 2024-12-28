using playerCharacter;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    private GameObject optionExitButton;

    private void Start()
    {
        optionExitButton = GameObject.Find("OptionController").transform.GetChild(0).GetChild(0).Find("OptionExitButton").gameObject;
    }

    public void StartGame()
    {
        PlayerCharacter.Instance.isStartButton = true;
        optionExitButton.SetActive(true);
        SceneManager.LoadScene("TutorialScene");
    }
}
