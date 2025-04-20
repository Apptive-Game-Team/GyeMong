using Creature.Attack.Component.Movement;
using Creature.Attack.Component.Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace System.Game.Item
{
    public class ItemObject : InteractableObject
    {
        [SerializeField] private ItemInfo _itemInfo;
        protected override void OnInteraction(Collider2D collision)
        {

        }
    }
}
