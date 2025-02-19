using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionUI : SingletonObject<OptionUI>
{
    private Image optionImage;
    private bool isOptionOpened = false;
    public bool isOptionUITop = true;
    [SerializeField] private SoundControlUI soundControlUI;
    public SoundControlUI GetSoundControlUI { get { return soundControlUI; } }
    [SerializeField] private SoundController soundController;
    public SoundController GetSoundController { get { return soundController; } }
    [SerializeField] private KeyButtonTexts keyButtonTexts;
    public KeyButtonTexts GetKeyButtonTexts { get { return keyButtonTexts;}}
    [SerializeField] private KeyMappingUI keyMappingUI;
    public KeyMappingUI GetKeyMappingUI { get { return keyMappingUI; } }

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
