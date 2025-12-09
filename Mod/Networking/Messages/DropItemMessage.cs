using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2DevTool.Networking.Messages
{
    /// <summary>
    /// Message for dropping items in front of a player
    /// </summary>
    public class DropItemMessage : RoR2DevToolMessageBase
    {
        private ItemIndex itemIndex;
        private int count;
        private Vector3 position;
        private Vector3 forward;

        public DropItemMessage()
        {
        }

        public DropItemMessage(ItemIndex itemIndex, int count, Vector3 position, Vector3 forward)
        {
            this.itemIndex = itemIndex;
            this.count = count;
            this.position = position;
            this.forward = forward;
        }

        public override void Serialize(NetworkWriter writer)
        {
            base.Serialize(writer);
            writer.Write(itemIndex);
            writer.WritePackedInt32(count);
            writer.Write(position);
            writer.Write(forward);
        }

        public override void Deserialize(NetworkReader reader)
        {
            base.Deserialize(reader);
            itemIndex = reader.ReadItemIndex();
            count = reader.ReadPackedInt32();
            position = reader.ReadVector3();
            forward = reader.ReadVector3();
        }

        public override void Handle()
        {
            base.Handle();

            // Calculate drop position (2 units in front of player, slightly above ground)
            Vector3 dropPosition = position + (forward * 2f) + (Vector3.up * 1f);

            // Create pickup droplet for each item
            for (int i = 0; i < count; i++)
            {
                // Slight offset for multiple items so they don't stack perfectly
                Vector3 offset = new Vector3(
                    Random.Range(-0.5f, 0.5f),
                    0,
                    Random.Range(-0.5f, 0.5f)
                );

                Vector3 finalPosition = dropPosition + offset;

                // Create the pickup droplet
                PickupDropletController.CreatePickupDroplet(
                    PickupCatalog.FindPickupIndex(itemIndex),
                    finalPosition,
                    Vector3.up * 10f // Give it some upward velocity
                );
            }
        }
    }
}
