using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionExitButton : MonoBehaviour
{
    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "TitleScene")
        {
            gameObject.SetActive(false);
        }
    }

    public void ExitButton()
    {
        gameObject.SetActive(false);
        OptionUI.Instance.OpenOrCloseOption();
        SceneManager.LoadScene("TitleScene");
    }
}
