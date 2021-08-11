using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace ACE.Utilities
{
    public static class PacketHandler
    {
        //IMPORTANT: each type MUST have a class with the exact same name
        public enum PACKET_TYPE : byte
        {
            ClientBroadcast,
        };

        //lookup for receiving of packets
        public static readonly Dictionary<PACKET_TYPE, Handler> LOOKUP = new Dictionary<PACKET_TYPE, Handler>();

        static PacketHandler()
        {
            foreach (PACKET_TYPE type in Enum.GetValues(typeof(PACKET_TYPE)))
            {
                LOOKUP[type] = Commons.CreateObjectFromName<Handler>(Enum.GetName(typeof(PACKET_TYPE), type), typeof(PacketHandler));
            }
        }

        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Base Type ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        public abstract class Handler
        {
            public PACKET_TYPE ID { get; private set; }
            public byte ID_Num { get; private set; }

            public Handler(PACKET_TYPE id)
            {
                ID = id;
                ID_Num = (byte)ID;
            }

            public ModPacket GetPacket(int origin)
            {
                ModPacket packet = ACE.MOD.GetPacket();
                packet.Write(ID_Num);
                packet.Write(origin);
                return packet;
            }

            public void Recieve(BinaryReader reader, int origin)
            {
                //do not read anything from reader here (called multiple times when processing full sync packet)

                bool do_trace = ConfigServer.Instance.Trace && (ID != PACKET_TYPE.ClientBroadcast);

                if (do_trace)
                {
                    Logger.Trace("Handling " + ID + " originating from " + origin);
                }

                ACEPlayer origin_ACEPlayer = null;
                if ((origin >= 0) && (origin <= Main.maxPlayers))
                {
                    origin_ACEPlayer = Main.player[origin].GetModPlayer<ACEPlayer>();
                }

                RecieveBody(reader, origin, origin_ACEPlayer);

                if (do_trace)
                {
                    Logger.Trace("Done handling " + ID + " originating from " + origin);
                }
            }

            protected abstract void RecieveBody(BinaryReader reader, int origin, ACEPlayer origin_ACEPlayer);
        }

        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Type Implementations ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        /// <summary>
        /// Client request broadcast from server
        /// </summary>
        public sealed class ClientBroadcast : Handler
        {
            private const PACKET_TYPE ptype = PACKET_TYPE.ClientBroadcast;

            public enum BROADCAST_TYPE : byte
            {
                MESSAGE,
                TRACE,
                ERROR
            }

            public ClientBroadcast() : base(ptype) { }

            public static void Send(int target, int origin, string message, BROADCAST_TYPE type)
            {
                //get packet containing header
                ModPacket packet = LOOKUP[ptype].GetPacket(origin);

                //type
                packet.Write((byte)type);

                //message
                packet.Write(message);

                //send
                packet.Send(target, origin);
            }

            protected override void RecieveBody(BinaryReader reader, int origin, ACEPlayer origin_ACEPlayer)
            {
                //type
                BROADCAST_TYPE type = (BROADCAST_TYPE)reader.ReadByte();

                //message
                string message = reader.ReadString();

                //colour
                Color colour = Color.White;
                switch (type)
                {
                    case BROADCAST_TYPE.ERROR:
                        colour = UI.Constants.COLOUR_MESSAGE_ERROR;
                        break;
                    case BROADCAST_TYPE.TRACE:
                        colour = UI.Constants.COLOUR_MESSAGE_TRACE;
                        break;
                    case BROADCAST_TYPE.MESSAGE:
                        colour = UI.Constants.COLOUR_MESSAGE_BROADCAST;
                        break;
                }

                //broadcast
                NetMessage.BroadcastChatMessage(NetworkText.FromLiteral(message), colour);

                //also write in console
                Console.WriteLine(message);
            }
        }


    }
}
