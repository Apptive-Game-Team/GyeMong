using UnityEngine;
using UnityEngine.UI;

public class Option : MonoBehaviour
{
    private Image optionImage;
    private bool isOptionOpened = false;

    private void Awake()
    {
        FindOptionImage();
        optionImage.gameObject.SetActive(isOptionOpened);
    }

    private void Update()
    {
        OpenOrCloseOption();
    }

    private void FindOptionImage()
    {
        Transform optionImageTransform = transform.Find("Canvas/OptionUI");
        optionImage = optionImageTransform.GetComponent<Image>();
    }


    private void OpenOrCloseOption()
    {   
        //if (InputManager.Instance.GetKeyDown(ActionCode.Option))
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isOptionOpened = !isOptionOpened;
            optionImage.gameObject.SetActive(isOptionOpened);
        }
    }
}
