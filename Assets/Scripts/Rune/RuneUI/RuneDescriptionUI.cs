using TMPro;
using UI.mouse_input;
using UnityEngine;

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
    public void SetDescription(DescriptionSet description);
}

public class RuneDescriptionUI : MonoBehaviour, IDescriptionUI, IMouseInputListener
{

    [SerializeField] TextMeshProUGUI textTitle;
    [SerializeField] TextMeshProUGUI textDescription;

    private void Start()
    {
        MouseInputManager.Instance.AddListener(this);
    }

    public void SetDescription(DescriptionSet descriptionSet)
    {
        textTitle.text = descriptionSet.Title; 
        textDescription.text = descriptionSet.Description;
    }

    public void OnMouseInput(MouseInputState state, ISelectableUI ui)
    {
        if (state == MouseInputState.ENTERED)
        {
            if (ui is RuneUIObject)
            {
                RuneUIObject runeUI = (RuneUIObject)ui;
                SetDescription(runeUI.BuildDescriptionSet());
            }
        }
    }
}
