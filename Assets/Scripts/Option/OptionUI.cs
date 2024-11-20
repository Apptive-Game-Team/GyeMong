using UnityEngine;
using UnityEngine.UI;

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
                OpenOrCloseOption();
            }
        }

        PauseOrResumeGame();
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
    }

    private void PauseOrResumeGame()
    {
        if (isOptionOpened) Time.timeScale = 0f;
        else Time.timeScale = 1f;
        InputManager.Instance.SetActionState(!isOptionOpened);
    }
}
