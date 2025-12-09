using System.Collections.Generic;
using RoR2;
using UnityEngine.Networking;

namespace RoR2DevTool.Networking
{
    public static class NetworkExtensions
    {
        // Write a dictionary of item counts
        public static void WriteItemAmounts(this NetworkWriter writer, Dictionary<ItemDef, uint> itemCounts)
        {
            writer.WritePackedUInt32((uint)itemCounts.Count);
            foreach (var kvp in itemCounts)
            {
                writer.Write(kvp.Key.itemIndex);
                writer.WritePackedUInt32(kvp.Value);
            }
        }

        // Read a dictionary of item counts
        public static Dictionary<ItemDef, uint> ReadItemAmounts(this NetworkReader reader)
        {
            var count = reader.ReadPackedUInt32();
            var result = new Dictionary<ItemDef, uint>();
            
            for (uint i = 0; i < count; i++)
            {
                var itemIndex = reader.ReadItemIndex();
                var amount = reader.ReadPackedUInt32();
                var itemDef = ItemCatalog.GetItemDef(itemIndex);
                
                if (itemDef != null)
                {
                    result[itemDef] = amount;
                }
            }
            
            return result;
        }

        // Write ItemIndex
        public static void Write(this NetworkWriter writer, ItemIndex itemIndex)
        {
            writer.WritePackedUInt32((uint)itemIndex);
        }

        // Read ItemIndex
        public static ItemIndex ReadItemIndex(this NetworkReader reader)
        {
            return (ItemIndex)reader.ReadPackedUInt32();
        }

        // Write EquipmentIndex
        public static void Write(this NetworkWriter writer, EquipmentIndex equipmentIndex)
        {
            writer.WritePackedUInt32((uint)equipmentIndex);
        }

        // Read EquipmentIndex
        public static EquipmentIndex ReadEquipmentIndex(this NetworkReader reader)
        {
            return (EquipmentIndex)reader.ReadPackedUInt32();
        }
    }
}
