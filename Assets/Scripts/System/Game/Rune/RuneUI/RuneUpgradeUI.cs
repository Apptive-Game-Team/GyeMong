using playerCharacter;
using UI.mouse_input;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum RuneUpgradeState
{
    NONE,
    LOCKED,
    UNLOCKED,
    ACTIVATED,
}
public class RuneUpgradeUI : SelectableUI, IInteractionalUI, IMouseInputListener
{ 
    [SerializeField] RuneUpgrade upgradeData;
    [SerializeField] public RuneUpgradeState upgradeState;

    [SerializeField] Image uiImage;
    
    public int ID => upgradeData.id;

    public RuneUpgrade UpgradeData => upgradeData;
    
    public void Init(RuneUpgrade newData)
    {
        upgradeData = newData;
        uiImage.sprite = upgradeData.upgradeImage;
        if (!upgradeData.isUnlocked)
        {
            uiImage.color = Color.gray;
        }
        else
        {
            uiImage.color = Color.white;
        }
    }

    public override void OnInteract()
    {
        // PlayerCharacter.Instance.GetComponent<RuneComponent>().UpgradeRune();
    }

    public override void OnLongInteract()
    {
        //해금 부분
        if (!upgradeData.isUnlocked)
        {
            upgradeData.isUnlocked = true;
            Init(upgradeData);
        }
    }

    public void OnMouseInput(MouseInputState state, ISelectableUI ui)
    {
        if (state.Equals(MouseInputState.LONG_CLICKED))
        {
            ui.OnLongInteract();
            Init(upgradeData);
        }
    }
}
