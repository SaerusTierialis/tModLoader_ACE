using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;

namespace ACE.Utilities
{
    public static class Logger
    {
        public static void Trace(string message)
        {
            if (ACE.NETMODE_SERVER)
            {
                message = "TRACE from Server: " + message;
                Console.WriteLine(message);
                NetMessage.BroadcastChatMessage(NetworkText.FromLiteral(message), UI.Constants.COLOUR_MESSAGE_TRACE);
            }
            else
            {
                if (ACE.NETMODE_CLIENT)
                {
                    message = "TRACE from Client#" + Main.LocalPlayer.whoAmI + ": " + message;
                    Main.NewText("Sending " + message, UI.Constants.COLOUR_MESSAGE_TRACE);
                    PacketHandler.ClientBroadcast.Send(-1, (byte)Main.LocalPlayer.whoAmI, message, PacketHandler.ClientBroadcast.BROADCAST_TYPE.TRACE);
                }
                else
                {
                    Main.NewText("TRACE: " + message, UI.Constants.COLOUR_MESSAGE_TRACE);
                }
            }
            if (ACE.MOD != null)
                ACE.MOD.Logger.Debug(message);
        }

        public static void Error(string message)
        {
            message = message + " (please report)";
            if (ACE.NETMODE_SERVER)
            {
                message = $"{ACE.MOD_NAME}-ERROR from Server: " + message;
                Console.WriteLine(message);
                NetMessage.BroadcastChatMessage(NetworkText.FromLiteral(message), UI.Constants.COLOUR_MESSAGE_ERROR);
            }
            else
            {
                if (ACE.NETMODE_CLIENT)
                {
                    message = $"{ACE.MOD_NAME}-ERROR from Client#" + Main.LocalPlayer.whoAmI + ": " + message;
                    Main.NewText("Sending " + message, UI.Constants.COLOUR_MESSAGE_ERROR);
                    PacketHandler.ClientBroadcast.Send(-1, (byte)Main.LocalPlayer.whoAmI, message, PacketHandler.ClientBroadcast.BROADCAST_TYPE.ERROR);
                }
                else
                {
                    Main.NewText($"{ACE.MOD_NAME}-ERROR: " + message, UI.Constants.COLOUR_MESSAGE_TRACE);
                }
            }
            ACE.MOD.Logger.Error(message);
        }
    }
}
