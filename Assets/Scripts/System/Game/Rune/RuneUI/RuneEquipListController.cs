using System.Game.Rune.RuneUI.RuneTreeSystem;
using System.Input;
using System.Input.Interface;
using Creature.Player;
using Creature.Player.Component;
using UnityEngine;

namespace System.Game.Rune.RuneUI
{
    public class RuneEquipListController : MonoBehaviour, IMouseInputListener
    {
        private RuneComponent _runeComponent;
    
        [SerializeField] RuneTreeNode runeIcon;
        [SerializeField] GameObject runeIcon_Empty;

        private void Start()
        {
            MouseInputManager.Instance.AddListener(this);
            _runeComponent = PlayerCharacter.Instance.GetComponent<RuneComponent>();
            UpdateRuneList();
        }

        private void UpdateRuneList()
        {
            foreach(Transform obj in transform)
            {
                Destroy(obj.gameObject);
            }
        
            int maxSlotNum = _runeComponent.MaxRuneEquipNum;
            int equippedSlotNum = _runeComponent.EquippedRuneList.Count;
            int emptySlotNum = maxSlotNum- equippedSlotNum;
            foreach(var runeData in _runeComponent.EquippedRuneList)
            {
                RuneUIObject runeUI = Instantiate(runeIcon, transform);
                runeUI.uiState = RuneUIState.EQUIPPED;
                runeIcon.Init(runeData);
            }
        
            for(int i = 0; i < emptySlotNum; i++)
            {
                Instantiate(runeIcon_Empty, transform);
            }
        }

        public void OnMouseInput(MouseInputState state, ISelectableUI ui)
        {
            if (state == MouseInputState.CLICKED && ui is RuneUIObject)
            {
                UpdateRuneList();
            }
        }
    }
}
