using System.Collections;
using System.Collections.Generic;
using System.Game.Item;
using UnityEngine;

namespace System.Game.Inventory
{
    public class InventoryUi : MonoBehaviour
    {
        [SerializeField] private List<InventorySlot> slots;

        private void OnEnable()
        {
            InventoryData.Instance.OnSlotUpdated += UpdateSlotUI;
            RedrawAll();
        }

        private void OnDisable()
        {
            InventoryData.Instance.OnSlotUpdated -= UpdateSlotUI;
        }

        private void UpdateSlotUI(int slotIndex, ItemInfo item)
        {
            if (slotIndex < 0 || slotIndex >= slots.Count) return;
            slots[slotIndex].UpdateSlot(item);
        }

        public void RedrawAll()
        {
            for (int i = 0; i < slots.Count; i++)
            {
                var item = InventoryData.Instance.GetItem(i);
                slots[i].UpdateSlot(item);
            }
        }
    }
}
