using RoR2;
using UnityEngine.Networking;

namespace RoR2DevTool.Networking.Messages
{
    /// <summary>
    /// Message for spawning items to a player's inventory
    /// </summary>
    public class SpawnItemMessage : RoR2DevToolMessageBase
    {
        private NetworkInstanceId inventoryId;
        private ItemIndex itemIndex;
        private int count;

        public SpawnItemMessage()
        {
        }

        public SpawnItemMessage(Inventory inventory, ItemIndex itemIndex, int count)
        {
            this.inventoryId = inventory.netId;
            this.itemIndex = itemIndex;
            this.count = count;
        }

        public override void Serialize(NetworkWriter writer)
        {
            base.Serialize(writer);
            writer.Write(inventoryId);
            writer.Write(itemIndex);
            writer.WritePackedInt32(count);
        }

        public override void Deserialize(NetworkReader reader)
        {
            base.Deserialize(reader);
            inventoryId = reader.ReadNetworkId();
            itemIndex = reader.ReadItemIndex();
            count = reader.ReadPackedInt32();
        }

        public override void Handle()
        {
            base.Handle();

            var obj = Util.FindNetworkObject(inventoryId);
            if (obj == null) return;

            var inventory = obj.GetComponent<Inventory>();
            if (inventory == null) return;

            if (count < 0)
            {
                // Remove items
                int currentCount = inventory.GetItemCount(itemIndex);
                int removeCount = UnityEngine.Mathf.Min(currentCount, -count);
                if (removeCount > 0)
                {
                    inventory.RemoveItem(itemIndex, removeCount);
                }
            }
            else if (count > 0)
            {
                // Add items
                inventory.GiveItem(itemIndex, count);
            }

            // Force sync
            if (inventory.GetComponent<CharacterMaster>() is CharacterMaster master)
            {
                master.inventory.SetDirtyBit(1U);
            }
        }
    }
}
