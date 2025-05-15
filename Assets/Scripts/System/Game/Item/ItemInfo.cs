using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace System.Game.Item
{
    [Serializable]
    public enum ItemType
    {
        IDLE = 0,
        QUEST = 1,
    }
    [CreateAssetMenu(fileName = "ItemInfo", menuName = "NewItem/ItemInfo")]
    public class ItemInfo : ScriptableObject
    {
        [SerializeField] private ItemType itemType;
        public ItemType Type
        {
            get
            {
                return itemType;
            }
        }
        [SerializeField] private string itemName;
        public string ItemName
        {
            get
            {
                return itemName;
            }
        }
        [SerializeField] private string itemDescription;
        public string ItemDescription
        {
            get
            {
                return itemDescription;
            }
        }
        [SerializeField] private Sprite itemImage;
        public Sprite Image
        {
            get
            {
                return itemImage;
            }
        }
    }
}