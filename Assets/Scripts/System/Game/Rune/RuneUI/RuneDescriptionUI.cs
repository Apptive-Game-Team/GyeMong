using Rune.RuneUI.Rework;
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
    [SerializeField] RuneUpgradeUI runeOptionUI1;
    [SerializeField] RuneUpgradeUI runeOptionUI2;

    private void Start()
    {
        MouseInputManager.Instance.AddListener(this);
    }

    public void SetDescription(RuneData runeData)
    {
        textTitle.text = runeData.name + " <" + runeData.cost + ">"; 
        textDescription.text = runeData.description;
        runeImageUI.sprite = runeData.runeImage;
        SetRuneUpgradeUI(runeData);
    }    
    public void SetDescription(RuneUpgrade upgradeData)
    {
        textTitle.text = upgradeData.name; 
        textDescription.text = upgradeData.description;
    }
    
    public void SetRuneUpgradeUI(RuneData runeData)
    {
        runeOptionUI1.Init(runeData.availableOptions[0]);
        runeOptionUI2.Init(runeData.availableOptions[1]);
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
            else if (ui is RuneUpgradeUI)
            {
                RuneUpgradeUI runeUpgradeUI = (RuneUpgradeUI)ui;
                SetDescription(runeUpgradeUI.UpgradeData);
            }
        }
    }
}
