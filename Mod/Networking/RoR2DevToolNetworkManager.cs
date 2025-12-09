using System;
using System.Linq;
using RoR2;
using RoR2.Networking;
using UnityEngine.Networking;

namespace RoR2DevTool.Networking
{
    public static class RoR2DevToolNetworkManager
    {
        public static Type[] RegisteredMessages;
        private const short MESSAGE_ID = 2005; // Using 2005 to avoid conflicts with other mods

        public static void Initialize()
        {
            NetworkManagerSystem.onStartServerGlobal += RegisterMessages;
            NetworkManagerSystem.onStartClientGlobal += RegisterMessages;

            // Find all message types that inherit from RoR2DevToolMessageBase
            RegisteredMessages = typeof(RoR2DevToolMessageBase).Assembly.GetTypes()
                .Where(x => typeof(RoR2DevToolMessageBase).IsAssignableFrom(x) && 
                           x != typeof(RoR2DevToolMessageBase) && 
                           !x.IsAbstract)
                .ToArray();
        }

        private static void RegisterMessages()
        {
            NetworkServer.RegisterHandler(MESSAGE_ID, HandleMessage);
        }

        public static void RegisterMessages(NetworkClient client)
        {
            client.RegisterHandler(MESSAGE_ID, HandleMessage);
        }

        private static void HandleMessage(NetworkMessage netmsg)
        {
            var message = netmsg.ReadMessage<RoR2DevToolMessage>();
            
            // Set the connection for broadcast messages
            if (message.message is BroadcastMessage mess)
            {
                mess.fromConnection = netmsg.conn;
            }
            
            message.message.Handle();
        }

        public static void SendDevToolMessage<T>(this NetworkConnection connection, T message) where T : RoR2DevToolMessageBase
        {
            var mes = new RoR2DevToolMessage(message);
            connection.Send(MESSAGE_ID, mes);
        }
    }

    public class RoR2DevToolMessageBase : MessageBase
    {
        // Base class for all RoR2DevTool network messages
        public virtual void Handle()
        {
        }

        public void SendToServer()
        {
            if (!NetworkServer.active)
                ClientScene.readyConnection.SendDevToolMessage(this);
            else
                Handle();
        }

        public void SendToEveryone()
        {
            Handle();
            new BroadcastMessage(this).SendToServer();
        }

        public void SendToAuthority(NetworkIdentity identity)
        {
            if (!Util.HasEffectiveAuthority(identity) && NetworkServer.active)
                identity.clientAuthorityOwner.SendDevToolMessage(this);
            else if (!NetworkServer.active)
                new NewAuthMessage(identity, this).SendToServer();
            else
                Handle();
        }

        public void SendToAuthority(NetworkUser user)
        {
            SendToAuthority(user.netIdentity);
        }

        public void SendToAuthority(CharacterMaster master)
        {
            SendToAuthority(master.networkIdentity);
        }

        public void SendToAuthority(CharacterBody body)
        {
            SendToAuthority(body.networkIdentity);
        }
    }

    public class BroadcastMessage : RoR2DevToolMessageBase
    {
        public NetworkConnection fromConnection;
        private RoR2DevToolMessageBase message;

        public BroadcastMessage()
        {
        }

        public BroadcastMessage(RoR2DevToolMessageBase messageBase)
        {
            message = messageBase;
        }

        public override void Handle()
        {
            base.Handle();
            
            foreach (var connection in NetworkServer.connections)
            {
                if (connection == fromConnection) continue;
                if (!connection.isConnected) continue;
                connection.SendDevToolMessage(message);
            }
            
            message.Handle();
        }

        public override void Deserialize(NetworkReader reader)
        {
            base.Deserialize(reader);
            message = reader.ReadMessage<RoR2DevToolMessage>().message;
        }

        public override void Serialize(NetworkWriter writer)
        {
            base.Serialize(writer);
            writer.Write(new RoR2DevToolMessage(message));
        }
    }

    public class NewAuthMessage : RoR2DevToolMessageBase
    {
        private RoR2DevToolMessageBase message;
        private NetworkIdentity target;

        public NewAuthMessage()
        {
        }

        public NewAuthMessage(NetworkIdentity identity, RoR2DevToolMessageBase messageBase)
        {
            target = identity;
            message = messageBase;
        }

        public override void Handle()
        {
            base.Handle();
            message.SendToAuthority(target);
        }

        public override void Deserialize(NetworkReader reader)
        {
            base.Deserialize(reader);
            var obj = Util.FindNetworkObject(reader.ReadNetworkId());
            if (obj)
                target = obj.GetComponent<NetworkIdentity>();
            message = reader.ReadMessage<RoR2DevToolMessage>().message;
        }

        public override void Serialize(NetworkWriter writer)
        {
            base.Serialize(writer);
            writer.Write(target.netId);
            writer.Write(new RoR2DevToolMessage(message));
        }
    }

    internal class RoR2DevToolMessage : MessageBase
    {
        public RoR2DevToolMessageBase message;
        public uint Type;

        public RoR2DevToolMessage()
        {
        }

        public RoR2DevToolMessage(RoR2DevToolMessageBase messageBase)
        {
            message = messageBase;
            Type = (uint)Array.IndexOf(RoR2DevToolNetworkManager.RegisteredMessages, message.GetType());
        }

        public override void Serialize(NetworkWriter writer)
        {
            base.Serialize(writer);
            writer.WritePackedUInt32(Type);
            writer.Write(message);
        }

        public override void Deserialize(NetworkReader reader)
        {
            base.Deserialize(reader);
            Type = reader.ReadPackedUInt32();
            var tmsg = (RoR2DevToolMessageBase)Activator.CreateInstance(RoR2DevToolNetworkManager.RegisteredMessages[Type]);
            tmsg.Deserialize(reader);
            message = tmsg;
        }
    }
}
