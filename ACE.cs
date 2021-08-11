using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ACE
{
	public class ACE : Mod
	{
		public static Mod MOD;
		public const string MOD_NAME = "ACE";

		/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Load/Unload ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
		public override void Load()
		{
			//shortcut to mod
			MOD = ModLoader.GetMod(MOD_NAME);

			//add serializers
			////Terraria.ModLoader.IO.TagSerializer.AddSerializer(new Containers.XPLevel.XPLevelSerializer());

			//reset local data
			LocalData.ResetLocalData();

			//set hotkeys
			////Hotkeys.Load();

			//load textures
			////Assets.Load();
		}

		public override void Unload()
		{
			//cleanup
			MOD = null;
			LocalData.ResetLocalData();
			////Hotkeys.Unload();
			////Assets.Unload();
		}

		/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Packets ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			//first 2 bytes are always type and origin
			byte packet_type = reader.ReadByte();
			int origin = reader.ReadInt32();

			if (packet_type >= 0 && packet_type < Utilities.PacketHandler.LOOKUP.Count)
			{
				Utilities.PacketHandler.LOOKUP[(Utilities.PacketHandler.PACKET_TYPE)packet_type].Recieve(reader, origin);
			}
			else
			{
				Utilities.Logger.Error("Cannot handle packet type id " + packet_type + " originating from " + origin);
			}
		}

		/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Netmode Shortcuts ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
		public static bool NETMODE_SERVER { get { return (Main.netMode == NetmodeID.Server); } }
		public static bool NETMODE_CLIENT { get { return (Main.netMode == NetmodeID.MultiplayerClient); } }
		public static bool NETMODE_SINGLEPLAYER { get { return (Main.netMode == NetmodeID.SinglePlayer); } }
		/// <summary>
		/// Client or singleplayer
		/// </summary>
		public static bool NETMODE_PLAYER { get { return (Main.netMode != NetmodeID.Server); } }
		/// <summary>
		/// Server or singleplayer
		/// </summary>
		public static bool NETMODE_EFFECTIVELY_SERVER { get { return (Main.netMode != NetmodeID.MultiplayerClient); } }
	}
}