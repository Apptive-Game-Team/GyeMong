using Gyemong.GameSystem.Creature.Player;
using Gyemong.GameSystem.Inventory;
using Gyemong.GameSystem.Object;
using UnityEngine;

namespace Gyemong.GameSystem.Item
{
    public class ItemObject : InteractableObject
    {
        [SerializeField] private ItemInfo _itemInfo;
        protected override void OnInteraction(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                PlayerCharacter.Instance.GetComponent<InventoryData>().AddItem(_itemInfo);
                Destroy(gameObject);
            }
        }
    }
}
