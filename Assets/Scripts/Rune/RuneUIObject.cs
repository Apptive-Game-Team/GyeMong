using playerCharacter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IDescriptionalUI
{
    public DescriptionSet BuildDescriptionSet();
    public void SetDescription(IDescriptionUI descriptionUI);
}

public interface IInteractionalUI
{
    public void OnInteract();
}

public enum RuneUIState
{
    UNEQUIPPED,
    EQUIPPED,
}

public class RuneUIObject : SelectableUI, IDescriptionalUI, IInteractionalUI
{
    [SerializeField] RuneData runeData;
    [SerializeField] RuneUIState uiState;

    [SerializeField] Image uiImage;

    private void Update()
    {
    }

    public void Init(RuneData newData)
    {
        runeData = newData;
        uiImage.sprite = runeData.runeImage;
    }

    public void SetDescription(IDescriptionUI descriptionUI)
    {
        descriptionUI.SetDescription(BuildDescriptionSet());
    }

    public DescriptionSet BuildDescriptionSet()
    {
        return new DescriptionSet(runeData.name, runeData.description);
    }

    public override void OnInteract()
    {
        if(uiState == RuneUIState.UNEQUIPPED) 
        {
            PlayerCharacter.Instance.GetComponent<RuneComponent>().EquipRune(runeData);
        }
        else if (uiState == RuneUIState.EQUIPPED)
        {
            PlayerCharacter.Instance.GetComponent<RuneComponent>().UnequipRune(runeData);
        }

        RuneWindow.Instance.ReDrawUI();
        RuneWindow.Instance.SetSelectableList();
    }
}
