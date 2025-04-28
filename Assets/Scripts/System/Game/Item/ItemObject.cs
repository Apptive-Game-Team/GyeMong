using Creature.Attack.Component.Movement;
using Creature.Attack.Component.Sound;
using Map.Puzzle.TemplePuzzle;
using playerCharacter;
using System.Collections;
using System.Collections.Generic;
using System.Game.Inventory;
using UnityEngine;

namespace System.Game.Item
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
