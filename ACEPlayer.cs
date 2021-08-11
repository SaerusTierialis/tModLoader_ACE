using System;
using Terraria;
using Terraria.ModLoader;

namespace ACE
{
    public class ACEPlayer : ModPlayer
    {
        public Containers.Entity Entity { get; private set; }

        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Init/Deinit ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

        public override void Initialize()
        {
            Entity = new Containers.Entity(this);
        }

        public override void OnEnterWorld(Player player)
        {
            //initialize local data
            LocalData.SetLocalPlayer(this);

            //calc XP mult
            ////Systems.XPRewards.Rewards.UpdateXPMultiplier();

            //request world data from server
            if (ACE.NETMODE_CLIENT)
            {
                ////PacketHandler.ClientRequestWorldData.Send(-1, LocalData.WHO_AM_I);
            }
        }

        public override void PlayerConnect(Player player)
        {
            //anything here may be called several times in a row by a client joining a populated server
            ////Systems.XPRewards.Rewards.UpdateXPMultiplier();
        }

        public override void PlayerDisconnect(Player player)
        {
            ////Systems.XPRewards.Rewards.UpdateXPMultiplier();
        }

        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Update Cycle ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

        public override void PreUpdate()
        {
            Entity.PreUpdate();
        }
    }
}
