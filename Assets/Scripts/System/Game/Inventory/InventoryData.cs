using System.Collections;
using System.Collections.Generic;
using System.Game.Item;
using UnityEngine;

namespace System.Game.Inventory
{
    public class InventoryData : MonoBehaviour
    {
        [SerializeField] private List<InventorySlot> slots;
        Dictionary<int, ItemInfo> items = new Dictionary<int, ItemInfo>();
        public event Action<int, ItemInfo> OnSlotUpdated;
        public void SetItem(int slotIndex, ItemInfo item)
        {
            items[slotIndex] = item;
            OnSlotUpdated?.Invoke(slotIndex, item);
        }
        public ItemInfo GetItem(int slotIndex)
        {
            return items.TryGetValue(slotIndex, out var item) ? item : null;
        }
        public bool AddItem(ItemInfo newItem)
        {
            int emptySlot = FindFirstEmptySlot();
            if (emptySlot == -1)
                return false;

            SetItem(emptySlot, newItem);
            return true;
        }
        public int FindFirstEmptySlot()
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (!items.ContainsKey(i))
                    return i;
            }
            return -1;
        }
    }
}
