using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Game.Item;
using UnityEngine;
using TMPro;

namespace System.Game.Inventory
{
    public class InventorySlot : MonoBehaviour
    {
        public ItemInfo _item;
        [SerializeField] private Image itemImage;
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private TextMeshProUGUI itemCount;
        [SerializeField] private Image slotMask;
        public void AddItem(ItemInfo item, int count = 1)
        {
            _item = item;
            itemCount.text = count.ToString();
            itemImage.sprite = _item.Image;
            itemName.text = _item.ItemName;
        }
    }
}