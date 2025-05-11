using Gyemong.GameSystem.Creature.Player;
using Gyemong.GameSystem.Creature.Player.Component;
using Gyemong.InputSystem;
using Gyemong.InputSystem.Interface;
using Gyemong.StatusSystem.Gold;
using UnityEngine;
using UnityEngine.UI;

namespace Gyemong.GameSystem.Rune.RuneUI
{
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

    public class RuneUIObject : SelectableUI, IInteractionalUI, IMouseInputListener
    {
        [SerializeField] RuneData runeData;
        [SerializeField] public RuneUIState uiState;

        [SerializeField] Image uiImage;
    
        public int ID => runeData.id;
        public int ParentID => runeData.parentID;

        public RuneData RuneData => runeData;
    
        public void Init(RuneData newData)
        {
            runeData = newData;
            uiImage.sprite = runeData.runeImage;
            if (!runeData.isUnlocked)
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
            if(runeData.isUnlocked && !PlayerCharacter.Instance.GetComponent<RuneComponent>().isRune(runeData.id)) 
            {
                PlayerCharacter.Instance.GetComponent<RuneComponent>().EquipRune(runeData);
                uiState = RuneUIState.EQUIPPED;
            }
            else if (runeData.isUnlocked && PlayerCharacter.Instance.GetComponent<RuneComponent>().isRune(runeData.id))
            {
                PlayerCharacter.Instance.GetComponent<RuneComponent>().UnequipRune(runeData);
                uiState = RuneUIState.UNEQUIPPED;
            }

            // RuneWindow.Instance.Init();
        }

        public override void OnLongInteract()
        {
            //해금 부분
            if (!runeData.isUnlocked && GoldManager.Instance.SpendGold(runeData.cost))
            {
                runeData.isUnlocked = true;
                Init(runeData);
            }
        }

        public void OnMouseInput(MouseInputState state, ISelectableUI ui)
        {
            if (state.Equals(MouseInputState.LONG_CLICKED))
            {
                ui.OnLongInteract();
                Init(runeData);
            }
        }
    }
}