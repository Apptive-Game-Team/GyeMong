using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    [SerializeField] GameObject optionExitButton;
    public void StartGame()
    {
        optionExitButton.SetActive(true);
        SceneManager.LoadScene("TutorialScene");
    }
}
