using TMPro;
using UI.mouse_input;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionSet
{
    string title;
    string description;

    public DescriptionSet(string title, string description)
    {
        this.title = title;
        this.description = description;
    }

    public string Title
    { get { return title; } }
    public string Description
    { get { return description; } }
}

public interface IDescriptionUI
{
    public void SetDescription(RuneData runeData);
}

public class RuneDescriptionUI : MonoBehaviour, IDescriptionUI, IMouseInputListener
{

    [SerializeField] TextMeshProUGUI textTitle;
    [SerializeField] TextMeshProUGUI textDescription;
    [SerializeField] Image runeImageUI;
    [SerializeField] Image runeOptionUI1;
    [SerializeField] Image runeOptionUI2;

    private void Start()
    {
        MouseInputManager.Instance.AddListener(this);
    }

    public void SetDescription(RuneData runeData)
    {
        textTitle.text = runeData.name; 
        textDescription.text = runeData.description;
        runeImageUI.sprite = runeData.runeImage;
        runeOptionUI1.sprite = runeData.availableOptions[0].optionImage;
        runeOptionUI2.sprite = runeData.availableOptions[0].optionImage;
    }

    public void OnMouseInput(MouseInputState state, ISelectableUI ui)
    {
        if (state == MouseInputState.ENTERED)
        {
            if (ui is RuneUIObject)
            {
                RuneUIObject runeUI = (RuneUIObject)ui;
                SetDescription(runeUI.RuneData);
            }
        }
    }
}
