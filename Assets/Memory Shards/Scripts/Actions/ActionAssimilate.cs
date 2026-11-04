using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SurvivalEngine
{
    /// <summary>
    /// Absorb a shard
    /// </summary>
    

    [CreateAssetMenu(fileName = "Action", menuName = "SurvivalEngine/Actions/Assimilate", order = 50)]
    public class ActionAssimilate : SAction
    {

        public override void DoAction(PlayerCharacter character, InventorySlot slot)
        {
            InventoryData inventory = slot.inventory;
            character.Inventory.AbsorbItem(inventory, slot.slot);
        }

        public override bool CanDoAction(PlayerCharacter character, InventorySlot slot)
        {
            ItemData item = slot.GetItem();
            return item != null && item.type == ItemType.Shard;
        }
    }

}