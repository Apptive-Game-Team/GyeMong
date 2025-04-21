using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Game.Item;
using UnityEngine;
using TMPro;
using UI.mouse_input;

namespace System.Game.Inventory
{
    public class InventorySlot : SelectableUI, IMouseInputListener
    {
        public ItemInfo _item;
        [SerializeField] private Image itemImage;
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private TextMeshProUGUI itemCount;
        [SerializeField] private TextMeshProUGUI itemDescription;
        [SerializeField] private Image slotMask;
        public void AddItem(ItemInfo item, int count = 1)
        {
            _item = item;
            itemCount.text = count.ToString();
            itemImage.sprite = _item.Image;
            itemName.text = _item.ItemName;
            itemDescription.text = "";
        }
        public void UpdateSlot(ItemInfo item)
        {
            if (item == null)
            {
                _item = null;
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
                AddItem(item);
            }
        }
        public override void OnInteract()
        {
            itemDescription.text = _item.ItemDescription;
            StartCoroutine(FlashSlotMask());
        }
        public override void OnLongInteract()
        {
        }
        public void OnMouseInput(MouseInputState state, ISelectableUI ui)
        {
            //클릭 시 알파값1로 올리고 떼면0으로 돌아오게 하는 중간과정을 위해 롱클릭도 알파값1로 하면 click->Long_Click의 연속적 이동이 되나?
        }
        private IEnumerator FlashSlotMask()
        {
            Color originalColor = slotMask.color;
            slotMask.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
            yield return new WaitForSeconds(0.1f);
            slotMask.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
            yield return new WaitForSeconds(0.1f);
            slotMask.color = originalColor;
        }
    }
}