using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionUI : SingletonObject<OptionUI>
{
    private Image optionImage;
    private bool isOptionOpened = false;
    public bool isOptionUITop = true;

    private void Start()
    {
        FindOptionImage();
        optionImage.gameObject.SetActive(isOptionOpened);
    }

    private void Update()
    {
        //if (InputManager.Instance.GetKeyDown(ActionCode.Option))
        if (isOptionUITop)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!(SceneManager.GetActiveScene().name == "TitleScene")) OpenOrCloseOption();
            }
        }
    }

    private void FindOptionImage()
    {
        Transform optionImageTransform = transform.Find("Canvas/OptionUI");
        optionImage = optionImageTransform.GetComponent<Image>();
    }


    public void OpenOrCloseOption()
    {   
        isOptionOpened = !isOptionOpened;
        optionImage.gameObject.SetActive(isOptionOpened);
        
        PauseOrResumeGame();
    }

    private void PauseOrResumeGame()
    {
        if (isOptionOpened) Time.timeScale = 0f;
        else Time.timeScale = 1f;
        InputManager.Instance.SetActionState(!isOptionOpened);
    }
}
