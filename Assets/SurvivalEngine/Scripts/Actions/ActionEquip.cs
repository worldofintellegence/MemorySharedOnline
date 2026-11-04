using System.Collections;
using System.Collections.Generic;
using Suriyun.MCS.Example;
using UnityEngine;

namespace SurvivalEngine
{
    /// <summary>
    /// Use to equip/unequip equipment items
    /// </summary>
    
    [CreateAssetMenu(fileName = "Action", menuName = "SurvivalEngine/Actions/Equip", order = 50)]
    public class ActionEquip : SAction
    {

        public override void DoAction(PlayerCharacter character, InventorySlot slot)
        {
            ItemData item = slot.GetItem();
            InventoryData inventory = slot.inventory;

            if (item != null)
                if (item.type == ItemType.Equipment)
                {
                    if (inventory.type == InventoryType.Equipment)
                    {
                        EquipSlot eslot = (EquipSlot)slot.slot; //Enum 
                        character.Inventory.UnequipItem(eslot);
                    }
                    else
                    {
                        character.Inventory.EquipItem(inventory, slot.slot);
                    }
                }
                else if (item.type == ItemType.Shard)
                {
                    Debug.Log("Shard");
                    if (inventory.type == InventoryType.Abilities)
                    {
                        Debug.Log("Equip");
                        EquipSlot eslot = (EquipSlot)slot.slot;
                        character.Inventory.UnequipShard(eslot);
                    }
                    else
                    {
                        character.Inventory.EquipShard(inventory, slot.slot);
                    }
                    //PlayerCharacter.Get().GetComponent<Abilities>().skillSettings[0].skillPrefab = item.SkillPrefab;
                }
                //////////////////////////////////
            /// 
            /*
            else if (item.type == ItemType.Shard)
            {
                if (inventory.type == InventoryType.Abilities)
                {
                    EquipSlot eslot = (EquipSlot)slot.slot;
                    character.Inventory.UnequipItem(eslot);
                }
                else
                {
                    character.Inventory.EquipItem(inventory, slot.slot);
                }
            }
            */
        }

        public override bool CanDoAction(PlayerCharacter character, InventorySlot slot)
        {
            ItemData item = slot.GetItem();
            return item != null && (item.type == ItemType.Equipment || item.type == ItemType.Shard);
        }
    }

}