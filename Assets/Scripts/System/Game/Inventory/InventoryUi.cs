using playerCharacter;
using System.Collections;
using System.Collections.Generic;
using System.Game.Item;
using UnityEngine;

namespace System.Game.Inventory
{
    public class InventoryUi : MonoBehaviour
    {
        [SerializeField] private List<InventorySlot> slots;
        private InventoryData _inventoryData;
        private void OnEnable()
        {
            _inventoryData = PlayerCharacter.Instance.GetComponent<InventoryData>();
            _inventoryData.OnSlotUpdated += UpdateSlotUI;
            RedrawAll();
        }
        private void OnDisable()
        {
            _inventoryData.OnSlotUpdated -= UpdateSlotUI;
        }
        private void UpdateSlotUI(int slotIndex, ItemInfo item, int count)
        {
            if (slotIndex < 0 || slotIndex >= slots.Count) return;
            slots[slotIndex].UpdateSlot(item, count);
        }
        public void RedrawAll()
        {
            for (int i = 0; i < slots.Count; i++)
            {
                var stack = _inventoryData.GetItem(i);
                if (stack.item != null)
                    slots[i].UpdateSlot(stack.item, stack.count);
                else
                    slots[i].UpdateSlot(null, 0);
            }
        }
    }
}
