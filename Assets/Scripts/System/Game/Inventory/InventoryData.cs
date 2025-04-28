using System.Collections;
using System.Collections.Generic;
using System.Game.Item;
using UnityEngine;

namespace System.Game.Inventory
{
    [Serializable]
    public struct ItemStack
    {
        public ItemInfo item;
        public int count;

        public ItemStack(ItemInfo item, int count)
        {
            this.item = item;
            this.count = count;
        }
    }
    public class InventoryData : MonoBehaviour
    {
        [SerializeField] private List<InventorySlot> slots;
        Dictionary<int, ItemStack> items = new Dictionary<int, ItemStack>();

        public event Action<int, ItemInfo, int> OnSlotUpdated;

        public void SetItem(int slotIndex, ItemInfo item, int count)
        {
            items[slotIndex] = new ItemStack(item, count);
            OnSlotUpdated?.Invoke(slotIndex, item, count);
        }
        public ItemStack GetItem(int slotIndex)
        {
            return items.TryGetValue(slotIndex, out var stack) ? stack : default;
        }
        public bool AddItem(ItemInfo newItem)
        {
            int sameItemSlot = FindSameItemSlot(newItem);
            if (sameItemSlot != -1)
            {
                var stack = items[sameItemSlot];
                stack.count++;
                items[sameItemSlot] = stack;
                OnSlotUpdated?.Invoke(sameItemSlot, stack.item, stack.count);
                return true;
            }

            int emptySlot = FindFirstEmptySlot();
            if (emptySlot == -1)
                return false;

            SetItem(emptySlot, newItem, 1);
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
        public int FindSameItemSlot(ItemInfo newItem)
        {
            foreach (var item in items)
            {
                if (item.Value.item == newItem)
                    return item.Key;
            }
            return -1;
        }
    }
}
