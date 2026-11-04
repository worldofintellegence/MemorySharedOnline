using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NetcodePlus;

namespace SurvivalEngine
{

    /// <summary>
    /// Shows currently equipped items
    /// </summary>

    public class AbilityPanel : ItemSlotPanel
    {
        private static List<AbilityPanel> panel_list = new List<AbilityPanel>();

        // Remember to add move left and right for abilities equipped
        private int equip_slot = -1;    

        public void UpdateCurrentSlot()
        {

            ////////////////////////////////
            /// 
            /* for (int i = 0; i < slots.Length; i++)
            {

                EquipSlotUI current_slot = (EquipSlotUI)slots[i];
                if (current_slot.equip_slot == EquipSlot.Ability)
                {
                    if (i == slots.Length - 1)
                    {
                        equip_slot = 0;
                    }
                    else
                    {
                        equip_slot = i + 1;
                    }
                    current_slot = (EquipSlotUI)slots[equip_slot];
                    current_slot.equip_slot = EquipSlot.Ability;
                }
            } */
            /// 
            /// 
            /// 
            /// 
        }

        protected override void Awake()
        {
            ////////////////////////////////
            /// 
            /* if (slots.Length > 0)
            {
                equip_slot = 0;
                for (int i = 0; i < slots.Length; i++)
                {

                    EquipSlotUI current_slot = (EquipSlotUI)slots[i];
                    if (i == 0)
                        current_slot.equip_slot = EquipSlot.Ability;
                    else
                        current_slot.equip_slot = EquipSlot.None;

                }

            } */
            /// 
            /// 
            /// 
            /// 



            base.Awake();
            panel_list.Add(this);
            unfocus_when_out = true;

            Hide(true);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            panel_list.Remove(this);
        }

        protected override void Start()
        {
            base.Start();

        }

        public override void InitPanel()
        {
            base.InitPanel();

            if (!IsInventorySet())
            {
                PlayerCharacter player = GetPlayer();
                if (player != null)
                {
                    bool has_inventory = player.SaveData.HasEquipInventory();
                    if (has_inventory)
                    {
                        SetInventory(InventoryType.Abilities, player.EquipData.uid, player.player_id, player.EquipData.size);
                        SetPlayer(player);
                        Show(true);
                    }
                }
            }
        }

        protected override void RefreshPanel()
        {
            InventoryData inventory = GetInventory();

            if (inventory != null)
            {
                for (int i = 0; i < slots.Length; i++)
                {
                    EquipSlotUI slot = (EquipSlotUI)slots[i];
                    if (slot != null)
                    {
                        InventoryItemData invdata = inventory.GetInventoryItem((int)slot.equip_slot);
                        ItemData idata = ItemData.Get(invdata?.item_id);

                        if (invdata != null && idata != null)
                        {
                            slot.SetSlot(idata, invdata.quantity, selected_slot == slot.index || selected_right_slot == slot.index);
                            slot.SetDurability(idata.GetDurabilityPercent(invdata.durability), ShouldShowDurability(idata, invdata.durability));
                            slot.SetFilter(GetFilterLevel(idata, invdata.durability));
                        }
                        else
                        {
                            slot.SetSlot(null, 0, false);
                        }
                    }
                }
            }
        }

        public static AbilityPanel Get(int player_id)
        {
            foreach (AbilityPanel panel in panel_list)
            {
                PlayerCharacter player = panel.GetPlayer();
                if (player != null && player.PlayerID == player_id)
                    return panel;
            }
            return null;
        }

        public static AbilityPanel Get()
        {
            return Get(TheNetwork.Get().PlayerID);
        }

        public static new List<AbilityPanel> GetAll()
        {
            return panel_list;
        }
    }

}
