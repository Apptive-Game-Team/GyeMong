using UnityEngine;
using UnityEngine.UI;

public class Option : SingletonObject<Option>
{
    private Image optionImage;
    private bool isOptionOpened = false;

    private void Start()
    {
        FindOptionImage();
        optionImage.gameObject.SetActive(isOptionOpened);
    }

    private void Update()
    {
        //if (InputManager.Instance.GetKeyDown(ActionCode.Option))
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenOrCloseOption();
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
        
    }
}
